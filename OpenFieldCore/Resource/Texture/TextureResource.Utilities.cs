using System;

using OpenTK.Graphics.OpenGL;

using OFC.IO;
using System.ComponentModel;

namespace OFC.Resource.Texture
{
    public partial class TextureResource
    {
        /// <summary>
        /// Flips a buffer of pixels vertically.
        /// </summary>
        /// <param name="buffer">a buffer of pixels</param>
        /// <param name="surfaceHeight">height of the surface stored in the buffer. Must be even!!</param>
        /// <param name="rowSize">size of a row of pixels in the buffer in bytes</param>
        public static void FlipV(ref byte[] buffer, int surfaceHeight, int rowSize)
        {
            if((surfaceHeight & 1) != 0)
                return;

            //Create a row buffer
            byte[] row = new byte[rowSize];
            
            int rowFrom, rowTo;
            for(int i = 0; i < surfaceHeight/2; ++i)
            {
                rowFrom = (rowSize * i);
                rowTo = (rowSize * (surfaceHeight - (i + 1)));

                //Copy a single row from the buffer
                Array.Copy(buffer, rowTo, row, 0, rowSize);

                //Copy a row into the position we just copied from.
                Array.Copy(buffer, rowFrom, buffer, rowTo, rowSize);

                //Copy from the row buffer into the position of the bytes we just moved
                Array.Copy(row, 0, buffer, rowFrom, rowSize);
            }

            /* I kept this because I haven't tested which is faster - 
             * This is certainly more memory intesive, though.
            ** 
            byte[] old = new byte[buffer.Length];
            Array.Copy(buffer, old, buffer.Length);

            for (int i = 0; i < surfaceHeight; ++i)
                Array.Copy(old, (rowSize * i), buffer, (rowSize * (surfaceHeight - (i + 1))), rowSize);
            */
        }

        /// <summary>
        /// Generates a mipmap (0.5x downsample of an image) using a variety of methods.
        /// Only works with RGB/RGBA Images.
        /// </summary>
        /// <param name="surface">Original Image Surface</param>
        /// <param name="surfWidth">Original Width</param>
        /// <param name="surfHeight">Original Height</param>
        /// <param name="pixelSize">Size of a pixel (in bytes)</param>
        /// <param name="mipmap">Output mipmap buffer</param>
        /// <param name="method">Method used to generate the mipmap</param>
        public static void GenerateMipmap(ref byte[] surface, int surfWidth, int surfHeight, int pixelSize, out byte[] mipmap, ETextureMipgenMethod method)
        {
            //Calculate Mipmap sizes - rounded to the next even number
            int mipmW = (int)Math.Round(surfWidth + 0.5, MidpointRounding.ToEven) >> 1;
            int mipmH = (int)Math.Round(surfHeight + 0.5, MidpointRounding.ToEven) >> 1;
            int mipmRowSize = pixelSize * mipmW;
            
            //Calculate Surface sizes.
            int surfRowSize = pixelSize * surfWidth;

            //Console.WriteLine($"SURF SIZE [W = {surfWidth}, H = {surfHeight}, ROWSIZE = {surfRowSize}]");
            //Console.WriteLine($"MIPM SIZE [W = {mipmW}, H = {mipmH}, ROWSIZE = {mipmRowSize}]");

            //Create array of bytes for mipmap
            byte[] mipm = new byte[mipmRowSize * mipmH];

            //View 1D arrays as a 2D arrays
            TextureView<byte> surf2D = new(ref surface, surfWidth, surfHeight, pixelSize, TextureViewEdgeMode.Wrap);
            TextureView<byte> mipm2D = new(ref mipm, mipmW, mipmH, pixelSize, TextureViewEdgeMode.Clamp);

            //Store sample
            byte[] sample = new byte[pixelSize];

            //Start downsampling the original image
            for(int c = 0; c < surfHeight; c += 2)
            {
                for(int r = 0; r < surfWidth; r += 2)
                {
                    //Sample from original image
                    switch(method)
                    {
                        case ETextureMipgenMethod.Box2x2:
                            MipgenSampleBox2x2(surf2D, c, r, pixelSize, ref sample);
                            break;

                        case ETextureMipgenMethod.Rand2x2:
                            MipgenSampleRand2x2(surf2D, c, r, pixelSize, ref sample);
                            break;

                        case ETextureMipgenMethod.Gaussian3x3:
                            MipgenSampleGauss3x3(surf2D, c, r, pixelSize, ref sample);
                            break;
                    }

                    //Write to mipmap
                    for(int i = 0; i < pixelSize; ++i)
                        mipm2D[c >> 1, r >> 1, i] = sample[i];
                }
            }

            mipmap = mipm;
            return;
        }

        public static SizedInternalFormat GetGLSizedFormat(ESurfaceFormat surfaceFormat)
        {
            return surfaceFormat switch
            {
                ESurfaceFormat.DB8G8R8ui =>   SizedInternalFormat.Rgb8,
                ESurfaceFormat.DR8G8B8ui =>   SizedInternalFormat.Rgb8,
                ESurfaceFormat.DB8G8R8A8ui => SizedInternalFormat.Rgba8,
                ESurfaceFormat.DR8G8B8A8ui => SizedInternalFormat.Rgba8,
                _ => throw new InvalidEnumArgumentException("surfaceFormat", (int)surfaceFormat, typeof(ESurfaceFormat))
            };;
        }

        public static PixelFormat GetGLPixelFormat(ESurfaceFormat surfaceFormat)
        {
            //This... is a kinda cool syntax feature Microsoft - Thank you.
            return surfaceFormat switch
            {
                ESurfaceFormat.DB8G8R8ui   => PixelFormat.Bgr,
                ESurfaceFormat.DR8G8B8ui   => PixelFormat.Rgb,
                ESurfaceFormat.DB8G8R8A8ui => PixelFormat.Bgra,
                ESurfaceFormat.DR8G8B8A8ui => PixelFormat.Rgba,
                _ => throw new InvalidEnumArgumentException("surfaceFormat", (int)surfaceFormat, typeof(ESurfaceFormat))
            };
        }

        public static PixelType GetGLPixelType(ESurfaceFormat surfaceFormat)
        {
            return surfaceFormat switch
            {
                ESurfaceFormat.DB8G8R8ui => PixelType.UnsignedByte,
                ESurfaceFormat.DR8G8B8ui => PixelType.UnsignedByte,
                ESurfaceFormat.DB8G8R8A8ui => PixelType.UnsignedByte,
                ESurfaceFormat.DR8G8B8A8ui => PixelType.UnsignedByte,
                _ => throw new InvalidEnumArgumentException("surfaceFormat", (int)surfaceFormat, typeof(ESurfaceFormat))
            };
        }

        private static void MipgenSampleGauss3x3(TextureView<byte> surf, int c, int r, int pixelSize, ref byte[] sample)
        {
            double S;
            for (int i = 0; i < pixelSize; ++i)
            {
                //Left 1 - 3
                S  = (surf[c-1, r-1, i] * 0.0625f);
                S += (surf[c  , r-1, i] * 0.125f);
                S += (surf[c+1, r-1, i] * 0.0625f);

                //Center 1 - 3
                S += (surf[c-1, r, i] * 0.125f);
                S += (surf[c  , r, i] * 0.25f);
                S += (surf[c+1, r, i] * 0.125f);

                //Right 1 - 3
                S += (surf[c-1, r+1, i] * 0.0625f);
                S += (surf[c  , r+1, i] * 0.125f);
                S += (surf[c+1, r+1, i] * 0.0625f);

                sample[i] = (byte)S;
            }
        }

        private static void MipgenSampleBox2x2(TextureView<byte> surf, int c, int r, int pixelSize, ref byte[] sample)
        {
            int S;
            for (int i = 0; i < pixelSize; ++i)
            {
                S  = (surf[c,   r,   i]);
                S += (surf[c,   r+1, i]);
                S += (surf[c+1, r,   i]);
                S += (surf[c + 1, r + 1, i]);

                sample[i] = (byte)(S >> 2);
            }
        }

        private static void MipgenSampleRand2x2(TextureView<byte> surf, int c, int r, int pixelSize, ref byte[] sample)
        {
            //Generate Sample Weights
            double R;
            Random rng = new Random();
            R = rng.NextDouble();
            double AL = Math.Min(R, 1.0 - R);
            double AR = 1.0 - AL;
            R = rng.NextDouble();
            double BT = Math.Min(R, 1.0 - R);
            double BB = 1.0 - BT;

            //Sample
            double S;
            for (int i = 0; i < pixelSize; ++i)
            {
                S =  surf[c,   r,   i] * (AL + BT);
                S += surf[c,   r+1, i] * (AR + BT);
                S += surf[c+1, r,   i] * (AL + BB);
                S += surf[c+1, r+1, i] * (AR + BB);

                sample[i] = (byte)(S * 0.25);
            }
        }
    }
}
