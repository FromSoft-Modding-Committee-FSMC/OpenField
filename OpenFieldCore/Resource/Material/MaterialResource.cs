using OFC.Numerics;
using OFC.Rendering;
using OFC.Resource.Shader;
using OFC.Resource.Texture;
using OFC.Utility;
using OpenTK.Windowing.Common.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OFC.Resource.Material
{
    public class MaterialResource : IResource
    {
        // CPU Properties - IResource
        public EResourceState State { get; set; }
        public EResourceFlags Flags { get; set; }
        public string Source { get; set; }
        public uint Hash { get; set; }
        public int ReferenceCount { get; set; }

        // CPU Properties
        public ShaderResource Shader { get; private set; }

        // CPU Data
        public Dictionary<string, object> parameters = new();

        /// <summary>
        /// Blank constructor for internal use within the resource manager only, where it is expected that all relevant data will be filled in through a file format handler.
        /// </summary>
        internal MaterialResource() 
        { }


        /// <summary>
        /// Full constructor for internal use.
        /// </summary>
        /// <param name="shader">The base shader of the material</param>
        /// <param name="state">The current state of the resource</param>
        /// <param name="flags">Flags controlling how the resource is handled</param>
        /// <param name="source">The source file of this resource</param>
        /// <param name="hash">An identifier or hashcode of the resource</param>
        public MaterialResource(ShaderResource shader, EResourceState state, EResourceFlags flags, string source, uint hash)
        {
            ReferenceCount = 0;
            Shader = shader;
            State = state;
            Flags = flags;
            Source = source;
            Hash = hash;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetParameter(string name, Matrix4f value)
        {
            if (!Shader.gpuUniform.ContainsKey(name))
            {
                Log.Warn($"The shader bound to the material does not support the parameter '{name}'");
                return;
            }
 
            parameters[name] = value;
        }

        public void SetParameter(string name, Vector3f value)
        {
            if (!Shader.gpuUniform.ContainsKey(name))
            {
                Log.Warn($"The shader bound to the material does not support the parameter '{name}'");
                return;
            }

            parameters[name] = value;
        }

        public void SetParameter(string name, TextureResource value)
        {
            if (!Shader.gpuSamplerUniform.ContainsKey(name))
            {
                Log.Warn($"The shader bound to the material does not support the parameter '{name}'");
                return;
            }

            parameters[name] = value;
        }


        /// <summary>
        /// Binds and sets parameters for the material, in preperation of rendering.
        /// </summary>
        /// <returns>True if the render context was changed, false otherwise</returns>
        public void Use()
        {
            // Uniforms & Samplers
            int i = 0;
            foreach (string uniformID in parameters.Keys)
            {
                switch (parameters[uniformID].GetType())
                {
                    case Type F32s when F32s == typeof(float):
                        Shader.UniformScalar(uniformID, (float)parameters[uniformID]);
                        break;

                    case Type M44s when M44s == typeof(Matrix4f):
                        Shader.UniformMatrix4f(uniformID, (Matrix4f)parameters[uniformID]);
                        break;

                    case Type TEXR when TEXR == typeof(TextureResource):
                        Shader.SamplerUnit(uniformID, i);
                        ((TextureResource)parameters[uniformID]).Bind(i);
                        i++;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
