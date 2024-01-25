using System;
using System.Runtime.CompilerServices;
using OFC.Numerics;

namespace OFC.Rendering
{
    public unsafe static partial class Render2D
    {
        /// <summary>
        /// Draws a single colour line primitive
        /// </summary>
        /// <param name="start">start position</param>
        /// <param name="end">end position</param>
        /// <param name="c">colour</param>
        public static void DrawLinearPath(ref Vector2f start, ref Vector2f end, ref Colour c)
        {
            DrawLinearPathInternal(ref start, ref end, ref c, ref c);
        }

        /// <summary>
        /// Draws a single colour thicc line primitive
        /// </summary>
        /// <param name="start">start position</param>
        /// <param name="end">end position</param>
        /// <param name="thickness">girth</param>
        /// <param name="c">colour</param>
        public static void DrawLinearPath(ref Vector2f start, ref Vector2f end, float thickness, ref Colour c)
        {
            DrawLinearPathInternal(ref start, ref end, thickness, ref c, ref c);
        }

        /// <summary>
        /// Draws a gradient colour line primitive
        /// </summary>
        /// <param name="start">start position</param>
        /// <param name="end">end position</param>
        /// <param name="c1">start colour</param>
        /// <param name="c2">end colour</param>
        public static void DrawLinearPath(ref Vector2f start, ref Vector2f end, ref Colour c1, ref Colour c2)
        {
            DrawLinearPathInternal(ref start, ref end, ref c1, ref c2);
        }

        /// <summary>
        /// Draws a gradient colour line primitive
        /// </summary>
        /// <param name="start">start position</param>
        /// <param name="end">end position</param>
        /// <param name="thickness">girth</param>
        /// <param name="c1">start colour</param>
        /// <param name="c2">end colour</param>
        public static void DrawLinearPath(ref Vector2f start, ref Vector2f end, float thickness, ref Colour c1, ref Colour c2)
        {
            DrawLinearPathInternal(ref start, ref end, thickness, ref c1, ref c2);
        }

        /// <summary>
        /// Draws a gradient colour line primitive (internal)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawLinearPathInternal(ref Vector2f start, ref Vector2f end, ref Colour c1, ref Colour c2)
        {
            BatchCheck(BatchVertexComponents.Position | BatchVertexComponents.Colour, 4, 6);

            //Find perpendiculars
            Vector2f unit = Vector2f.Normalized(start - end) * 0.5f;

            //Write vertices to batch
            fixed (void* buffer = &batchBuffer[batchBufferID].componentBuffer[4 * batchVertexCount])
            {
                //Cast to our structure for easy writing
                VertexPostionColour* vertices = (VertexPostionColour*)buffer;

                vertices[0].x = start.X + unit.Y;
                vertices[0].y = start.Y - unit.X;
                vertices[0].rgba = c1.AsInteger;

                vertices[1].x = start.X - unit.Y;
                vertices[1].y = start.Y + unit.X;
                vertices[1].rgba = c1.AsInteger;

                vertices[2].x = end.X + unit.Y;
                vertices[2].y = end.Y - unit.X;
                vertices[2].rgba = c2.AsInteger;

                vertices[3].x = end.X - unit.Y;
                vertices[3].y = end.Y + unit.X;
                vertices[3].rgba = c2.AsInteger;
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
        /// Draws a gradient colour thicc line primitive (internal)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawLinearPathInternal(ref Vector2f start, ref Vector2f end, float thickness, ref Colour c1, ref Colour c2)
        {
            BatchCheck(BatchVertexComponents.Position | BatchVertexComponents.Colour, 4, 6);

            //Find perpendiculars
            Vector2f unit = Vector2f.Normalized(start - end) * (thickness * 0.5f);

            //Write vertices to batch
            fixed (void* buffer = &batchBuffer[batchBufferID].componentBuffer[4 * batchVertexCount])
            {
                //Cast to our structure for easy writing
                VertexPostionColour* vertices = (VertexPostionColour*)buffer;

                vertices[0].x = start.X + unit.Y;
                vertices[0].y = start.Y - unit.X;
                vertices[0].rgba = c1.AsInteger;

                vertices[1].x = start.X - unit.Y;
                vertices[1].y = start.Y + unit.X;
                vertices[1].rgba = c1.AsInteger;

                vertices[2].x = end.X + unit.Y;
                vertices[2].y = end.Y - unit.X;
                vertices[2].rgba = c2.AsInteger;

                vertices[3].x = end.X - unit.Y;
                vertices[3].y = end.Y + unit.X;
                vertices[3].rgba = c2.AsInteger;
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
