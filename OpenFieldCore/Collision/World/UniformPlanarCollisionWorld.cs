using System;

using OFC.Numerics;

namespace OFC.Collision.World
{
    /// <summary>
    /// A Uniform planar Collision world is composed of evenly distributed cells.
    /// </summary>
    public class UniformPlanarCollisionWorld : IPlanarCollisionWorld
    {
        //Data
        private readonly uint[,] cells;
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
    }
}
