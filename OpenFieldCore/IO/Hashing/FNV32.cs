using System;

namespace OFC.IO.Hashing
{
    public class FNV32 : IHashFunction
    {
        public byte[] GetHash(ref byte[] buffer)
        {
            uint hash = 0;

            for(int i = 0; i < buffer.Length; ++i)
            {
                hash += (hash << 1) + (hash << 4) + (hash << 7) + (hash << 8) + (hash << 24);
                hash ^= buffer[i];
            }

            return BitConverter.GetBytes(hash);
        }
    }
}
