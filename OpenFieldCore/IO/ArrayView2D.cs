using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.IO
{
    public class ArrayView2D<T>
    {
        public T[] array;
        public int rowSize;
        int rowStride;

        public ArrayView2D(ref T[] array, int rowSize) : this(ref array, rowSize, 1)
        { }

        public ArrayView2D(ref T[] array, int rowSize, int rowStride)
        {
            this.array = array;
            this.rowSize = rowSize;
            this.rowStride = rowStride;
        }

        public T this[int c, int r]
        {
            get { return array[(rowSize * c) + (rowStride * r)]; }
            set { array[(rowSize * c) + (rowStride * r)] = value; }
        }
    }
}
