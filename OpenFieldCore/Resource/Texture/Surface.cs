using System;

namespace OFC.Resource.Texture
{
    /// <summary>
    /// A surface is a 2 dimensional slice of memory for representing a pixel buffer
    /// </summary>
    public class Surface
    {
        //Storage Data
        ESurfaceFlags flags;
        ESurfaceFormat format;
        int width;
        int height;
        int[] mipmapIDs;
        int next;           //ID of the next surface or -1
        byte[] buffer;      //1D Pixel Buffer

        string name;

        //Properties
        public ESurfaceFlags Flags { get => flags; set => flags = value; }
        public ESurfaceFormat Format { get => format; set => format = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public int MipmapCount => mipmapIDs.Length;
        public int[] MipmapIDs { get => mipmapIDs; set => mipmapIDs = value; }
        public int Next { get => next; set => next = value; }
        public byte[] Buffer { get => buffer; set => buffer = value; }

        public string Name { get => name; set => name = value; }
    }
}
