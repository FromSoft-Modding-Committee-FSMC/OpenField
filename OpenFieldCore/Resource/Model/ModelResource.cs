using OpenTK.Graphics.OpenGL;

using OFC.Resource.Material;

namespace OFC.Resource.Model
{
    public class ModelResource : IResource
    {
        // CPU Data - IResource
        public EResourceState State { get; set; }
        public EResourceFlags Flags { get; set; }
        public int ReferenceCount { get; set; }
        public string Source { get; set; }
        public uint Hash { get; set; }

        // CPU Data
        VertexFormat[] vertexFormats;
        int vertexFormatCount;
        IMesh[] meshes;
        int meshCount;
        MaterialResource[] materials;
        int materialCount;

        public int MeshCount => meshCount;

        // GPU Data
        int glVAO = 0;

        /// <summary>
        /// Constructs a new ModelResource
        /// </summary>
        public ModelResource()
        {

        }


        /// <summary>
        /// Constructs a new ModelResource
        /// </summary>
        /// <param name="maxMeshCount">The max number of meshes</param>
        /// <param name="maxVertexFormatCount">The maximum number of vertex formats</param>
        /// <param name="maxMaterialCount">The maximum number of materials</param>
        public ModelResource(int maxMeshCount, int maxVertexFormatCount = 1, int maxMaterialCount = 1)
        {
            Reserve(maxMeshCount, maxVertexFormatCount, maxMaterialCount);
        }


        /// <summary>
        /// Reserves slots for mesh data
        /// </summary>
        /// <param name="maxMeshCount"></param>
        /// <param name="maxVertexFormatCount"></param>
        /// <param name="maxMaterialCount"></param>
        public void Reserve(int maxMeshCount, int maxVertexFormatCount, int maxMaterialCount)
        {
            vertexFormats = new VertexFormat[maxVertexFormatCount];
            vertexFormatCount = 0;

            meshes = new IMesh[maxMeshCount];
            meshCount = 0;

            materials = new MaterialResource[maxMaterialCount];
            materialCount = 0;
        }


        /// <summary>
        /// Loads a mesh object into the model
        /// </summary>
        /// <param name="mesh">The mesh object to load</param>
        /// <returns>The index of the mesh</returns>
        public int LoadMesh(IMesh mesh)
        {
            meshes[meshCount] = mesh;
            return meshCount++;
        }


        /// <summary>
        /// Loads a material object into the model
        /// </summary>
        /// <param name="material">The material object to load</param>
        /// <returns>The index of the material</returns>
        public int LoadMaterial(MaterialResource material)
        {
            materials[materialCount] = material;
            return materialCount++;
        }


        /// <summary>
        /// Loads a vertex format object into the model
        /// </summary>
        /// <param name="format">The vertex format object to load</param>
        /// <returns>The index of the vertex format</returns>
        public int LoadVertexFormat(VertexFormat format)
        {
            vertexFormats[vertexFormatCount] = format;
            return vertexFormatCount++;
        }


        public T GetMesh<T>(int index) where T : IMesh
        {
            return (T)meshes[index];
        }

        public void Draw()
        {
            for(int i = 0; i < meshCount; ++i)
            {
                if (meshes[i] == null)
                    continue;
                meshes[i].Draw();
            } 
        }

        public void Draw(int meshID) => meshes[meshID].Draw();
    }
}
