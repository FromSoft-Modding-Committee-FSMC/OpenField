using System;
using System.Runtime.CompilerServices;
using OFC.Mathematics;
using OFC.Numerics;

namespace OFC.Rendering
{
    public unsafe static partial class Render2D
    {
        /// <summary>
        /// Draws a single colour circle primitive
        /// </summary>
        /// <param name="center">center (origin) point</param>
        /// <param name="radius">radius</param>
        /// <param name="c">colour</param>
        public static void DrawCircle(ref Vector2f center, float radius, ref Colour c)
        {
            DrawCircleInternal(ref center, radius, ref c, ref c);
        }

        /// <summary>
        /// Draws a single colour circle primitive
        /// </summary>
        /// <param name="center">center (origin) point</param>
        /// <param name="radius">radius</param>
        /// <param name="c1">colour (center)</param>
        /// <param name="c2">colour (edge)</param>
        public static void DrawCircle(ref Vector2f center, float radius, ref Colour c1, ref Colour c2)
        {
            DrawCircleInternal(ref center, radius, ref c1, ref c2);
        }

        /// <summary>
        /// Draws a gradient colour circle primitive (internal)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawCircleInternal(ref Vector2f center, float radius, ref Colour c1, ref Colour c2)
        {
            BatchCheck(BatchVertexComponents.Position | BatchVertexComponents.Colour, 37, 108);

            float sliceAngle = -2.96705981f; //Nearly PI
            ushort centerVID = (ushort)batchIndexCount;

            //Add the center vertex, and first edge vertex to the buffer
            fixed (void* buffer = &batchBuffer[batchBufferID].componentBuffer[4 * batchVertexCount])
            {
                //Cast to our structure for easy writing
                VertexPostionColour* vertices = (VertexPostionColour*)buffer;

                //Center Point
                vertices[0].x = center.X;
                vertices[0].y = center.Y;
                vertices[0].rgba = c1.AsInteger;

                //First Edge Point
                vertices[1].x = center.X - radius;
                vertices[1].y = center.Y;
                vertices[1].rgba = c2.AsInteger;
            }
            batchVertexCount += 2;

            //Add Edge Vertices for each sector
            for (int i = 0; i < 35; i++)
            {
                //Add a triangle for each sector
                fixed (ushort* indices = &batchBuffer[batchBufferID].indexBuffer[batchIndexCount])
                {
                    indices[0] = centerVID;
                    indices[1] = (ushort)batchVertexCount;
                    indices[2] = (ushort)(batchVertexCount - 1);
                }
                batchIndexCount += 3;

                //Add a single vertex for each sector
                fixed (void* buffer = &batchBuffer[batchBufferID].componentBuffer[4 * batchVertexCount])
                {
                    //Cast to our structure for easy writing
                    VertexPostionColour* vertices = (VertexPostionColour*)buffer;

                    //Center Point
                    vertices[0].x = center.X + (FastMath.Cos2(sliceAngle) * radius);
                    vertices[0].y = center.Y + (FastMath.Sin2(sliceAngle) * radius);
                    vertices[0].rgba = c2.AsInteger;
                }
                batchVertexCount++;

                sliceAngle += 0.17453293f;
            }

            //We need to add one more triangle to join the sectors together
            fixed (ushort* indices = &batchBuffer[batchBufferID].indexBuffer[batchIndexCount])
            {
                indices[0] = centerVID;
                indices[1] = (ushort)(centerVID + 1);
                indices[2] = (ushort)(batchVertexCount - 1);
            }
            batchIndexCount += 3;
        }
    }
}
