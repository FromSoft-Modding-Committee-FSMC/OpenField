using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Hashing
{
    public class FNV1A : IHashFunction<uint>
    {
        //Static FNV1A Function allows immediate usage. Screw the memory overhead.
        private static FNV1A instance = new();

        public static FNV1A Instance => instance;

        public bool Compare(ref byte[] data, uint value)
        {
            return Calculate(ref data) == value;
        }

        public uint Calculate(ref byte[] data)
        {
            uint fnva = 0x811C9DC5;

            int count = data.Length-1;
            while (count >= 0)
            {
                fnva ^= data[count];
                fnva *= 0x01000193;

                count--;
            }

            return fnva;
        }

        public uint Calculate(ref char[] data)
        {
            uint fnva = 0x811C9DC5;

            int count = data.Length - 1;
            while (count >= 0)
            {
                fnva ^= (byte)data[count];
                fnva *= 0x01000193;

                count--;
            }

            return fnva;
        }

        public uint Calculate(ref string data)
        {
            uint fnva = 0x811C9DC5;

            int count = data.Length - 1;
            while (count >= 0)
            {
                fnva ^= (byte)data[count];
                fnva *= 0x01000193;

                count--;
            }

            return fnva;
        }
    }
}
