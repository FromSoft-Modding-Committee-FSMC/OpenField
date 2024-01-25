using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System;

using OFC.Resource.Shader;
using OFC.Utility;
using OFC.Resource.Model;
using OFC.Resource.FileFormat;
using OpenTK.Compute.OpenCL;
using OFC.Resource.Texture;

namespace OFC.Resource.Factory
{
    class ShaderFactory : FormatFactory<ShaderResource>
    {
        // Data
        ConcurrentDictionary<string, ShaderResource> cache;

        // Indexer
        public ShaderResource this[string name]
        {
            get { return cache[name]; }
            set { cache[name] = value; }
        }

        // Constructor
        public ShaderFactory()
        { 
            cache = new ConcurrentDictionary<string, ShaderResource>();
        }

        public void LoadExplicit(SResourceLoadContext context)
        {
            ShaderResource resource = cache[context.name];

            if (!File.Exists(context.source))
            {
                Log.Error($"Couldn't find file '{context.source}'!");
                goto LoadFailed;
            }

            string buffer = File.ReadAllText(context.source);

            SJFXFile jfx = JsonSerializer.Deserialize<SJFXFile>(buffer, jsonSerializerOptions);

            if (jfx.vertexSource != null)
                resource.vsSource = string.Join('\n', jfx.vertexSource);

            if (jfx.fragmentSource != null)
                resource.fsSource = string.Join('\n', jfx.fragmentSource);

            if (jfx.geometrySource != null)
                resource.gsSource = string.Join('\n', jfx.geometrySource);

            if (jfx.uniforms != null)
                resource.cpuUniforms = jfx.uniforms;

            if (jfx.samplers != null)
                resource.cpuSamplers = jfx.samplers;

            context.completeCallback?.Invoke();

            resource.State = EResourceState.WaitGPUTransfer;
            return;

        LoadFailed:
            resource.State = EResourceState.Failed;
            throw new Exception("Failed to load texture resource!");
        }

        public bool Exists(string name)
        {
            return cache.ContainsKey(name);
        }

        #region Specification
        // JSON Serializer Configuration
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            IncludeFields = true
        };

        public struct SShaderProgram
        {
            public int type;
        }

        public struct SJFXFile
        {
            public SShaderProgram program;
            public string[] uniforms;
            public SShaderSampler[] samplers;
            public string[] vertexSource;
            public string[] fragmentSource;
            public string[] geometrySource;
        }

        #endregion
    }
}
