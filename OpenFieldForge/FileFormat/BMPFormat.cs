using OFC.Asset.Format;
using OFC.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OFC.IO;
using OFC.Utility;

namespace OFF.FileFormat
{
    public class BMPFormat : IFormat<TextureAsset>
    {
        #region Format Structures
        /// <summary> Valid values for the compression type of a BMP. </summary>
        enum BMPCompressionType : uint
        {
            None = 0,
            RLE8 = 1,
            RLE4 = 2
        }

        /// <summary> Details the format, size and data offset of the bitmap file. </summary>
        struct BMPFileHeader
        {
            /// <summary> Format Tag / Magic Number. Equal to 'BM'. </summary>
            public ushort tag;

            /// <summary> Size of the file in bytes. </summary>
            public uint fileSize;

            /// <summary> Reserved bytes. Equal to '0x00000000'. </summary>
            public byte[] reserved0x06_4;

            /// <summary> Offset to the beginning of the bitmap data. </summary>
            public uint dataOffset;
        }

        /// <summary> Details specifics about the bitmap data such as width, height and type. </summary>
        struct BMPInfoHeader
        {
            /// <summary> Size of the BMPInfoHeader (fucking microsoft, man...) Equal to 0x28. </summary>
            public uint infoSize;

            /// <summary> Horizontal size of the bitmap (in pixels) </summary>
            public uint width;

            /// <summary> Vertical size of the bitmap (in pixels) </summary>
            public uint height;

            /// <summary> The number of planes? (layers?..) Equal to 0x1. </summary>
            public ushort planes;

            /// <summary> The number of bits used for each pixel. </summary>
            public ushort bitsPerPixel;

            /// <summary> The type of compression used </summary>
            public BMPCompressionType compressionType;

            /// <summary> Size of the bitmap data. This can be equal to 0x0 when compression type is equal to BMPCompressionType.None! </summary>
            public uint dataSize;

            /// <summary> Horizontal size of the bitmap (in pixels per meter) </summary>
            public uint widthPerMeter;

            /// <summary> Vertical size of the bitmap (in pixels per meter) </summary>
            public uint heightPerMeter;

            /// <summary> Number of colours inside the CLUT. </summary>
            public uint clutColoursUsed;

            /// <summary> Number of 'important' colours inside the CLUT. 0x0 means 'all'</summary>
            public uint clutImportantColours;
        }

        #endregion

        // Data
        private FormatParameters _parameters = new FormatParameters
        {
            description = "Microsoft Windows [B]it[M]a[P]",
            filter = "Microsoft Windows [B]it[M]a[P];*.bmp",
            extensions = new string[]
            {
                ".bmp"
            },
            allowImport = true,
            allowExport = false,

            validator = BMPFormat.FormatValidator,
            type = FormatType.Texture
        };

        // Properties
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
                BMPFileHeader fileheader = new BMPFileHeader
                {
                    tag = bis.ReadUInt16(),
                    fileSize = bis.ReadUInt32(),
                    reserved0x06_4 = bis.ReadBytes(4),
                    dataOffset = bis.ReadUInt32()
                };

                BMPInfoHeader infoheader = new BMPInfoHeader
                {

                };


            } catch (Exception ex)
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
                fileIsValid &= (bis.ReadUInt16() == 0x4D42);
                bis.Seek(4, SeekOrigin.Current);
                fileIsValid &= (bis.ReadUInt32() == 0x0);
                bis.Seek(4, SeekOrigin.Current);
                fileIsValid &= (bis.ReadUInt32() == 0x28);
            }
            catch (Exception ex)
            {
                Log.Write("Exception", 0xFF4444, ex.Message);
                Log.Write("Stack Trace", 0xCCCCCC, $"\n{ex.StackTrace}");
                return false;
            }

            return fileIsValid;
        }

        // IFormat Interface
        public bool Load(string filepath, out TextureAsset asset)
        {
            TextureAsset result = new TextureAsset();

            using (BinaryInputStream bis = new BinaryInputStream(filepath))
            {
                if (!LoadFromStream(bis, ref result))
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
