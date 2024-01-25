using OFC.Resource.Texture;
using System.Collections.Generic;

namespace OFC.Resource.FileFormat
{
    public partial class TGAFormat
    {
        static Dictionary<int, ESurfaceFormat> tgaColourMode = new Dictionary<int, ESurfaceFormat>
        {
            { 8,  ESurfaceFormat.I8          }, //Index 8BPP
            { 24, ESurfaceFormat.DB8G8R8ui   }, //Direct BGR8
            { 32, ESurfaceFormat.DB8G8R8A8ui }  //Direct ABGR8
        };

        struct TGAHeader
        {
            public byte idLength;
            public byte colourMapType;
            public byte imageType;
        }

        struct TGAColourMap
        {
            public ushort entryOffset;
            public ushort entryCount;
            public byte entryBitSize;
        }

        struct TGAPixelMap
        {
            public ushort xorigin;
            public ushort yorigin;
            public ushort width;
            public ushort height;
            public byte bitsPerPixel;
            public byte flags;
        }

        struct TGAFooter
        {
            public uint extAreaOffset;
            public uint devAreaOffset;
            public byte[] signature;        //18 Bytes, 'TRUEVISION-XFILE', '.', '\0'
        }
    }
}
