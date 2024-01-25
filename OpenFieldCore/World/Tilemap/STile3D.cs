using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.World.Tilemap
{
    public struct STile3D
    {
        public ushort flags;
        public ushort meshID;
        public short  elevation;
        public ushort padding;
    }
}
