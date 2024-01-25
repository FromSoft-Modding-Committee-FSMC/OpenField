using System;

namespace OFC.Numerics.Random
{
    public class LehmerRandom : IRandom
    {
        private ulong seed;

        public LehmerRandom()
        {
            seed = (ulong)DateTime.Now.Ticks;
        }

        public LehmerRandom(ulong seed)
        {
            this.seed = seed;
        }

        private ulong Next()
        {
            seed = (seed * 48271) % 0xffffffff;
            return seed;
        }

        public float Get(float max)
        {
            return Next() / 4294967295f * max;
        }

        public int Get(int max)
        {
            return (int)(Next() / 4294967295f * max);
        }

        public float GetRange(float min, float max)
        {
            float t = Next() / 4294967295f;
            return (1f - t) * min + t * max;
        }

        public int GetRange(int min, int max)
        {
            float t = Next() / 4294967295f;
            return (int)((1f - t) * min + t * max);
        }
    }
}
