using System;
using System.Collections.Generic;
using System.Text;

namespace OFC.IO.Hashing
{
    public interface IHashFunction
    {
        public byte[] GetHash(ref byte[] buffer);
    }
}



