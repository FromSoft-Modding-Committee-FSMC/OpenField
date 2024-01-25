using OFC.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.World.Graph
{
    public interface IGraphNode
    {
        public EGraphNodeState CurrentState { get; set; }

        public List<IGraphNode> Children { get; }

        public void Draw();

        public void Update();
    }
}
