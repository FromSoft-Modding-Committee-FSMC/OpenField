using System;

using OFC.Asset;
using OFC.Asset.Format;
using OFC.Utility;
using OFC.IO;

namespace OFF.FileFormat
{
    public class FileFormatTIM : IFormat<TextureAsset>
    {
        #region Format Structures
        struct TIMFormatHeader
        {
            /// <summary> Format tag (or magic number), equal to 0x10 for authentic PSX tim files. </summary>
            public uint tag;

            /// <summary> 
            /// Flags describing how the following data is to be interpreted.
            /// <br>Bits 0-2 = The BPP... Where 0 = 4BPP (Indexed), 1 = 8BPP (Indexed), 2 = 16BPP (High Colour), 3 = 24BPP (True Colour) and 4 = Mixed Mode </br>
            /// <br>Bits 3-3 = Clut Buffer Present. Set when palette/clut data is present in the TIM file. Normally exclusive with indexed BPPs.</br>
            /// </summary>
            public uint flags;

            /// <summary>A property (not actual data) which returns the 3rd bit of 'flags', aka - [C]lut [B]uffer [P]resent</summary>
            public bool CBP
            {
                get
                {
                    return ((flags >> 3) & 0x1) == 1;
                }
            }

            /// <summary>A property (not actual data) which returns the first 3 bits of 'flags', the colour mode / BPP</summary>
            public uint Mode
            {
                get
                {
                    return (flags & 0x7);
                }
            }
        }
        struct TIMBufferHeader
        {
            /// <summary> Size of the buffer, plus the 12 bytes of this header. </summary>
            public uint bufferLength;

            /// <summary> Destination X coordinate of the buffer in PSX vram. </summary>
            public ushort destinationX;

            /// <summary> Destination Y coordinate of the buffer in PSX vram. </summary>
            public ushort destinationY;

            /// <summary> Destination horizontal size of the buffer in PSX vram. </summary>
            public ushort destinationW;

            /// <summary> Destination vertical size of the buffer in PSX vram. </summary>
            public ushort destinationH;
        }

        #endregion

        //Data
        private FormatParameters _parameters = new FormatParameters
        {
            description = "Sony PlayStation [T]exture/[IM]age",
            filter = "Sony PlayStation [T]exture/[IM]age;*.tim",
            extensions = new string[]
            {
                ".tim"
            },
            allowImport = true,
            allowExport = false,

            validator = FileFormatTIM.FormatValidator,
            type = FormatType.Texture
        };

        //Properties
        public FormatParameters Parameters
        {
            get 
            { 
                return _parameters; 
            }
        }

        private static bool LoadFromStream(BinaryInputStream bis, ref TextureAsset asset)
        {
            try
            {
                TIMFormatHeader header = new TIMFormatHeader
                {
                    tag = bis.ReadUInt32(),
                    flags = bis.ReadUInt32()
                };

                if (header.tag != 0x10)
                {
                    throw new Exception($"Invalid TIM -> (tag: {header.tag:X8} != 0x10000000)");
                }

                if (header.Mode == 4)
                {
                    throw new Exception("Invalid TIM -> Mixed Mode is not supported");
                }

                if (header.CBP)
                {
                    TIMBufferHeader clutBuffer = new TIMBufferHeader
                    {
                        bufferLength = bis.ReadUInt32(),
                        destinationX = bis.ReadUInt16(),
                        destinationY = bis.ReadUInt16(),
                        destinationW = bis.ReadUInt16(),
                        destinationH = bis.ReadUInt16()
                    };

                    TexturePalette texturePalette;

                    if (header.Mode == 0 || header.Mode == 1)
                    {
                        texturePalette = new TexturePalette
                        {
                            name = "n/a",
                            mode = ColourMode.D16,
                            colourCount = (int)(clutBuffer.bufferLength - 12) / 2,
                            bufferLength = (int)(clutBuffer.bufferLength - 12)
                        };
                        texturePalette.buffer = bis.ReadBytes(texturePalette.bufferLength);
                    }
                    else
                    {
                        throw new Exception("Invalid TIM -> Image is not 4bpp/8bpp, yet contains a CLUT");
                    }

                    asset.AddPalette(ref texturePalette);
                }
                else
                {
                    if (header.Mode == 0 || header.Mode == 1)
                    {
                        throw new Exception("Invalid TIM -> Image is 4bpp/8bpp, but doesn't contain a CLUT");
                    }
                }

                TIMBufferHeader imageBuffer = new TIMBufferHeader
                {
                    bufferLength = bis.ReadUInt32(),
                    destinationX = bis.ReadUInt16(),
                    destinationY = bis.ReadUInt16(),
                    destinationW = bis.ReadUInt16(),
                    destinationH = bis.ReadUInt16()
                };


            }
            catch (Exception ex)
            {
                Log.Write("Exception", 0xFF4444, ex.Message);
                Log.Write("Stack Trace", 0xCCCCCC, $"\n{ex.StackTrace}");
                return false;
            }

            return true;
        }

        private static bool FormatValidator(byte[] buffer)
        {
            bool fileIsValid = true;

            try
            {
                using BinaryInputStream bis = new BinaryInputStream(buffer);
                fileIsValid &= (bis.ReadUInt32() == 0x10);

            }
            catch (Exception ex)
            {
                Log.Write("Exception", 0xFF4444, ex.Message);
                Log.Write("Stack Trace", 0xCCCCCC, $"\n{ex.StackTrace}");
                return false;
            }

            return fileIsValid;
        }

        //IFormat Interface
        public bool Load(string filepath, out TextureAsset asset)
        {
            TextureAsset result = new TextureAsset();

            using(BinaryInputStream bis = new BinaryInputStream(filepath))
            {
                if(!LoadFromStream(bis, ref result))
                {
                    Log.Warn($"Failed to load file [desc: {_parameters.description}, path: {filepath}]");
                    asset = null;
                    return false;
                }
            }

            asset = result;
            return true;
        }

        public bool Load(byte[] buffer, out TextureAsset asset)
        {
            TextureAsset result = new TextureAsset();

            using (BinaryInputStream bis = new BinaryInputStream(buffer))
            {
                if (!LoadFromStream(bis, ref result))
                {
                    Log.Warn($"Failed to load file {_parameters.description}");
                    asset = null;
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
