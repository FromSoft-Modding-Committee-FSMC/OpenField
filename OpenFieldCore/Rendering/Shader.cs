using System;
using System.IO;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using OFC.Utility;
using OFC.Mathematics;

namespace OFC.Rendering
{
    public enum SamplerWrapMode : int
    {
        Clamp   = 33071,
        Repeat  = 10479,
        Mirror  = 33648
    }

    public enum SamplerFilterMode : int
    {
        Nearest = 9728,
        Linear = 9729,
    }

    public class Shader : IDisposable
    {
        private struct Sampler
        {
            public int sampler;
        }

        private readonly int glProgram;

        private Dictionary<string, int> programUniforms;
        private Dictionary<int, Sampler> programSamplers;

        public Shader(string vertexShaderPath, string fragmentShaderPath, string[] uniforms = null, string[] samplers = null)
        {
            //Vertex Shader
            string vsSource;
            try
            {
                vsSource = File.ReadAllText(vertexShaderPath);
            } catch (Exception ex)
            {
                Log.Write("Exception", 0xFF4444, $"Couldn't load vertex shader source: {vertexShaderPath}");
                Log.Write("Stack Trace", 0xCCCCCC, $"\n{ex.StackTrace}");
                return;
            }

            int vsShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vsShader, vsSource);
            GL.CompileShader(vsShader);

            string vsError = GL.GetShaderInfoLog(vsShader);
            if (string.Empty != vsError)
            {
                Log.Error($"Failed to compile vertex shader: {vertexShaderPath}");
                Log.Write("VS Error", 0xCCCCCC, $"\n{vsError}");

                GL.DeleteShader(vsShader);
                return;
            }

            //Fragment Shader
            string fsSource;
            try
            {
                fsSource = File.ReadAllText(fragmentShaderPath);
            }
            catch (Exception ex)
            {
                Log.Write("Exception", 0xFF4444, $"Couldn't load fragment shader source: {fragmentShaderPath}");
                Log.Write("Stack Trace", 0xCCCCCC, $"\n{ex.StackTrace}");
                return;
            }

            int fsShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fsShader, fsSource);
            GL.CompileShader(fsShader);

            string fsError = GL.GetShaderInfoLog(fsShader);
            if (string.Empty != fsError)
            {
                Log.Error($"Failed to compile fragment shader: {fragmentShaderPath}");
                Log.Write("FS Error", 0xCCCCCC, $"\n{fsError}");
                GL.DeleteShader(vsShader);
                GL.DeleteShader(fsShader);
                return;
            }

            //Program
            glProgram = GL.CreateProgram();
            GL.AttachShader(glProgram, vsShader);
            GL.AttachShader(glProgram, fsShader);
            GL.LinkProgram(glProgram);

            GL.DetachShader(glProgram, vsShader);
            GL.DeleteShader(vsShader);

            GL.DetachShader(glProgram, fsShader);
            GL.DeleteShader(fsShader);

            //Find all uniforms and cache them for later use. This improves perf due to less waiting for responses from the GPU
            programUniforms = new Dictionary<string, int>();

            if (uniforms != null)
            {
                foreach (string uniformName in uniforms)
                {
                    int uniformId = GL.GetUniformLocation(glProgram, uniformName);
                    if (uniformId <= -1)
                    {
                        Log.Error($"Couldn't get uniform from shader [vs: {vertexShaderPath}, ps: {vertexShaderPath}, uniform name: {uniformName}]");
                        continue;
                    }

                    programUniforms[uniformName] = uniformId;
                }
            }

            //Find all samplers and create them
            programSamplers = new Dictionary<int, Sampler>();

            if(samplers != null)
            {
                foreach(string samplerName in samplers)
                {
                    int samplerId = GL.GetUniformLocation(glProgram, samplerName);
                    if(samplerId <= -1)
                    {
                        Log.Error($"Couldn't get sampler from shader [vs: {vertexShaderPath}, ps: {vertexShaderPath}, sampler name: {samplerName}]");
                        continue;
                    }

                    int glSampler = GL.GenSampler();

                    programSamplers[samplerId] = new Sampler
                    {
                        sampler = glSampler
                    };

                    programUniforms[samplerName] = samplerId;

                    //Default Configuration for sampler
                    GL.SamplerParameter(glSampler, SamplerParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.SamplerParameter(glSampler, SamplerParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    GL.SamplerParameter(glSampler, SamplerParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
                    GL.SamplerParameter(glSampler, SamplerParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.SamplerParameter(glSampler, SamplerParameterName.TextureMaxAnisotropyExt, 1f);

                    Log.Info($"New Shader Sampler [id = {samplerId}, name = {samplerName}, gl id = {glSampler}]");
                }
            }
        }

        public void Bind()
        {
            GL.UseProgram(glProgram);
        }

        public void SetScalar(string uniformName, float value)
        {
            GL.Uniform1(programUniforms[uniformName], value);
        }
        public void SetScalar(string uniformName, uint value)
        {
            GL.Uniform1(programUniforms[uniformName], value);
        }
        public void SetScalar(string uniformName, int value)
        {
            GL.Uniform1(programUniforms[uniformName], value);
        }
        public void SetScalarArray(string uniformName, int count, ref float[] values)
        {
            GL.Uniform1(programUniforms[uniformName], count, values);
        }
        public void SetScalarArray(string uniformName, int count, ref uint[] values)
        {
            GL.Uniform1(programUniforms[uniformName], count, values);
        }
        public void SetScalarArray(string uniformName, int count, ref int[] values)
        {
            GL.Uniform1(programUniforms[uniformName], count, values);
        }

        /// <summary> Sets a vector2 uniform </summary>
        /// <param name="uniformName">Uniform Name</param>
        /// <param name="vec">A Vector2s (32-bit float components)</param>
        public void SetVector2(string uniformName, ref Vector2s vec)
        {
            GL.Uniform2(programUniforms[uniformName], vec.X, vec.Y);
        }

        /// <summary> Sets a vector2 uniform </summary>
        /// <param name="uniformName">Uniform Name</param>
        /// <param name="vec">A Vector2i (32-bit int components)</param>
        public void SetVector2(string uniformName, ref Vector2i vec)
        {
            GL.Uniform2(programUniforms[uniformName], vec.X, vec.Y);
        }

        /// <summary> Sets a vector3 uniform </summary>
        /// <param name="uniformName">Uniform Name</param>
        /// <param name="vec">A Vector3s (32-bit float components)</param>
        public void SetVector3(string uniformName, ref Vector3s vec)
        {
            GL.Uniform3(programUniforms[uniformName], vec.X, vec.Y, vec.Z);
        }

        /// <summary> Sets a vector4 uniform </summary>
        /// <param name="uniformName">Uniform Name</param>
        /// <param name="vec">A Vector4s (32-bit float components)</param>
        public void SetVector4(string uniformName, ref Vector4s vec)
        {
            GL.Uniform4(programUniforms[uniformName], vec.X, vec.Y, vec.Z, vec.W);
        }
        public void SetVector4(string uniformName, ref Colour colour)
        {
            GL.Uniform4(programUniforms[uniformName], colour.R, colour.G, colour.B, colour.A);
        }

        public void SetMatrix44(string uniformName, ref Matrix44 mat)
        {
            GL.UniformMatrix4(programUniforms[uniformName], 1, false, mat.Components); 
        }

        public void SetSamplerWrapMode(string samplerName, SamplerWrapMode wrapU, SamplerWrapMode wrapV)
        {
            GL.SamplerParameter(programSamplers[programUniforms[samplerName]].sampler, SamplerParameterName.TextureWrapS, (int)wrapU);
            GL.SamplerParameter(programSamplers[programUniforms[samplerName]].sampler, SamplerParameterName.TextureWrapT, (int)wrapV);
        }
        public void SetSamplerMagFilter(string samplerName, SamplerFilterMode filterMode)
        {
            GL.SamplerParameter(programSamplers[programUniforms[samplerName]].sampler, SamplerParameterName.TextureMagFilter, (int)filterMode);
        }
        public void SetSamplerMinFilter(string samplerName, SamplerFilterMode filterMode)
        {
            GL.SamplerParameter(programSamplers[programUniforms[samplerName]].sampler, SamplerParameterName.TextureMinFilter, (int)filterMode);
        }

        public void SetSampler2D(string samplerName, int textureUnit)
        {
            GL.Uniform1(programUniforms[samplerName], textureUnit);
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindSampler(textureUnit, programSamplers[programUniforms[samplerName]].sampler);
        }

        // IDisposable Implementation
        ~Shader()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposeManagedObjects)
        {
            GL.DeleteProgram(glProgram);

            foreach(Sampler sampler in programSamplers.Values)
            {
                GL.DeleteSampler(sampler.sampler);
            }

            if (disposeManagedObjects)
            {
                programUniforms.Clear();
                programUniforms = null;

                programSamplers.Clear();
                programSamplers = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
