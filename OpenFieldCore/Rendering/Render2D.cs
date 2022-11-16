using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

using OpenTK.Graphics.OpenGL;

using OFC.Mathematics;
using OFC.Utility;
using OpenTK.Windowing.Common;
using System.Runtime.InteropServices;

namespace OFC.Rendering
{
    public static class Render2D
    {
        //Primitive Vertex Types
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct VPositionColour
        {
            public float X;
            public float Y;
            public uint Colour;
            public uint pad;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct VPositionColourTexture
        {
            public float x;
            public float y;
            public uint colour;
            public uint pad0x0c;
            public float u;
            public float v;
            public float slot;
            public float pad0x1c;
        }

        [Flags]
        private enum BatchFlags
        {
            None     = 0x0000,
            Position = 0x0001,
            Colour   = 0x0002,
            Texture  = 0x0004
        }

        //Data
        private static bool initialized = false;
        private static bool drawing = false;

        private static int batchVAO;
        private static int[] batchVBOs;
        private static int[] batchEBOs;
        private static int batchBufferNum = 3;
        private static int batchBufferID  = 0;

        private static readonly int maxBatchVertexCount = 25000;
        private static readonly int maxBatchIndexCount  = 20000;
        private static readonly int maxBatchVertexSize  = 8;

        private static BatchFlags batchCurrentFlags;
        private static int batchCount = 0;

        private static float[] batchBuffer;
        private static int batchVertexCount = 0;

        private static ushort[] batchIndices;
        private static int batchIndexCount = 0;
        
        private static Shader shaderColour = null;
        private static Shader shaderColourTexture = null;

        private static Texture batchTexture = null;

        private static Vector2i renderSize = new(1024, 768);
        private static Matrix44 cameraMatrix = Matrix44.CreateOrthographic(0, 0, 1024, 768, -2048f, 2048f);
        private static Colour blendColour = new Colour(1f, 1f, 1f, 1f);

        //Properties
        public static int BatchesThisFrame
        {
            get { return batchCount; }
        }

        /// <summary>
        /// Initializes the 2D renderer. Cannot be called more than once, unless uninitialized first.
        /// </summary>
        public static void Initialize()
        {
            if (initialized)
            {
                Log.Warn("Tried to initialize Render2D more than once!");
                return;
            }

            InitializeShaders();

            //Initialize batch buffers
            batchVBOs = new int[batchBufferNum];
            batchEBOs = new int[batchBufferNum];

            GL.GenBuffers(batchBufferNum, batchVBOs);
            GL.GenBuffers(batchBufferNum, batchEBOs);

            for (int i = 0; i < batchBufferNum; ++i)
            {
                Log.Info($"Buffers [Index = {i}, VBO = {batchVBOs[i]}, EBO = {batchEBOs[i]}]");

                GL.BindBuffer(BufferTarget.ArrayBuffer, batchVBOs[i]);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, batchEBOs[i]);

                GL.BufferData(BufferTarget.ArrayBuffer, 4 * (maxBatchVertexSize * maxBatchVertexCount), (IntPtr)null, BufferUsageHint.StreamDraw);
                GL.BufferData(BufferTarget.ElementArrayBuffer, 2 * maxBatchIndexCount, (IntPtr)null, BufferUsageHint.StreamDraw);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            batchVAO = GL.GenVertexArray();

            batchBuffer = new float[(maxBatchVertexSize * maxBatchVertexCount)];
            batchIndices = new ushort[maxBatchIndexCount];

            initialized = true;
        }

        private static void InitializeShaders()
        {
            string[] uniformNames = null, samplerNames = null;

            //Position - Colour
            uniformNames = new string[]
            {
                "camera",
                "colour"
            };

            shaderColour = new Shader("Resources\\Shader\\Colour2D.vss", "Resources\\Shader\\Colour2D.fss", uniformNames, samplerNames);

            //Position - Colour - Texture
            uniformNames = new string[] {
                "camera",
                "colour"
            };

            samplerNames = new string[]
            {
                "s2dTexture",
            };

            shaderColourTexture = new Shader("Resources\\Shader\\ColourTexture2D.vss", "Resources\\Shader\\ColourTexture2D.fss", uniformNames, samplerNames);
        }

        /// <summary>
        /// Unitializes the 2D renderer, freeing its resources.
        /// </summary>
        public static void Uninitialize()
        {
            if(!initialized)
            {
                Log.Warn("Tried to uninitialize Render2D before it was initialized!");
                return;
            }

            GL.DeleteVertexArray(batchVAO);
            GL.DeleteBuffers(batchBufferNum, batchEBOs);
            GL.DeleteBuffers(batchBufferNum, batchVBOs);

            shaderColour.Dispose();
            shaderColourTexture.Dispose();

            initialized = false;
        }

        public static void Resize(ResizeEventArgs ev)
        {
            renderSize.X = ev.Width;
            renderSize.Y = ev.Height;
            cameraMatrix = Matrix44.CreateOrthographic(0, 0, ev.Width, ev.Height, 0f, 2048f);
        }

        /// <summary>
        /// Begin Drawing to the screen
        /// </summary>
        public static void DrawBegin()
        {
            if(drawing)
            {
                Log.Warn("Tried to DrawBegin while already drawing");
                return;
            }
            GL.Viewport(0, 0, renderSize.X, renderSize.Y);

            batchCount = 0;
            BeginBatch(BatchFlags.Position | BatchFlags.Colour);

            //Enable the OpenGL state we want for 2D drawing
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        /// <summary>
        /// End Drawning to the screen
        /// </summary>
        public static void DrawEnd()
        {
            if (drawing)
            {
                Log.Warn("Tried to DrawEnd before calling DrawBegin!");
                return;
            }

            EndBatch();
            drawing = false;

            //Disable the OpenGL state we wanted for 2D drawing
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
        }

        /// <summary>
        /// Sets the base draw colour for all batches drawn to the screen.
        /// </summary>
        /// <param name="colour">The colour</param>
        public static void DrawSetColour(Colour colour)
        {
            blendColour = colour;
        }

        public static void DrawText(ref Vector2s xy, ref Font font, ref Colour colour, string text)
        {
            CheckBreakBatch(4 * text.Length, 6 * text.Length, BatchFlags.Position | BatchFlags.Colour | BatchFlags.Texture, font.fontAtlas);

            float X1 = xy.X;
            float Y1 = xy.Y;

            unsafe
            {
                for (int i = 0; i < text.Length; ++i)
                {
                    //Find glyph (OPTIMIZE PLEASEEEEE)
                    Font.JSONGlyph glyph = font.glyphs[0];
                    foreach(Font.JSONGlyph g in font.glyphs)
                    {
                        if(g.unicode == text[i])
                        {
                            glyph = g;
                            break;
                        }
                    }

                    if(glyph.unicode == 32)
                    {
                        X1 += glyph.advance;
                        continue;
                    }

                    float size = 32f;

                    //Write Vertices
                    fixed(void* buffer = &batchBuffer[(8 * batchVertexCount)])
                    {
                        VPositionColourTexture* vertex = (VPositionColourTexture*)buffer;

                        vertex[0].x = (X1 + (glyph.planeLeft * size));
                        vertex[0].y = (Y1 - (glyph.planeTop * size));
                        vertex[0].colour = colour.AsInteger;
                        vertex[0].u = glyph.atlasLeft / 512f;
                        vertex[0].v = (512f - glyph.atlasTop) / 512f;
                        vertex[0].slot = 0;

                        vertex[1].x = (X1 + (glyph.planeRight * size));
                        vertex[1].y = (Y1 - (glyph.planeTop * size));
                        vertex[1].colour = colour.AsInteger;
                        vertex[1].u = glyph.atlasRight / 512f;
                        vertex[1].v = (512f - glyph.atlasTop) / 512f;
                        vertex[1].slot = 0;

                        vertex[2].x = (X1 + (glyph.planeLeft * size));
                        vertex[2].y = (Y1 - (glyph.planeBottom * size));
                        vertex[2].colour = colour.AsInteger;
                        vertex[2].u = glyph.atlasLeft / 512f;
                        vertex[2].v = (512f - glyph.atlasBottom) / 512f;
                        vertex[2].slot = 0;

                        vertex[3].x = (X1 + (glyph.planeRight * size));
                        vertex[3].y = (Y1 - (glyph.planeBottom * size));
                        vertex[3].colour = colour.AsInteger;
                        vertex[3].u = glyph.atlasRight / 512f;
                        vertex[3].v = (512f - glyph.atlasBottom) / 512f;
                        vertex[3].slot = 0;
                    }

                    X1 += glyph.advance * size;

                    //Write Indices
                    fixed (ushort* buffer = &batchIndices[batchIndexCount])
                    {
                        buffer[0] = (ushort)(batchVertexCount + 2);
                        buffer[1] = (ushort)(batchVertexCount + 1);
                        buffer[2] = (ushort)(batchVertexCount + 0);
                        buffer[3] = (ushort)(batchVertexCount + 2);
                        buffer[4] = (ushort)(batchVertexCount + 3);
                        buffer[5] = (ushort)(batchVertexCount + 1);
                    }

                    batchVertexCount += 4;
                    batchIndexCount  += 6;
                }
            }
        }

        /// <summary>
        /// Draws a gradient colour rectangle to the screen
        /// </summary>
        /// <param name="xy">XY position (top left origin)</param>
        /// <param name="wh">WH size</param>
        /// <param name="TLC">Top left colour</param>
        /// <param name="TRC">Top right colour</param>
        /// <param name="BLC">Bottom left colour</param>
        /// <param name="BRC">Bottom right colour</param>
        public static void DrawRectangle(ref Vector2s xy, ref Vector2s wh, ref Colour TLC, ref Colour TRC, ref Colour BLC, ref Colour BRC)
        {
            CheckBreakBatch(4, 6, BatchFlags.Position | BatchFlags.Colour, null);

            ushort indexCount = (ushort)batchVertexCount;

            float X1 = xy.X;
            float Y1 = xy.Y;
            float X2 = xy.X + wh.X;
            float Y2 = xy.Y + wh.Y;

            //Attempt data write using unsafe code
            unsafe
            {
                //Vertex Buffer Write
                fixed (void* buffer = &batchBuffer[(4 * batchVertexCount)])
                {
                    VPositionColour* vertex = (VPositionColour*)buffer;

                    vertex[0].X = X1;
                    vertex[0].Y = Y1;
                    vertex[0].Colour = TLC.AsInteger;
                    vertex[1].X = X2;
                    vertex[1].Y = Y1;
                    vertex[1].Colour = TRC.AsInteger;
                    vertex[2].X = X1;
                    vertex[2].Y = Y2;
                    vertex[2].Colour = BLC.AsInteger;
                    vertex[3].X = X2;
                    vertex[3].Y = Y2;
                    vertex[3].Colour = BRC.AsInteger;
                }

                //Index Buffer Write
                fixed(ushort* buffer = &batchIndices[batchIndexCount])
                {
                    buffer[2] = indexCount;   //Index + 0, Triangle 1
                    buffer[1] = ++indexCount; //Index + 1, Triangle 1
                    buffer[5] = indexCount;   //Index + 1, Triangle 2

                    buffer[0] = ++indexCount; //Index + 2, Triangle 1 
                    buffer[3] = indexCount;   //Index + 2, Triangle 2
                    buffer[4] = ++indexCount; //Index + 3, Triangle 2
                }
            }

            batchVertexCount += 4;
            batchIndexCount += 6;
        }

        public static void DrawRectangle(ref Vector2s xy, ref Vector2s wh, ref Colour TLC, ref Colour TRC, ref Colour BLC, ref Colour BRC, ref Texture texture)
        {
            CheckBreakBatch(4, 6, BatchFlags.Position | BatchFlags.Colour | BatchFlags.Texture, texture);

            ushort indexCount = (ushort)batchVertexCount;

            float X1 = xy.X;
            float Y1 = xy.Y;
            float X2 = xy.X + wh.X;
            float Y2 = xy.Y + wh.Y;

            //Attempt data write using unsafe code
            unsafe
            {
                //Vertex Buffer Write
                fixed (void* buffer = &batchBuffer[(8 * batchVertexCount)])
                {
                    VPositionColourTexture* vertex = (VPositionColourTexture*)buffer;

                    vertex[0].x = X1;
                    vertex[0].y = Y1;
                    vertex[0].colour = TLC.AsInteger;
                    vertex[0].u = 0f;
                    vertex[0].v = 0f;
                    vertex[0].slot = 0;

                    vertex[1].x = X2;
                    vertex[1].y = Y1;
                    vertex[1].colour = TRC.AsInteger;
                    vertex[1].u = 1f;
                    vertex[1].v = 0f;
                    vertex[1].slot = 0;

                    vertex[2].x = X1;
                    vertex[2].y = Y2;
                    vertex[2].colour = BLC.AsInteger;
                    vertex[2].u = 0f;
                    vertex[2].v = 1f;
                    vertex[2].slot = 0;

                    vertex[3].x = X2;
                    vertex[3].y = Y2;
                    vertex[3].colour = BRC.AsInteger;
                    vertex[3].u = 1f;
                    vertex[3].v = 1f;
                    vertex[3].slot = 0;
                }

                //Index Buffer Write
                fixed (ushort* buffer = &batchIndices[batchIndexCount])
                {
                    buffer[2] = indexCount;   //Index + 0, Triangle 1
                    buffer[1] = ++indexCount; //Index + 1, Triangle 1
                    buffer[5] = indexCount;   //Index + 1, Triangle 2

                    buffer[0] = ++indexCount; //Index + 2, Triangle 1 
                    buffer[3] = indexCount;   //Index + 2, Triangle 2
                    buffer[4] = ++indexCount; //Index + 3, Triangle 2
                }
            }

            batchVertexCount += 4;
            batchIndexCount += 6;
        }

        /// <summary>
        /// Draws a single colour rectangle to the screen
        /// </summary>
        /// <param name="xy">XY position (top left origin)</param>
        /// <param name="wh">WH size</param>
        /// <param name="colour">Colour</param>
        public static void DrawRectangle(ref Vector2s xy, ref Vector2s wh, ref Colour colour)
        {
            DrawRectangle(ref xy, ref wh, ref colour, ref colour, ref colour, ref colour);
        }

        /// <summary>
        /// Draws a rotated gradient colour rectangle to the screen
        /// </summary>
        /// <param name="xy">XY position (top left origin)</param>
        /// <param name="wh">WH size</param>
        /// <param name="angle">Rotation angle (in degrees)</param>
        /// <param name="TLC">Top left colour</param>
        /// <param name="TRC">Top right colour</param>
        /// <param name="BLC">Bottom left colour</param>
        /// <param name="BRC">Bottom right colour</param>
        public static void DrawRectangle(ref Vector2s xy, ref Vector2s wh, ref float angle, ref Colour TLC, ref Colour TRC, ref Colour BLC, ref Colour BRC)
        {
            CheckBreakBatch(4, 6, BatchFlags.Position | BatchFlags.Colour, null);

            float wd = wh.X * 0.5f;
            float hd = wh.Y * 0.5f;
            float cx = xy.X + wd;
            float cy = xy.Y + hd;

            float arad = (angle + 45f) * 0.0174533f;

            float xrot = ((MathF.Cos(arad) * 1.41421356237f) * wd);
            float yrot = ((MathF.Sin(arad) * 1.41421356237f) * hd);

            //Attempt data write using unsafe code
            unsafe
            {
                //Vertex Buffer Write
                fixed (void* buffer = &batchBuffer[(4 * batchVertexCount)])
                {
                    VPositionColour* vertex = (VPositionColour*)buffer;

                    vertex[0].X = cx - xrot;
                    vertex[0].Y = cy - yrot;
                    vertex[0].Colour = TLC.AsInteger;
                    vertex[1].X = cx + yrot;
                    vertex[1].Y = cy - xrot;
                    vertex[1].Colour = TRC.AsInteger;
                    vertex[2].X = cx - yrot;
                    vertex[2].Y = cy + xrot;
                    vertex[2].Colour = BLC.AsInteger;
                    vertex[3].X = cx + xrot;
                    vertex[3].Y = cy + yrot;
                    vertex[3].Colour = BRC.AsInteger;
                }

                //Index Buffer Write
                fixed (ushort* buffer = &batchIndices[batchIndexCount])
                {
                    //Triangle 1
                    buffer[0] = (ushort)(batchVertexCount + 2);
                    buffer[1] = (ushort)(batchVertexCount + 1);
                    buffer[2] = (ushort)batchVertexCount;

                    //Triangle 2
                    buffer[3] = (ushort)(batchVertexCount + 2);
                    buffer[4] = (ushort)(batchVertexCount + 3);
                    buffer[5] = (ushort)(batchVertexCount + 1);
                }
            }

            batchVertexCount += 4;
            batchIndexCount += 6;
        }

        /// <summary>
        /// Draws a rotated gradient colour rectangle to the screen
        /// </summary>
        /// <param name="xy">XY position (top left origin)</param>
        /// <param name="wh">WH size</param>
        /// <param name="angle">Rotation angle (in degrees)</param>
        /// <param name="colour">Colour</param>
        public static void DrawRectangle(ref Vector2s xy, ref Vector2s wh, ref float angle, ref Colour colour)
        {
            DrawRectangle(ref xy, ref wh, ref angle, ref colour, ref colour, ref colour, ref colour);
        }

        /// <summary>
        /// Draws a gradient colour line to the screen
        /// </summary>
        /// <param name="start">Start point of the line</param>
        /// <param name="end">End point of the line</param>
        /// <param name="width">Width of the line</param>
        /// <param name="colourStart">Colour at the start point of the line</param>
        /// <param name="colourEnd">Colour at the end point of the line</param>
        public static void DrawLine(ref Vector2s start, ref Vector2s end, ref float width, ref Colour colourStart, ref Colour colourEnd)
        {
            CheckBreakBatch(4, 6, BatchFlags.Position | BatchFlags.Colour, null);

            //Find perpendiculars
            Vector2s unit = Vector2s.Subtract(start, end);
            unit.Normalize();

            float halfWidth = width * 0.5f;
            float dxHalf = halfWidth * unit.X;
            float dyHalf = halfWidth * unit.Y;

            ushort indexCount = (ushort)batchVertexCount;

            //Attempt data write using unsafe code
            unsafe
            {
                //Vertex Buffer Write
                fixed (void* buffer = &batchBuffer[4 * batchVertexCount])
                {
                    float* vertex = (float*)buffer;
                    uint* colour = (uint*)buffer;

                    //vertex 1
                    vertex[0] = start.X + dyHalf;
                    vertex[1] = start.Y - dxHalf;
                    colour[2] = colourStart.AsInteger;

                    //vertex 2
                    vertex[4] = start.X - dyHalf;
                    vertex[5] = start.Y + dxHalf;
                    colour[6] = colourStart.AsInteger;

                    //vertex 3
                    vertex[8] = end.X + dyHalf;
                    vertex[9] = end.Y - dxHalf;
                    colour[10] = colourEnd.AsInteger;

                    //vertex 4
                    vertex[12] = end.X - dyHalf;
                    vertex[13] = end.Y + dxHalf;
                    colour[14] = colourEnd.AsInteger;
                }

                //Index Buffer Write
                fixed (ushort* buffer = &batchIndices[batchIndexCount])
                {
                    buffer[0] = indexCount;     //Index + 0, Triangle 1
                    buffer[1] = ++indexCount;   //Index + 1, Triangle 1
                    buffer[5] = indexCount;     //Index + 1, Triangle 2

                    buffer[2] = ++indexCount;   //Index + 2, Triangle 1
                    buffer[3] = indexCount;     //Index + 2, Triangle 2
                    buffer[4] = ++indexCount;   //Index + 3, Triangle 2
                }
            }

            batchVertexCount += 4;
            batchIndexCount += 6;
        }

        /// <summary>
        /// Draws a single colour line to the screen
        /// </summary>
        /// <param name="start">Start point of the line</param>
        /// <param name="end">End point of the line</param>
        /// <param name="width">Width of the line</param>
        /// <param name="colour">Colour of the line</param>
        public static void DrawLine(ref Vector2s start, ref Vector2s end, ref float width, ref Colour colour)
        {
            DrawLine(ref start, ref end, ref width, ref colour, ref colour);
        }

        /// <summary>
        /// Draws a gradient colour circle to the screen
        /// </summary>
        /// <param name="origin">Origin of the circle</param>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="originColour">Colour at the center of the circle</param>
        /// <param name="edgeColour">Colour at the edge of the circle</param>
        public static void DrawCircle(ref Vector2s origin, ref float radius, ref Colour originColour, ref Colour edgeColour)
        {
            CheckBreakBatch(37, 108, BatchFlags.Position | BatchFlags.Colour, null);

            float angle = -3.141592655f;
            ushort preVertexCount = (ushort)batchVertexCount;

            unsafe
            {
                //Here we assume the first two vertices of the circle.
                fixed (void* vertexBuffer = &batchBuffer[4 * batchVertexCount])
                {
                    float* position = (float*)vertexBuffer;
                    uint* colour = (uint*)vertexBuffer;

                    //Center point will always be the same
                    position[0] = origin.X;
                    position[1] = origin.Y;
                    colour[2] = originColour.AsInteger;

                    //Assume the edge vertex, which allows us to only use cos/sin once per loop.
                    position[4] = origin.X - radius;
                    position[5] = origin.Y;
                    colour[6] = edgeColour.AsInteger;
                }
                batchVertexCount += 2;

                //we do one segmant less than needed to create a full circle, because the last segments vertices will already exist...
                for (int i = 0; i < 35; ++i)
                {
                    fixed (ushort* ibuffer = &batchIndices[batchIndexCount])
                    {
                        ibuffer[0] = preVertexCount;
                        ibuffer[1] = (ushort)batchVertexCount;
                        ibuffer[2] = (ushort)(batchVertexCount - 1);
                    }
                    batchIndexCount += 3;

                    angle += 0.1745329252777778f;

                    fixed (void* vertexBuffer = &batchBuffer[4 * batchVertexCount])
                    {
                        float* position = (float*)vertexBuffer;
                        uint* colour = (uint*)vertexBuffer;

                        position[0] = origin.X + FastMath.Cos2(angle) * radius;
                        position[1] = origin.Y + FastMath.Sin2(angle) * radius;
                        colour[2] = edgeColour.AsInteger;
                    }

                    batchVertexCount++;
                }

                //The last segment
                fixed (ushort* ibuffer = &batchIndices[batchIndexCount])
                {
                    ibuffer[0] = preVertexCount;
                    ibuffer[1] = (ushort)(preVertexCount + 1);
                    ibuffer[2] = (ushort)(batchVertexCount - 1);

                }
                batchIndexCount += 3;
            }
        }

        //Batching Helpers
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BeginBatch(BatchFlags flags)
        {
            //Set up batch type.
            switch (flags)
            {
                case BatchFlags.None:
                    batchCurrentFlags = BatchFlags.None;
                    batchVertexCount = 0;
                    batchIndexCount = 0;
                    return;

                case (BatchFlags.Position | BatchFlags.Colour):
                    shaderColour.Bind();
                    shaderColour.SetMatrix44("camera", ref cameraMatrix);
                    shaderColour.SetVector4("colour", ref blendColour);
                    break;

                case (BatchFlags.Position | BatchFlags.Colour | BatchFlags.Texture):
                    shaderColourTexture.Bind();
                    shaderColourTexture.SetMatrix44("camera", ref cameraMatrix);
                    shaderColourTexture.SetVector4("colour", ref blendColour);
                    shaderColourTexture.SetSampler2D("s2dTexture", 0);
                    break;

                default:
                    Log.Error($"Invalid Batch Flags: {flags}");
                    break;
            }

            batchVertexCount = 0;
            batchIndexCount = 0;
            batchCurrentFlags = flags;

            batchCount++;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EndBatch()
        {
            GL.BindVertexArray(batchVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, batchVBOs[batchBufferID]);  //We triple buffer to avoid CPU-GPU sync as much as possible.

            //What kind of batch are we finalizing?
            switch (batchCurrentFlags)
            {
                case (BatchFlags.Position | BatchFlags.Colour):
                    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (16 * batchVertexCount), batchBuffer);

                    GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 16, 0);
                    GL.EnableVertexAttribArray(0);

                    GL.VertexAttribPointer(1, 4, VertexAttribPointerType.UnsignedByte, false, 16, 8);
                    GL.EnableVertexAttribArray(1);
                    break;

                case (BatchFlags.Position | BatchFlags.Colour | BatchFlags.Texture):
                    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (32 * batchVertexCount), batchBuffer);

                    GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 32, 0);
                    GL.EnableVertexAttribArray(0);

                    GL.VertexAttribPointer(1, 4, VertexAttribPointerType.UnsignedByte, false, 32, 8);
                    GL.EnableVertexAttribArray(1);

                    GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 32, 16);
                    GL.EnableVertexAttribArray(2);

                    batchTexture.Bind(0);
                    break;

                default:
                    GL.BindVertexArray(0);
                    return;
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, batchEBOs[batchBufferID]);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, 2 * batchIndexCount, batchIndices);

            GL.DrawElements(PrimitiveType.Triangles, batchIndexCount, DrawElementsType.UnsignedShort, 0);           

            batchBufferID = (batchBufferID + 1) % batchBufferNum; //Cycle to the next buffer.
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckBreakBatch(int vertexNum, int indexNum, BatchFlags pattern, Texture texture)
        {
            if(texture != null)
            {
                if(!texture.Equals(batchTexture))
                {
                    batchTexture = texture;

                    EndBatch();
                    BeginBatch(pattern);

                    return;
                }
            }

            if ((batchCurrentFlags != pattern) || ((batchVertexCount + vertexNum) >= maxBatchVertexCount) || ((batchIndexCount + indexNum) >= maxBatchIndexCount))
            {
                EndBatch();
                BeginBatch(pattern);
            }
        }
    }
}
