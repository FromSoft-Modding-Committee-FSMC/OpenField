using System;

namespace OFC.IO.Hashing
{
    public class FromSoftwareCRC : IHashFunction
    {
        public byte[] GetHash(ref byte[] buffer)
        {
            uint hash = 0x12345678;

            for(int i = 0; i < (buffer.Length & 0xFFFFFFFC); i += 4)
            {
                hash += (uint) ((buffer[i + 0] << 24) | (buffer[i + 1] << 16) | (buffer[i + 2] << 8) | (buffer[i + 3]));
            }

            return BitConverter.GetBytes(hash);
        }
    }
}
