using OFC.Numerics;
using OFC.Numerics.Random;
using System;

namespace OFC.World.Tilemap
{
    public class TilemapLayer3D
    {
        // Data
        STile3D[,] tiles;
        Matrix4f[,] transforms;

        int width;
        int height;
        int elevation = 0;

        public STile3D this[int x, int y] => tiles[y, x];

        public Matrix4f[,] TileTransforms => transforms;

        public int Width => width;
        public int Height => height;

        private TilemapLayer3D() { }

        public TilemapLayer3D(STile3D[,] tiles, int width, int height)
        {
            this.tiles = tiles;
            this.width = width;
            this.height = height;

            //Calculate Transforms
            transforms = new Matrix4f[height, width];

            for(int z = 0; z < height; z++)
            {
                for(int x = 0; x < width; x++)
                {
                    //X, Z of tile already exist... Y Doesn't.
                    float y = Math.Clamp(elevation, -50000, 50000) / 1000f;                     //Layer Elevation -50m to 50m
                          y += Math.Clamp((int)tiles[z, x].elevation, -32000, 32000) / 1000f;   //Tile Elevation -32m to 32m

                    //Build Translation Matrix
                    Matrix4f translation = Matrix4f.CreateTranslation(x, y, z);

                    //Build Rotation Matrix
                    Matrix4f rotationY = Matrix4f.CreateRotationY(-FastMathF.PI + (FastMathF.PI / 2f) * ((tiles[z, x].flags >> 8) & 3));

                    transforms[z, x] = rotationY * translation;
                }
            }
        }

        public static TilemapLayer3D CreateDebug(int w, int h)
        {
            TilemapLayer3D result = new TilemapLayer3D();

            // RNG
            LehmerRandom RNG = new LehmerRandom(12345678);

            // Construct Data Storage
            result.tiles  = new STile3D[w, h];
            result.width  = w;
            result.height = h;

            // Fill Data Storage
            for(int y = 0; y < h; ++y)
            {
                for(int x = 0; x < w; ++x)
                {
                    result.tiles[y, x] = new STile3D
                    {
                        flags  = 0xFFFF,
                        meshID = (ushort)((x + y) % 4),
                        elevation = 0,
                        padding = 0
                    };
                }
            }

            return result;
        }
    }
}
