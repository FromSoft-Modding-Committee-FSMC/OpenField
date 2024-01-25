using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Resource.Model
{
    /// <summary>
    /// Used to declare the type & size of a single index within a mesh index buffer.
    /// </summary>
    public enum EMeshIndexType : ushort
    {
        /// <summary>
        /// Unsigned Integer Index (8 bits)
        /// </summary>
        UInt8 = 0x1401,

        /// <summary>
        /// Unsigned Integer Index (16 bits)
        /// </summary>
        UInt16 = 0x1403,

        /// <summary>
        /// Unsigned Integer Index (32 bits)
        /// </summary>
        UInt32 = 0x1405
    }
}
