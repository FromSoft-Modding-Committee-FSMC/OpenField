namespace OFC.Hashing
{
    /// <summary>
    /// Interface allows a common implementation of hash functions.
    /// </summary>
    /// <typeparam name="T">Hash return type</typeparam>
    public interface IHashFunction<T>
    {
        /// <summary>
        /// Compares a given buffer of data against a previous hash value.
        /// </summary>
        /// <param name="data">buffer of data</param>
        /// <param name="value">hash value</param>
        /// <returns>true on match</returns>
        bool Compare(ref byte[] data, T value);

        /// <summary>
        /// Calculate hash for a given buffer of data.
        /// </summary>
        /// <param name="data">buffer of data</param>
        /// <returns>The calculated hash</returns>
        T Calculate(ref byte[] data);

        T Calculate(ref char[] data);

        T Calculate(ref string data);
    }
}
