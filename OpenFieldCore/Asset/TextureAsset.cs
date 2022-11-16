using System.Collections.Generic;

namespace OFC.Asset
{
    public enum ColourMode
    {
        /// <summary> Unknown colour mode. </summary>
        Unknown = 0,

        /// <summary> 4 bits per pixel, mapped. </summary>
        M4 = 1,

        /// <summary> 8 bits per pixel, mapped. </summary>
        M8 = 2,

        /// <summary> 16 bits per pixel, direct. 5 Bits for RGB, 1 bit for alpha mask.</summary>
        D16 = 3,

        /// <summary> 24 bits per pixel, direct. 8 bits for RGB. </summary>
        D24 = 4,

        /// <summary> 32 bits per pixel, direct. 8 bits for RGBA. </summary>
        D32 = 5,

        /// <summary> 128 bits per pixel, direct. 32 bits for each RGBA component. </summary>
        F128 = 6
    }

    public struct TextureSubimage
    {
        public string name;
        public uint width;
        public uint height;

        public ColourMode mode;
        public uint[] paletteIDs;

        public int bufferLength;
        public byte[] buffer;

        public static TextureSubimage Default
        {
            get
            {
                return new TextureSubimage()
                {
                    name = "Empty Subimage",
                    width = 0,
                    height = 0,
                    mode = ColourMode.Unknown,
                    paletteIDs = null,
                    bufferLength = 0,
                    buffer = null
                };
            }
        }
    }

    public struct TexturePalette
    {
        public string name;
        public int colourCount;

        public ColourMode mode;

        public int bufferLength;
        public byte[] buffer;

        public static TexturePalette Default
        {
            get
            {
                return new TexturePalette
                {
                    name = "Empty Palette",
                    colourCount = 0,
                    mode = ColourMode.Unknown,
                    bufferLength = 0,
                    buffer = null
                };
            }
        }
    }

    public class TextureAsset
    {
        //Data
        private readonly List<TextureSubimage> subimages;
        private readonly List<TexturePalette> palettes;

        //Properties
        public int SubImageCount
        {
            get
            {
                return subimages.Count;
            }
        }
        public List<TextureSubimage> Subimages
        {
            get { return subimages; }
        }

        public int PaletteCount
        {
            get
            {
                return palettes.Count;
            }
        }
        public List<TexturePalette> Palettes
        {
            get { return palettes; }
        }

        public TextureAsset()
        {
            subimages = new List<TextureSubimage>();
            palettes = new List<TexturePalette>();
        }

        public int AddSubImage(ref TextureSubimage subImage)
        {
            subimages.Add(subImage);
            return subimages.Count;
        }

        public int AddPalette(ref TexturePalette palette)
        {
            palettes.Add(palette);
            return palettes.Count;
        }

        public bool GetSubImage(int index, out TextureSubimage subimage)
        {
            if(index < 0 || index >= subimages.Count)
            {
                subimage = TextureSubimage.Default;
                return false;
            }

            subimage = subimages[index];
            return true;
        }

        public bool GetPalette(int index, out TexturePalette palette)
        {
            if (index < 0 || index >= palettes.Count)
            {
                palette = TexturePalette.Default;
                return false;
            }

            palette = palettes[index];
            return true;
        }
    }
}
