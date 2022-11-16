using System;

using OFC.Mathematics;

namespace OFC.Collision.World
{
    /// <summary>
    /// A Uniform planar Collision world is composed of evenly distributed cells.
    /// </summary>
    public class UniformPlanarCollisionWorld : IPlanarCollisionWorld
    {
        //Data
        private uint[,] cells;
        private readonly uint cellCountX;
        private readonly uint cellCountY;
        private readonly uint cellSizeX;
        private readonly uint cellSizeY;

        public UniformPlanarCollisionWorld(uint worldSizeX, uint worldSizeY, uint cellSizeXY)
        {
            cellCountX = worldSizeX / cellSizeXY;
            cellCountY = worldSizeY / cellSizeXY;
            cellSizeX = cellSizeXY;
            cellSizeY = cellSizeXY;

            cells = new uint[cellCountX, cellCountY];
        }

        public bool LineTrace(Vector2s start, Vector2s direction, float maxDistance, uint mask, out PlanarTraceResult result)
        {
            Vector2s cellStepSize = new Vector2s(
                MathF.Sqrt(1 + (direction.Y / direction.X) * (direction.Y / direction.X)),
                MathF.Sqrt(1 + (direction.X / direction.Y) * (direction.X / direction.Y))
            );
            Vector2s tracePosition = Vector2s.Divide(start, new Vector2s(cellSizeX, cellSizeY));
            Vector2s traceDirection = Vector2s.Zero;
            Vector2s traceLength = Vector2s.Zero;
            Vector2s traceLastPosition = Vector2s.Zero;

            if (traceDirection.X < 0)
            {
                traceDirection.X = -1;
                traceLength.X = (start.X - (tracePosition.X)) * cellStepSize.X;
            }
            else
            {
                traceDirection.X = 1;
                traceLength.X = ((tracePosition.X + 1) - start.X) * cellStepSize.X;
            }

            if(traceDirection.Y < 0)
            {
                traceDirection.Y = -1;
                traceLength.Y = (start.Y - (tracePosition.Y)) * cellStepSize.Y;
            }
            else
            {
                traceDirection.Y = 1;
                traceLength.Y = ((tracePosition.Y + 1) - start.Y) * cellStepSize.Y;
            }

            float traceDistance = 0f;
            
            while (traceDistance < maxDistance)
            {
                if (traceLength.X < traceLength.Y)
                {
                    traceLastPosition.X = tracePosition.X;
                    tracePosition.X += traceDirection.X;
                    traceDistance = traceLength.X;
                    traceLength.X += cellStepSize.X;
                }
                else
                {
                    traceLastPosition.Y = tracePosition.Y;
                    tracePosition.Y += traceDirection.Y;
                    traceDistance = traceLength.Y;
                    traceLength.Y += cellStepSize.Y;
                }

                if(tracePosition.X >= 0 && tracePosition.X < cellCountX && tracePosition.Y >= 0 && tracePosition.Y < cellCountY)
                {
                    //Check for colliders in cell
                    //Perfrom the complex line trace against all children of the cell.
                }
            }

            result = new PlanarTraceResult
            {
                intersectionDistance = 0,
                intersectionPoint = Vector2s.Zero
            };
            return false;
        }
    }
}
