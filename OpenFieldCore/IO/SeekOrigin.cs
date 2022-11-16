using System;
using System.Collections.Generic;
using System.Text;

namespace OFC.IO
{
    public enum SeekOrigin
    {
        /// <summary>The start of a binary stream </summary>
        Begin = 0,

        /// <summary>The current position of a binary stream </summary>
        Current = 1,

        /// <summary> The end position of a binary stream </summary>
        End = 2
    }
}
