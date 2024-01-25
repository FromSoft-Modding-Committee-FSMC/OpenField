using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.IO
{
    public enum TextureViewEdgeMode
    {
        Clamp,
        Wrap
    }

    public class TextureView<T>
    {
        public T[] array;
        public int width;
        public int height;
        public int pixelSize;
        public TextureViewEdgeMode edgeMode;

        public TextureView(ref T[] array, int width, int height, int pixelSize, TextureViewEdgeMode edgeMode)
        {
            this.array = array;
            this.width = width;
            this.height = height;
            this.pixelSize = pixelSize;
            this.edgeMode = edgeMode;
        }

        public T this[int c, int r, int i]
        {
            get
            {
                switch(edgeMode)
                {
                    case TextureViewEdgeMode.Clamp:
                        c = Math.Clamp(c, 0, height);
                        r = Math.Clamp(r, 0, width);
                        break;
                    case TextureViewEdgeMode.Wrap:
                        while (c < 0)
                            c += height;
                        while (c > height)
                            c -= height;
                        while (r < 0)
                            r += width;
                        while (r > width)
                            r -= width;
                        break;
                }
                return array[((pixelSize * width) * c) + (pixelSize * r) + i];
            }

            set
            {
                switch (edgeMode)
                {
                    case TextureViewEdgeMode.Clamp:
                        c = Math.Clamp(c, 0, height);
                        r = Math.Clamp(r, 0, width);
                        break;
                    case TextureViewEdgeMode.Wrap:
                        while (c < 0)
                            c += height;
                        while (c > height)
                            c -= height;
                        while (r < 0)
                            r += width;
                        while (r > width)
                            r -= width;
                        break;
                }

                array[(pixelSize * width) * c + (pixelSize * r) + i] = value;
            }
        }
    }
}
