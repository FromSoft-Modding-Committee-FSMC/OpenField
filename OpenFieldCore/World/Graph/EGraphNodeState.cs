using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.World.Graph
{
    /// <summary>
    /// Enables the controlling of graph node state thorugh a series of flags
    /// </summary>
    [Flags]
    public enum EGraphNodeState : uint
    {
        /// <summary>
        /// Set when the node is enabled
        /// </summary>
        Enabled = (1 << 0),

        /// <summary>
        /// Set when the node contains children
        /// </summary>
        Parent = (1 << 1),
    }
}
