using OFC.Numerics;
using OFC.Rendering;
using OFC.Utility;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace OFC.Resource.Shader
{
    public class ShaderResource : IResource
    {
        // CPU Data
        EResourceState state = EResourceState.Unloaded;
        int referenceCount = 0;
        string sourceFilename;
        uint hashcode;

        public string vsSource = string.Empty;
        public string fsSource = string.Empty;
        public string gsSource = string.Empty;
        public string[] cpuUniforms;
        public SShaderSampler[] cpuSamplers;

        // GPU Data
        int gpuProgram;
        public readonly Dictionary<string, int> gpuUniform;
        public readonly Dictionary<string, int> gpuSamplerUniform;
        public readonly Dictionary<int, int>    gpuSampler;

        // IResource Implementation
        public EResourceFlags Flags => EResourceFlags.GPUResource | EResourceFlags.FreeIntermediate;
        public EResourceState State { get => state; set => state = value; }
        public int ReferenceCount { get => referenceCount; set => referenceCount = value; }
        public string Source { get => sourceFilename; set => sourceFilename = value; }
        public uint Hash { get => hashcode; set => hashcode = value; }

        public ShaderResource()
        {
            gpuUniform = new Dictionary<string, int>();
            gpuSamplerUniform = new Dictionary<string, int>();
            gpuSampler = new Dictionary<int, int>();
        }

        public void GPUGenerate()
        {
            if (state != EResourceState.WaitGPUTransfer)
                return;

            string error;

            // Compile Vertex Shader
            int vsObject = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vsObject, string.Join('\n', vsSource));
            GL.CompileShader(vsObject);

            error = GL.GetShaderInfoLog(vsObject);
            if (error != string.Empty)
            {
                Log.Error("Failed to compile vertex shader!");
                Log.Error($"Error: \n{error}");

                GL.DeleteShader(vsObject);
                throw new Exception("Well then");
            }

            // Compile Fragment Shader
            int fsObject = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fsObject, string.Join('\n', fsSource));
            GL.CompileShader(fsObject);

            error = GL.GetShaderInfoLog(fsObject);
            if (error != string.Empty)
            {
                Log.Error("Failed to compile fragment shader!");
                Log.Error($"Error: \n{error}");

                GL.DeleteShader(vsObject);
                GL.DeleteShader(fsObject);
                return;
            }

            // Link Program
            gpuProgram = GL.CreateProgram();
            GL.AttachShader(gpuProgram, vsObject);
            GL.AttachShader(gpuProgram, fsObject);
            GL.LinkProgram(gpuProgram);
            GL.DetachShader(gpuProgram, vsObject);
            GL.DeleteShader(vsObject);
            GL.DetachShader(gpuProgram, fsObject);
            GL.DeleteShader(fsObject);

            // Find and cache shader uniforms
            foreach (string uniformName in cpuUniforms)
            {
                int uniformID = GL.GetUniformLocation(gpuProgram, uniformName);
                if (uniformID <= -1)
                {
                    Log.Warn($"Invalid uniform '{uniformName}'! Could not be found.");
                    continue;
                }

                Console.WriteLine($"New Uniform = {uniformName}");

                gpuUniform[uniformName] = uniformID;
            }

            // Find and cache shader samplers
            foreach (SShaderSampler samplerInfo in cpuSamplers)
            {
                int uniformID = GL.GetUniformLocation(gpuProgram, samplerInfo.name);
                if (uniformID <= -1)
                {
                    Log.Warn($"Invalid sampler '{samplerInfo.name}'! Could not be found.");
                    continue;
                }

                //Initialize GPU Sampler
                GL.CreateSamplers(1, out int samplerID);
                GL.SamplerParameter(samplerID, SamplerParameterName.TextureWrapS, samplerInfo.tileU);
                GL.SamplerParameter(samplerID, SamplerParameterName.TextureWrapT, samplerInfo.tileV);
                GL.SamplerParameter(samplerID, SamplerParameterName.TextureMinFilter, samplerInfo.filterMin);
                GL.SamplerParameter(samplerID, SamplerParameterName.TextureMagFilter, samplerInfo.filterMag);
                GL.SamplerParameter(samplerID, SamplerParameterName.TextureLodBias, samplerInfo.filterBias);

                // I normally disagree with putting idiot fixes in code,
                // but in this case it feels very natural to disable anistropy by using 0f
                // so I'm making a rare exception.
                GL.SamplerParameter(samplerID, SamplerParameterName.TextureMaxAnisotropyExt, Math.Clamp(samplerInfo.filterAnisotropy, 1f, 16f));

                Console.WriteLine($"New Sampler = {samplerInfo.name}");

                //Store Sampler
                gpuSamplerUniform[samplerInfo.name] = uniformID;
                gpuSampler[uniformID] = samplerID;
            }

            state = EResourceState.Ready;
        }

        public void Use()
        {
            GPUGenerate();
            GL.UseProgram(gpuProgram);

            RenderContext.CurrentShader = (uint)gpuProgram;
        }

        public void UniformScalar(string name, float scalar) => GL.Uniform1(gpuUniform[name], scalar);
        public void UniformScalar(int id, float scalar) => GL.Uniform1(id, scalar);

        public void UniformScalar(string name, int scalar) => GL.Uniform1(gpuUniform[name], scalar);
        public void UniformScalarArray(string name, ref float[] scalarArray) => GL.Uniform1(gpuUniform[name], scalarArray.Length, scalarArray);
        public void UniformScalarArray(string name, ref int[] scalarArray) => GL.Uniform1(gpuUniform[name], scalarArray.Length, scalarArray);
        public void UniformScalarArray(string name, float[] scalarArray) => GL.Uniform1(gpuUniform[name], scalarArray.Length, scalarArray);
        public void UniformScalarArray(string name, int[] scalarArray) => GL.Uniform1(gpuUniform[name], scalarArray.Length, scalarArray);

        public void UniformMatrix4f(string name, Matrix4f matrix, bool transpose = false) =>
            GL.UniformMatrix4(gpuUniform[name], 1, transpose, matrix.Components);
        public void UniformMatrix4f(string name, ref Matrix4f matrix, bool transpose = false) =>
            GL.UniformMatrix4(gpuUniform[name], 1, transpose, matrix.Components);

        public void SamplerUnit(string name, int unit)
        {
            int uniformID = gpuSamplerUniform[name];

            GL.Uniform1(uniformID, unit);
            GL.BindSampler(unit, gpuSampler[uniformID]);
        }
    }
}
