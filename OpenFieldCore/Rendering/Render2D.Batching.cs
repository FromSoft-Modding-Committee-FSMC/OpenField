using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL;

using OFC.Resource.Texture;

namespace OFC.Rendering
{
    public unsafe static partial class Render2D
    {
        static delegate*<void> BatchEnd = &BatchEndDefault;

        /// <summary>
        /// Allows to begin a batch.
        /// </summary>
        /// <param name="components">The pattern vertices will be expected in</param>
        private static void BatchBegin(BatchVertexComponents components, TextureResource texture = null)
        {
            switch(components)
            {
                //Default Type (No Batch)
                case BatchVertexComponents.None:
                    BatchEnd = &BatchEndDefault;
                    break;

                case BatchVertexComponents.Position | BatchVertexComponents.Colour:
                    BatchEnd = &BatchEndPositionColour;
                    break;


                case BatchVertexComponents.Position | BatchVertexComponents.Colour | BatchVertexComponents.Texcoord:
                    BatchEnd = &BatchEndPositionColourTexcoord;
                    break;
            }

            //Set current texture
            batchTexture = texture;

            //Reset the current buffer.
            batchVertexCount = 0;
            batchIndexCount = 0;
            batchComponents = components;
        }

        /// <summary>
        /// This is a stub BatchEnd which does nothing.
        /// </summary>
        private static void BatchEndDefault()
        {

        }

        /// <summary>
        /// Defines a 2D Vertex with a position and colour
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct VertexPostionColour
        {
            public float x;
            public float y;
            public uint  rgba;
            public uint  pad1;
        }

        /// <summary>
        /// Ends a Position/Colour type batch.
        /// </summary>
        private static void BatchEndPositionColour()
        {
            //Bind Buffers
            GL.BindVertexArray(batchBuffer[batchBufferID].glVAO);

            //Copy Vertex Data
            GL.BindBuffer(BufferTarget.ArrayBuffer, batchBuffer[batchBufferID].glVBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(VertexPostionColour) * batchVertexCount, batchBuffer[batchBufferID].componentBuffer);

            //Copy Index Data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, batchBuffer[batchBufferID].glIBO);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, 2 * batchIndexCount, batchBuffer[batchBufferID].indexBuffer);

            //Configure Vertex Array
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 16, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.UnsignedByte, false, 16, 8);
            GL.EnableVertexAttribArray(1);

            //Set up shader
            shdColour.Bind();
            shdColour.SetMatrix44("camera", ref camera);

            //Draw
            GL.DrawElements(PrimitiveType.Triangles, batchIndexCount, DrawElementsType.UnsignedShort, 0);

            //Cycle to the next batch buffer
            batchBufferID = (batchBufferID + 1) % maxBatchBuffers;

            //Increment batch counter
            batchCount++;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct VertexPositionColourTexcoord
        {
            public float x;
            public float y;
            public uint rgba;
            public uint pad1;
            public float u;
            public float v;
            public uint pad2;
            public uint pad3;
        }

        /// <summary>
        /// Ends a Position/Colour/Texcoord type batch
        /// </summary>
        private static void BatchEndPositionColourTexcoord()
        {
            //Bind Buffers
            GL.BindVertexArray(batchBuffer[batchBufferID].glVAO);

            //Copy Vertex Data
            GL.BindBuffer(BufferTarget.ArrayBuffer, batchBuffer[batchBufferID].glVBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(VertexPositionColourTexcoord) * batchVertexCount, batchBuffer[batchBufferID].componentBuffer);

            //Copy Index Data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, batchBuffer[batchBufferID].glIBO);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, sizeof(ushort) * batchIndexCount, batchBuffer[batchBufferID].indexBuffer);

            //Configure Vertex Array
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 32, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.UnsignedByte, false, 32, 8);
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 32, 16);
            GL.EnableVertexAttribArray(2);

            //Set up shader
            shdColourTexture.Bind();
            shdColourTexture.SetMatrix44("camera", ref camera);
            shdColourTexture.SetSampler2D("sDiffuse", 1);

            //Bind Texture
            batchTexture.Bind();

            //Draw
            GL.DrawElements(PrimitiveType.Triangles, batchIndexCount, DrawElementsType.UnsignedShort, 0);

            //Cycle to the next batch buffer
            batchBufferID = (batchBufferID + 1) % maxBatchBuffers;

            //Increment batch counter
            batchCount++;
        }

        /// <summary>
        /// Checks if a batch is ready to be broken.
        /// If a batch is ready to be broken, it will automatically cycle buffers and start a new batch.
        /// </summary>
        /// <param name="pattern">The pattern vertices are expected in.</param>
        /// <param name="addedVertexCount">The number of vertices the drawing primitive needs (i.e 4 for rect)</param>
        /// <param name="addedIndexCount">The number of indices the drawing primitive needs (i.e 6 for rect)</param>
        /// <returns>'True' if the batch was broken, 'False' if it was not.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool BatchCheck(BatchVertexComponents pattern, int addedVertexCount, int addedIndexCount)
        {
            //The easiest thing to check is the pattern. If that is different, we automatically return true.
            if (pattern != batchComponents)
                goto BatchBreak;

            //Now check if completing the parent call would overflow our vertex/index buffers.
            if ((batchVertexCount + addedVertexCount) > BatchBuffer.maxVertices)
                goto BatchBreak;

            if ((batchIndexCount + addedIndexCount) > BatchBuffer.maxIndices)
                goto BatchBreak;

            //Add more checks here. Rearange checks to whatever is considered to be more common.
            return false;

        BatchBreak:
            BatchEnd();
            BatchBegin(pattern);
            return true;
        }

        /// <summary>
        /// Checks if a batch is ready to be broken.
        /// If a batch is ready to be broken, it will automatically cycle buffers and start a new batch.
        /// </summary>
        /// <param name="pattern">The pattern vertices are expected in.</param>
        /// <param name="addedVertexCount">The number of vertices the drawing primitive needs (i.e 4 for rect)</param>
        /// <param name="addedIndexCount">The number of indices the drawing primitive needs (i.e 6 for rect)</param>
        /// <returns>'True' if the batch was broken, 'False' if it was not.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool BatchCheck(BatchVertexComponents pattern, TextureResource texture, int addedVertexCount, int addedIndexCount)
        {
            //The easiest thing to check is the pattern. If that is different, we automatically return true.
            if(pattern != batchComponents)
                goto BatchBreak;

            //The next thing is textures.
            if(batchTexture.Hash != texture.Hash)
                goto BatchBreak;

            //Now check if completing the parent call would overflow our vertex/index buffers.
            if ((batchVertexCount + addedVertexCount) > BatchBuffer.maxVertices)
                goto BatchBreak;

            if ((batchIndexCount + addedIndexCount) > BatchBuffer.maxIndices)
                goto BatchBreak;

            //Add more checks here. Rearange checks to whatever is considered to be more common.
            return false;

        BatchBreak:
            BatchEnd();
            BatchBegin(pattern, texture);
            return true;
        }
    }
}
