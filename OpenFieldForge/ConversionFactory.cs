using System;
using System.Collections.Generic;
using System.Text;

using OFC.Asset.Format;
using OFC.Asset;

using OFF.FileFormat;

namespace OFF
{
    public static class ConversionFactory
    {
        public static List<IFormat<TextureAsset>> textureFormats;

        public static void Initialize()
        {
            //File System Formats

            //Texture Formats
            textureFormats = new List<IFormat<TextureAsset>>();
            textureFormats.Add(new FileFormatTIM());

        }


    }
}
