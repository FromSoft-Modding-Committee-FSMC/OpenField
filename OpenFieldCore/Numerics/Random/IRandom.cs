namespace OFC.Numerics.Random
{
    public interface IRandom
    {
        /// <summary>
        /// Returns a random float between min and max
        /// </summary>
        /// <param name="min">The minimum returned value (inclusive)</param>
        /// <param name="max">The maximum returned value (inclusive)</param>
        /// <returns>A random float</returns>
        public float GetRange(float min, float max);

        /// <summary>
        /// Returns a random integer between min and max
        /// </summary>
        /// <param name="min">The minimum returned value (inclusive)</param>
        /// <param name="max">The maximum returned value (inclusive)</param>
        /// <returns>A random integer</returns>
        public int GetRange(int min, int max);

        /// <summary>
        /// Returns a random float between 0 and max
        /// </summary>
        /// <param name="max">The maximum returned value (inclusive)</param>
        /// <returns>A random float</returns>
        public float Get(float max);

        /// <summary>
        /// Returns a random integer between 0 and max
        /// </summary>
        /// <param name="max">The maximum returned value (inclusive)</param>
        /// <returns>A random integer</returns>
        public int Get(int max);
    }
}
