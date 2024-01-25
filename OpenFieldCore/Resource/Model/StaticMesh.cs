using System;
using OFC.Resource.Material;
using OFC.Utility;
using OpenTK.Graphics.OpenGL;

namespace OFC.Resource.Model
{
    public class StaticMesh : IMesh
    {
        //CPU Data
        public VertexFormat Format { get; set; }
        public MaterialResource Material { get; set; }

        byte[] vertexBuffer;
        byte[] indexBuffer;
        EMeshIndexType indexType;
        int indexCount;

        //GPU Data
        int gpuVBO;
        int gpuEBO;
        int gpuVAO = -1;

        public void LoadVertices(ref byte[] vertices)
        {
            vertexBuffer = vertices;
        }

        public void LoadIndices(ref byte[] indices, EMeshIndexType type, int count)
        {
            indexBuffer = indices;
            indexType = type;
            indexCount = count;
        }

        public void Draw()
        {
            if (gpuVAO == -1)
                Transfer();

            GL.BindVertexArray(gpuVAO);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, (DrawElementsType)indexType, 0);
        }

        private void Transfer()
        {
            // Construct VBO
            GL.CreateBuffers(1, out gpuVBO);
            GL.NamedBufferData(gpuVBO, vertexBuffer.Length, vertexBuffer, BufferUsageHint.StaticDraw);

            // Construct EBO
            GL.CreateBuffers(1, out gpuEBO);
            GL.NamedBufferData(gpuEBO, indexBuffer.Length, indexBuffer, BufferUsageHint.StaticDraw);

            // Construct VAO
            GL.CreateVertexArrays(1, out gpuVAO);
            GL.VertexArrayVertexBuffer(gpuVAO, 0, gpuVBO, 0, Format.size);
            GL.VertexArrayElementBuffer(gpuVAO, gpuEBO);

            int componentID = 0;
            foreach(SVertexComponent component in Format.components)
            {
                GL.EnableVertexArrayAttrib(gpuVAO, componentID);
                GL.VertexArrayAttribFormat(gpuVAO, componentID, component.count, (VertexAttribType)component.type, false, component.offset);
                GL.VertexArrayAttribBinding(gpuVAO, componentID, 0);
                componentID++;
            }
        }
    }
}
