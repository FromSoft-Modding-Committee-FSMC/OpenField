using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Resource.Factory
{
    [Flags]
    public enum EFormatFilter : ushort
    {
        Importable = 0x0001,
        Exportable = 0x0002,
        None = 0x0000,
        All = 0xFFFF
    }
}
