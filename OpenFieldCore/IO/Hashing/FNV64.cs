using System;

namespace OFC.IO.Hashing
{
    public class FNV64 : IHashFunction
    {
        public byte[] GetHash(ref byte[] buffer)
        {
            ulong hash = 0;

            for(int i = 0; i < buffer.Length; ++i)
            {
                hash += (hash << 1) + (hash << 4) + (hash << 5) + (hash << 7) + (hash << 8) + (hash << 40);
                hash ^= buffer[i];
            }

            return BitConverter.GetBytes(hash);
        }
    }
}
