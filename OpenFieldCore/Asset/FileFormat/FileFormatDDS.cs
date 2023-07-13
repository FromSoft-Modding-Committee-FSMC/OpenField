using System;
using System.Runtime.CompilerServices;

using OFC.Utility;
using OFC.IO;

using OFC.Asset.Format;
using System.Net;

namespace OFC.Asset.FileFormat
{
    public class FileFormatDDS : IFormat<TextureAsset>
    {
        #region Format Structures
        [Flags]
        enum DDSSurfaceFlags : uint
        {
            Caps        = 0x00000001,
            Height      = 0x00000002,
            Width       = 0x00000004,
            Pitch       = 0x00000008,
            PixelFormat = 0x00001000,
            MipMapCount = 0x00020000,
            LinearSize  = 0x00080000,
            Depth       = 0x00800000
        }

        [Flags]
        enum DDSFormatFlags : uint
        {
            AlphaPixels = 0x00000001,
            Alpha       = 0x00000002,
            FourCC      = 0x00000004,
            RGB         = 0x00000040,
            YUV         = 0x00000200,
            Luminance   = 0x00020000,
        }

        [Flags]
        enum DDSFormatCaps : uint
        {
            //Caps 1
            Complex = 0x00000008,
            Texture = 0x00001000,
            Mipmap  = 0x00400000,

            //Caps 2
            Cubemap          = 0x00000200,
            CubemapPositiveX = 0x00000600,
            CubemapNegativeX = 0x00000a00,
            CubemapPositiveY = 0x00001200,
            CubemapNegativeY = 0x00002200,
            CubemapPositiveZ = 0x00004200,
            CubemapNegativeZ = 0x00008200,
            Volume           = 0x00200000,
        }

        enum DDSDataType : uint
        {
            RGBA_U8   = 0x00000000,

            RGBA_F32  = 0x00000074,

            COMP_DXT1 = 0x31545844,

            COMP_DXT2 = 0x32545844,

            COMP_DXT3 = 0x33545844,

            COMP_DXT4 = 0x34545844,

            COMP_DXT5 = 0x35545844,

            TYPE_DX10 = 0x30315844
        }

        struct DDSHeader
        {
            /// <summary> Format tag (or magic number), equal to 'DDS '</summary>
            public uint tag;
        }

        struct DDSSurfaceHeader
        {
            /// <summary> Length of the surface header structure, including the 4 bytes of this value. </summary>
            public uint length;

            /// <summary> Surface flags, at the least 'Caps', 'Height', 'Width', 'PixelFormat' always required.</summary>
            public DDSSurfaceFlags flags;

            /// <summary> Height of the surface </summary>
            public uint height;

            /// <summary> Width of the surface </summary>
            public uint width;

            /// <summary> Pitch of the surface... Or, byte size of one row of pixels. </summary>
            public uint pitch;

            /// <summary> Depth of the surface (for volume textures. Can be 0 or 1 for 2D textures)</summary>
            public uint depth;

            /// <summary> The number of mip maps contained for the surface</summary>
            public uint mipmapCount;

            /// <summary> 44 bytes of reserved data. NVTT (Nvidia Texture Tools) puts the program name here</summary>
            public byte[] reserved0x1C;
        }

        struct DDSPixelFormat
        {
            /// <summary> Length of the pixel format structure, including the 4 bytes of this value. </summary>
            public uint length;

            /// <summary> Pixel Format Flags </summary>
            public DDSFormatFlags flags;

            /// <summary> Pixel format data type, or 'FourCC'. </summary>
            public DDSDataType dataType;

            /// <summary> Size of a whole pixel </summary>
            public uint bitCount;

            /// <summary> Bit mask of the pixel R component </summary>
            public uint bitMaskR;

            /// <summary> Bit mask of the pixel G component </summary>
            public uint bitMaskG;

            /// <summary> Bit mask of the pixel B component </summary>
            public uint bitMaskB;

            /// <summary> Bit mask of the pixel A component </summary>
            public uint bitMaskA;
        }

        struct DDSCaps
        {
            /// <summary> Capability flags of the image</summary>
            public uint caps;
            public uint capsEx;
            public uint capsReserved0x8;
            public uint capsReserved0xC;
            public uint reserved0x10;
        }

        struct DDSDX10
        {
            public uint dxgiFormat;
            public uint dimension;
            public uint flags0x08;
            public uint arraySize;
            public uint flags0x10;
        }

        #endregion

        //Data
        private FormatParameters _parameters = new FormatParameters
        {
            name = "DDS",
            description = "[D]irect [D]raw [S]urface",
            filter = "[D]irect [D]raw [S]urface;*.dds",
            extensions = new string[]
            {
                ".dds",
            },
            allowImport = true,
            allowExport = true,

            validator = FileFormatDDS.FormatValidator,
            type = FormatType.Texture
        };

        //Properties
        public FormatParameters Parameters
        {
            get { return _parameters; }
        }

        private static bool FormatValidator(byte[] buffer)
        {
            bool fileIsValid = true;

            try
            {
                using BinaryInputStream bis = new BinaryInputStream(buffer);

                fileIsValid &= (bis.ReadUInt32() == 0x20534444);    //tag == 'DDS '
                fileIsValid &= (bis.ReadUInt32() == 0x0000007C);    //headerLength = 124
            }
            catch (Exception ex)
            {
                Log.Write("Exception", 0xFF4444, ex.Message);
                Log.Write("Stack Trace", 0xCCCCCC, $"\n{ex.StackTrace}");
                return false;
            }

            return fileIsValid;
        }
        private static bool LoadFromStream(BinaryInputStream bis, ref TextureAsset asset)
        {
            try
            {
                //DDS Header
                DDSHeader header = new DDSHeader
                {
                    tag = bis.ReadUInt32()
                };

                //DDS Surface Header
                DDSSurfaceHeader surfaceHeader = new DDSSurfaceHeader
                {
                    length = bis.ReadUInt32(),
                    flags = (DDSSurfaceFlags)bis.ReadUInt32(),
                    height = bis.ReadUInt32(),
                    width = bis.ReadUInt32(),
                    pitch = bis.ReadUInt32(),
                    depth = bis.ReadUInt32(),
                    mipmapCount = bis.ReadUInt32(),
                    reserved0x1C = bis.ReadBytes(44)
                };

                //DDS Pixel Format
                DDSPixelFormat pixelFormat = new DDSPixelFormat
                {
                    length = bis.ReadUInt32(),
                    flags = (DDSFormatFlags)bis.ReadUInt32(),
                    dataType = (DDSDataType)bis.ReadUInt32(),
                    bitCount = bis.ReadUInt32(),
                    bitMaskR = bis.ReadUInt32(),
                    bitMaskG = bis.ReadUInt32(),
                    bitMaskB = bis.ReadUInt32(),
                    bitMaskA = bis.ReadUInt32()
                };

                //DDS Caps
                DDSCaps caps = new DDSCaps
                {
                    caps = bis.ReadUInt32(),
                    capsEx = bis.ReadUInt32(),
                    capsReserved0x8 = bis.ReadUInt32(),
                    capsReserved0xC = bis.ReadUInt32(),
                    reserved0x10 = bis.ReadUInt32()
                };

                //DDS DX10 Extension
                DDSDX10 dx10Header;
                if(pixelFormat.dataType == DDSDataType.TYPE_DX10)
                {
                    dx10Header = new DDSDX10
                    {
                        dxgiFormat = bis.ReadUInt32(),
                        dimension = bis.ReadUInt32(),
                        flags0x08 = bis.ReadUInt32(),
                        arraySize = bis.ReadUInt32(),
                        flags0x10 = bis.ReadUInt32()
                    };

                    throw new Exception("DX10 DDS Files are not supported.");
                }

                //Load DDS data
                TextureSubimage ddsMainSurface = new TextureSubimage
                {
                    name = "DDS Main Surface",
                    width = surfaceHeader.width,
                    height = surfaceHeader.height,
                    paletteIDs = null,
                };

                switch(pixelFormat.dataType)
                {
                    case DDSDataType.RGBA_U8:
                        ReadSurfaceU32(bis, ref ddsMainSurface);
                        break;

                    case DDSDataType.RGBA_F32:
                        ReadSurfaceF128(bis, ref ddsMainSurface);
                        break;

                    default:
                        throw new Exception("Unsupported DDS pixel format.");
                }

                asset.AddSubImage(ref ddsMainSurface);

                //Ignoring mipmaps, but they're here.

            }
            catch (Exception ex)
            {
                Log.Write("Exception", 0xFF4444, ex.Message);
                Log.Write("Stack Trace", 0xCCCCCC, $"\n{ex.StackTrace}");
                return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadSurfaceF128(BinaryInputStream bis, ref TextureSubimage subimage)
        {
            subimage.bufferLength = (int)((16 * subimage.width) * subimage.height);
            subimage.mode = ColourMode.F128;
            subimage.buffer = bis.ReadBytes(subimage.bufferLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadSurfaceU32(BinaryInputStream bis, ref TextureSubimage subimage)
        {
            subimage.bufferLength = (int)((4 * subimage.width) * subimage.height);
            subimage.mode = ColourMode.D32;
            subimage.buffer = bis.ReadBytes(subimage.bufferLength);
        }

        //IFormat Interface
        public bool Load(string filepath, out TextureAsset asset)
        {
            asset = null;

            if(!System.IO.File.Exists(filepath))
            {
                Log.Warn($"Cannot find DDS file [path: {filepath}]");
                return false;
            }

            TextureAsset result = new TextureAsset();

            using (BinaryInputStream bis = new BinaryInputStream(filepath))
            {
                if (!LoadFromStream(bis, ref result))
                {
                    Log.Warn($"Failed to load file [desc: {_parameters.description}, path: {filepath}]");
                    return false;
                }
            }

            asset = result;
            return true;
        }

        public bool Load(byte[] buffer, out TextureAsset asset)
        {
            asset = null;

            TextureAsset result = new TextureAsset();

            using (BinaryInputStream bis = new BinaryInputStream(buffer))
            {
                if (!LoadFromStream(bis, ref result))
                {
                    Log.Warn($"Failed to load file [desc: {_parameters.description}, path: n/a]");
                    return false;
                }
            }

            asset = result;
            return true;
        }

        public void Save(string filepath, TextureAsset asset)
        {
            throw new NotImplementedException();
        }
    }
}
