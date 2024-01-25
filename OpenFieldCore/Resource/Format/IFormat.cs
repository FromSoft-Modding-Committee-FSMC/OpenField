namespace OFC.Resource.Format
{
    public interface IFormat<T>
    {
        public SFormatParameters Parameters { get; }
        public bool Load(string filepath, ref T asset, in object parameters);
        public bool Load(byte[] buffer, ref T asset, in object parameters);
        public bool Save(string filepath, T asset);
    }
}
