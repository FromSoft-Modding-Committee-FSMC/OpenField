using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Resource.Texture
{
    [Flags]
    public enum ETextureMode : uint
    {
        Texture2D = (1 << 0),
        Texture2DArray = (1 << 1),
        Texture3D = (1 << 2),
        TextureCube = (1 << 3)
    }
}
