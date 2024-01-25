namespace OFC.Resource
{
    public delegate void ResourceLoadCallback();

    public struct SResourceLoadContext
    {
        public ResourceLoadCallback completeCallback;
        public object parameters;
        public string name;
        public string source;
    }
}
