using System;

using OFC.IO;
using OFC.Resource.Texture;

namespace OFC.Resource.FileFormat
{
    public partial class TGAFormat
    {
        public static TextureResource LoadFromStream(InputStream ins, ref TextureResource result, in object parameters)
        {
            //Cast Paramteters to STextureLoadParameters
            STextureLoadParameters loadParams = STextureLoadParameters.Default;
            if (parameters != null)
                loadParams = (STextureLoadParameters)parameters;

            result.Reserve(1 + loadParams.mipmapsMax);
            result.SetMode(ETextureMode.Texture2D);

            //Read TGA Header
            TGAHeader header = new TGAHeader
            {
                idLength = ins.ReadU8(),
                colourMapType = ins.ReadU8(),
                imageType = ins.ReadU8()
            };

            //Read TGA Colour Map Specification
            TGAColourMap colourMap = new TGAColourMap
            {
                entryOffset = ins.ReadU16(),
                entryCount = ins.ReadU16(),
                entryBitSize = ins.ReadU8()
            };

            //Read TGA Pixel Map Specification
            TGAPixelMap pixelMap = new TGAPixelMap
            {
                xorigin = ins.ReadU16(),
                yorigin = ins.ReadU16(),
                width = ins.ReadU16(),
                height = ins.ReadU16(),
                bitsPerPixel = ins.ReadU8(),
                flags = ins.ReadU8()
            };

            //Figure out if image data is flipped (from our perspective [top to bottom, left to right order])
            bool isFlippedH = ((pixelMap.flags >> 4) & 1) == 1;
            bool isFlippedV = ((pixelMap.flags >> 5) & 1) != 1;

            //If there is an identifcation area, we skip it.
            if(header.idLength > 0)
                ins.SeekRelative(header.idLength);

            //If there is a colour map, we read it.
            int colourEntryByteSize = 0;
            byte[] colourMapData = null;

            if (colourMap.entryCount > 0)
            {
                colourEntryByteSize = ((colourMap.entryBitSize + 7) >> 3);
                colourMapData = new byte[(colourMap.entryOffset + colourMap.entryCount) * colourEntryByteSize];

                for (int i = colourMap.entryOffset; i < colourMap.entryOffset + colourMap.entryCount; i += colourEntryByteSize)
                {
                    for (int j = 0; j < colourEntryByteSize; ++j)
                        colourMapData[i + j] = ins.ReadU8();
                }

                //Add this palette as a TexturePalette to our resource
            }

            if (pixelMap.width > 0 && pixelMap.height > 0)
            {
                //If there is a pixel map (and there should be), we read it.
                int pixelByteSize = (pixelMap.bitsPerPixel + 7) >> 3;
                byte[] pixelMapData = ins.ReadBytes(((pixelByteSize * pixelMap.width) * pixelMap.height));

                //Flip TGA is Top-To-Bottom bit is not set meaning data is stored Bottom-To-Top
                //OpenField requires textures to be in top to bottom order.
                if (isFlippedV)
                    TextureResource.FlipV(ref pixelMapData, pixelMap.height, pixelByteSize * pixelMap.width);

                //Create the main surface
                Surface mainSurface = new()
                {
                    Flags = ESurfaceFlags.HasMipmaps,
                    Format = tgaColourMode[pixelMap.bitsPerPixel],

                    Width = pixelMap.width,
                    Height = pixelMap.height,

                    Buffer = pixelMapData,

                    MipmapIDs = null,
                    Next = -1,

                    Name = "TGA Main Surface"
                };
                result.LoadSurface(mainSurface);

                //Are we generating mipmaps or not? -
                if(loadParams.mipmapsGenerate)
                {
                    Surface mipSurface;
                    int mipWidth  = pixelMap.width;
                    int mipHeight = pixelMap.height;
                    int mipLevel  = 0;
                    int mipMaxLevel = 0;
                    byte[] lastMip = pixelMapData;
                    byte[] currMip;

                    //How many mipmaps can we have?
                    int mipSize = Math.Min(mipWidth, mipHeight);
                    while (mipSize > 1)
                    {
                        mipMaxLevel++;
                        mipSize >>= 1;
                    }

                    //Clamp maximum mipmaps
                    mipMaxLevel = Math.Min(loadParams.mipmapsMax, mipMaxLevel);

                    int[] mipSurfIDs = new int[mipMaxLevel];

                    while (mipLevel < mipMaxLevel)
                    {
                        //We do a call to the mipmap generator. 
                        TextureResource.GenerateMipmap(ref lastMip, mipWidth, mipHeight, pixelByteSize, out currMip, loadParams.mipmapsMethod);

                        //Half the size...
                        mipWidth >>= 1;
                        mipHeight >>= 1;

                        //Store into the texture resource
                        mipSurface = new Surface
                        {
                            Flags = ESurfaceFlags.None,
                            Format = mainSurface.Format,

                            Width = mipWidth,
                            Height = mipHeight,

                            Buffer = currMip,
                            MipmapIDs = null,
                            Next = -1,

                            Name = $"Generated Mipmap [Method = {loadParams.mipmapsMethod}, Level = {mipLevel + 1} / {mipMaxLevel}]"
                        };
                    
                        mipSurfIDs[mipLevel] = result.LoadSurface(mipSurface);
                        lastMip = currMip;

                        mipLevel++;
                    }

                    //Store result of mip generation in the main texture
                    mainSurface.MipmapIDs = mipSurfIDs;
                }
            }

            return result;
        }
    }
}
