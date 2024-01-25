using System;
using System.Runtime.CompilerServices;

using OFC.Numerics;
using OFC.Resource.Texture;

namespace OFC.Rendering
{
    public unsafe static partial class Render2D
    {
        /// <summary>
        /// Draws a single colour rectangle primitive
        /// </summary>
        /// <param name="xy1">First Position</param>
        /// <param name="xy2">Second Position</param>
        /// <param name="c">Colour</param>
        public static void DrawRectangle(ref Vector2f xy1, ref Vector2f xy2, ref Colour c)
        {
            DrawRectangleInternal(ref xy1, ref xy2, ref c, ref c, ref c, ref c);
        }

        /// <summary>
        /// Draws a single colour rotated rectangle primitive
        /// </summary>
        /// <param name="xy">Position</param>
        /// <param name="wh">Size</param>
        /// <param name="angle">Rotation in degrees</param>
        /// <param name="c">Colour</param>
        public static void DrawRectangle(ref Vector2f xy, ref Vector2f wh, float angle, ref Colour c)
        {
            DrawRectangleInternal(ref xy, ref wh, angle, ref c, ref c, ref c, ref c);
        }

        /// <summary>
        /// Draws a textured gradient colour rectangle primitive
        /// </summary>
        /// <param name="xy1">First Position</param>
        /// <param name="xy2">Second Position</param>
        /// <param name="texture">Texture</param>
        /// <param name="c">Colour</param>
        public static void DrawRectangle(ref Vector2f xy1, ref Vector2f xy2, TextureResource texture, ref Colour c)
        {
            DrawRectangleInternal(ref xy1, ref xy2, texture, ref c, ref c, ref c, ref c);
        }

        /// <summary>
        /// Draws a gradiant colour rectangle primitive
        /// </summary>
        /// <param name="xy1">First Position</param>
        /// <param name="xy2">Second Position</param>
        /// <param name="c1">Top Left Colour</param>
        /// <param name="c2">Top Right Colour</param>
        /// <param name="c3">Bottom Left Colour</param>
        /// <param name="c4">Bottom Right Colour</param>
        public static void DrawRectangle(ref Vector2f xy1, ref Vector2f xy2, ref Colour c1, ref Colour c2, ref Colour c3, ref Colour c4)
        {
            DrawRectangleInternal(ref xy1, ref xy2, ref c1, ref c2, ref c3, ref c4);
        }

        /// <summary>
        /// Draws a gradient colour rotated rectangle primitive
        /// </summary>
        /// <param name="xy">Position</param>
        /// <param name="wh">Size</param>
        /// <param name="angle">Rotation in degrees</param>
        /// <param name="c1">Top Left Colour</param>
        /// <param name="c2">Top Right Colour</param>
        /// <param name="c3">Bottom Left Colour</param>
        /// <param name="c4">Bottom Right Colour</param>
        public static void DrawRectangle(ref Vector2f xy1, ref Vector2f xy2, float angle, ref Colour c1, ref Colour c2, ref Colour c3, ref Colour c4)
        {
            DrawRectangleInternal(ref xy1, ref xy2, angle, ref c1, ref c2, ref c3, ref c4);
        }

        /// <summary>
        /// Draws a gradient colour rectangle primitive (internal)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawRectangleInternal(ref Vector2f xy1, ref Vector2f xy2, ref Colour c1, ref Colour c2, ref Colour c3, ref Colour c4)
        {
            //Check if the batch format is incompatible.
            BatchCheck(BatchVertexComponents.Position | BatchVertexComponents.Colour, 4, 6);

            //Write vertices to batch
            fixed (void* buffer = &batchBuffer[batchBufferID].componentBuffer[4 * batchVertexCount])
            {
                //Cast to our structure for easy writing
                VertexPostionColour* vertices = (VertexPostionColour*)buffer;

                //Top Left
                vertices[0].x = xy1.X;
                vertices[0].y = xy1.Y;
                vertices[0].rgba = c1.AsInteger;

                //Top Right
                vertices[1].x = xy2.X;
                vertices[1].y = xy1.Y;
                vertices[1].rgba = c2.AsInteger;

                //Bottom Left
                vertices[2].x = xy1.X;
                vertices[2].y = xy2.Y;
                vertices[2].rgba = c3.AsInteger;

                //Bottom Right
                vertices[3].x = xy2.X;
                vertices[3].y = xy2.Y;
                vertices[3].rgba = c4.AsInteger;
            }

            //Write indices to batch
            ushort currentVertex = (ushort)batchVertexCount;

            fixed (ushort* indices = &batchBuffer[batchBufferID].indexBuffer[batchIndexCount])
            {
                indices[2] = currentVertex;     //Triangle 1, Index + 0
                indices[1] = ++currentVertex;   //Triangle 1, Index + 1
                indices[5] = currentVertex;     //Triangle 2, Index + 1
                indices[0] = ++currentVertex;   //Triangle 1, Index + 2
                indices[3] = currentVertex;     //Triangle 2, Index + 2
                indices[4] = ++currentVertex;   //Triangle 2, Index + 3
            }

            //Add the number of vertices and indices to the batch
            batchVertexCount += 4;
            batchIndexCount += 6;
        }

        /// <summary>
        /// Draws a textured gradient colour rectangle primitive (internal)
        /// </summary>
        private static void DrawRectangleInternal(ref Vector2f xy1, ref Vector2f xy2, TextureResource texture, ref Colour c1, ref Colour c2, ref Colour c3, ref Colour c4)
        {
            //Check if the batch format is incompatible.
            BatchCheck(BatchVertexComponents.Position | BatchVertexComponents.Colour | BatchVertexComponents.Texcoord, texture, 4, 6);

            //Write vertices to batch
            fixed (void* buffer = &batchBuffer[batchBufferID].componentBuffer[8 * batchVertexCount])
            {
                //Cast to our structure for easy writing
                VertexPositionColourTexcoord* vertices = (VertexPositionColourTexcoord*)buffer;

                //Top Left
                vertices[0].x = xy1.X;
                vertices[0].y = xy1.Y;
                vertices[0].rgba = c1.AsInteger;
                vertices[0].u = 0f;
                vertices[0].v = 0f;

                //Top Right
                vertices[1].x = xy2.X;
                vertices[1].y = xy1.Y;
                vertices[1].rgba = c2.AsInteger;
                vertices[1].u = 1f;
                vertices[1].v = 0f;

                //Bottom Left
                vertices[2].x = xy1.X;
                vertices[2].y = xy2.Y;
                vertices[2].rgba = c3.AsInteger;
                vertices[2].u = 0f;
                vertices[2].v = 1f;

                //Bottom Right
                vertices[3].x = xy2.X;
                vertices[3].y = xy2.Y;
                vertices[3].rgba = c4.AsInteger;
                vertices[3].u = 1f;
                vertices[3].v = 1f;
            }

            //Write indices to batch
            ushort currentVertex = (ushort)batchVertexCount;

            fixed (ushort* indices = &batchBuffer[batchBufferID].indexBuffer[batchIndexCount])
            {
                indices[2] = currentVertex;     //Triangle 1, Index + 0
                indices[1] = ++currentVertex;   //Triangle 1, Index + 1
                indices[5] = currentVertex;     //Triangle 2, Index + 1
                indices[0] = ++currentVertex;   //Triangle 1, Index + 2
                indices[3] = currentVertex;     //Triangle 2, Index + 2
                indices[4] = ++currentVertex;   //Triangle 2, Index + 3
            }

            //Add the number of vertices and indices to the batch
            batchVertexCount += 4;
            batchIndexCount += 6;
        }

        /// <summary>
        /// Draws a gradient colour rotated rectangle primitive (internal)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawRectangleInternal(ref Vector2f xy, ref Vector2f wh, float angle, ref Colour c1, ref Colour c2, ref Colour c3, ref Colour c4)
        {
            //Check if the batch format is incompatible.
            BatchCheck(BatchVertexComponents.Position | BatchVertexComponents.Colour, 4, 6);

            float hw = wh.X * 0.5f;
            float hh = wh.Y * 0.5f;
            float cx = xy.X + hw;
            float cy = xy.Y + hh;
            float arad = (angle + 45f) * 0.0174533f;
            float xrot = ((MathF.Cos(arad) * 1.41421356237f) * hw);
            float yrot = ((MathF.Sin(arad) * 1.41421356237f) * hh);

            //Write vertices to batch
            fixed (void* buffer = &batchBuffer[batchBufferID].componentBuffer[4 * batchVertexCount])
            {
                //Cast to our structure for easy writing
                VertexPostionColour* vertices = (VertexPostionColour*)buffer;

                //Top Left
                vertices[0].x = cx - xrot;
                vertices[0].y = cy - yrot;
                vertices[0].rgba = c1.AsInteger;

                //Top Right
                vertices[1].x = cx + yrot;
                vertices[1].y = cy - xrot;
                vertices[1].rgba = c2.AsInteger;

                //Bottom Left
                vertices[2].x = cx - yrot;
                vertices[2].y = cy + xrot;
                vertices[2].rgba = c3.AsInteger;

                //Bottom Right
                vertices[3].x = cx + xrot;
                vertices[3].y = cy + yrot;
                vertices[3].rgba = c4.AsInteger;
            }

            //Write indices to batch
            ushort currentVertex = (ushort)batchVertexCount;

            fixed (ushort* indices = &batchBuffer[batchBufferID].indexBuffer[batchIndexCount])
            {
                indices[2] = currentVertex;     //Triangle 1, Index + 0
                indices[1] = ++currentVertex;   //Triangle 1, Index + 1
                indices[5] = currentVertex;     //Triangle 2, Index + 1
                indices[0] = ++currentVertex;   //Triangle 1, Index + 2
                indices[3] = currentVertex;     //Triangle 2, Index + 2
                indices[4] = ++currentVertex;   //Triangle 2, Index + 3
            }

            //Add the number of vertices and indices to the batch
            batchVertexCount += 4;
            batchIndexCount += 6;
        }
    }
}
