using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Resource.Texture
{
    /// <summary>
    /// ESurfaceFlags are used to say how a surface is stored
    /// </summary>
    [Flags]
    public enum ESurfaceFlags : uint
    {
        /// <summary>
        /// Declares that the surface has mipmaps present
        /// </summary>
        HasMipmaps = (1 << 0),

        /// <summary>
        /// Declares that the surface is a palette
        /// </summary>
        IsPalette = (1 << 1),

        None = 0
    }
}
