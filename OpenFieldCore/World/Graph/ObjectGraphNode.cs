using OFC.Numerics;
using OFC.Resource.Model;
using System;
using System.Collections.Generic;

namespace OFC.World.Graph
{
    public class ObjectGraphNode : IGraphNode
    {
        // IGraphNode Implementation
        public EGraphNodeState CurrentState { get; set; }

        public List<IGraphNode> Children { get; }

        // Transform Data
        Vector3f position;
        Vector3f rotation;
        Vector3f scale;

        public Matrix4f Transform { get; }

        // Resource Data
        ModelResource model;

        public ObjectGraphNode(Vector3f position, Vector3f rotation, Vector3f scale, ModelResource model)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;

            Transform = Matrix4f.CreateRotationY(rotation.Y) * Matrix4f.CreateTranslation(position);

            this.model = model;
        }

        public void Draw()
        {


            model?.Draw();

            //Draw my children
            if (Children == null)
                return;

            foreach (IGraphNode child in Children)
                child.Draw();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
