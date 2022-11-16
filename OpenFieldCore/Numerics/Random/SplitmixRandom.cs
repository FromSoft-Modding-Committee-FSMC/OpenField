using System;

namespace OFC.Numerics.Random
{
    public class SplitMixRandom : IRandom
    {
        private ulong seed;

        public SplitMixRandom()
        {
            seed = (ulong)DateTime.Now.Ticks;
        }

        public SplitMixRandom(ulong seed)
        {
            this.seed = seed;
        }

        private ulong Next()
        {
            ulong z = seed += 0x9e3779b97f4a7c15;
            z = (z ^ z >> 30) * 0xbf58476d1ce4e5b9;
            z = (z ^ z >> 27) * 0x94d049bb133111eb;
            return z ^ z >> 31;
        }

        public float Get(float max)
        {
            return Next() / 18446744073709551615f * max;
        }

        public int Get(int max)
        {
            return (int)(Next() / 18446744073709551615f * max);
        }

        public float GetRange(float min, float max)
        {
            float t = Next() / 18446744073709551615f;
            return (1f - t) * min + t * max;
        }

        public int GetRange(int min, int max)
        {
            float t = Next() / 18446744073709551615f;
            return (int)((1f - t) * min + t * max);
        }
    }
}