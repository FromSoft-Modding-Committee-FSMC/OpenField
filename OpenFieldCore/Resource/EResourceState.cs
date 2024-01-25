using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Resource
{
    /// <summary>
    /// Flags for resource state
    /// </summary>
    public enum EResourceState : uint
    {
        /// <summary>
        /// The resource is defined, but currently not loaded.
        /// </summary>
        Unloaded     = 0,

        /// <summary>
        /// The resource is being loaded from file to ram
        /// </summary>
        LoadingIntermediate = 1,

        /// <summary>
        /// The resource is waiting to be transferred to the GPU
        /// </summary>
        WaitGPUTransfer = 3,

        /// <summary>
        /// The resource is currently being transfered to the GPU
        /// </summary>
        InGPUTransfer = 4,

        /// <summary>
        /// The resource is ready to be used
        /// </summary>
        Ready        = 0xFFFFFFFE,

        /// <summary>
        /// The resource failed to load
        /// </summary>
        Failed       = 0xFFFFFFFF
    }
}
