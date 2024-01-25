using System;

namespace OFC.Resource
{
    /// <summary>
    /// Flags for resource definition
    /// </summary>
    [Flags]
    public enum EResourceFlags
    {
        /// <summary>
        /// The intermediate form of the resource must be transfered to the GPU before use
        /// </summary>
        GPUResource = (1 << 0),

        /// <summary>
        /// The intermediate form of the resource can be destroyed after GPU Transfer
        /// </summary>
        FreeIntermediate = (1 << 1),

        /// <summary>
        /// The resource is persistant, and won't be unloaded even with a refcount of 0
        /// </summary>
        Persistant = (1 << 2)
    }
}
