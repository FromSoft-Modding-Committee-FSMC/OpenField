namespace OFC.Resource
{
    public interface IResource
    {
        /// <summary>
        /// Name and path of the source file
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Hash code for comparison
        /// </summary>
        public uint Hash { get; }

        /// <summary>
        /// The current load state of the resource
        /// </summary>
        public EResourceState State { get; }

        /// <summary>
        /// Flags to control how the resource operates
        /// </summary>
        public EResourceFlags Flags { get; }

        /// <summary>
        /// The number of times the resource is currently referenced
        /// </summary>
        public int ReferenceCount { get; }
    }
}
