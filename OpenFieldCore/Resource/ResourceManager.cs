using System;
using System.IO;
using System.Reflection;
using System.Threading;

using OFC.Hashing;
using OFC.Resource.Factory;
using OFC.Resource.Material;
using OFC.Resource.Model;
using OFC.Resource.Shader;
using OFC.Resource.Texture;

namespace OFC.Resource
{
    public static partial class ResourceManager
    {
        // Factories
        static TextureFactory textureFactory;
        static ModelFactory modelFactory;
        static ShaderFactory shaderFactory;
        static MaterialFactory materialFactory;

        // Data
        static bool isInitialized = false;

        // Properties
        public static string ResourcePath => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Resources");

        public static void Initialize()
        {
            //Initialization Guard
            if(isInitialized)
                return;
            isInitialized = true;

            // Initialize Material Factory
            materialFactory = new MaterialFactory();
            //materialFactory.Store("ofcDefaultMat", new BasicMaterial

            //Initialize Other Factories
            textureFactory = new TextureFactory();
            modelFactory = new ModelFactory();
            shaderFactory = new ShaderFactory();
            materialFactory = new MaterialFactory();
        }

        public static void Dump()
        {
            textureFactory.Dump();
            materialFactory.Dump();
        }

        /// <summary>
        /// Synchronously loads a texture
        /// </summary>
        /// <param name="source">File to load the texture from</param>
        /// <param name="parameters">Loader configuration parameters</param>
        /// <param name="texture">TextureResource object</param>
        /// <returns>Resource Internal Name</returns>
        public static string Load(string source, out TextureResource texture, STextureLoadParameters? parameters = null)
        {
            // What is the name of this file?
            string name = Path.GetFileNameWithoutExtension(source);

            // See if this resource has already been loaded.
            if (textureFactory.Exists(name))
            {
                texture = textureFactory[name];
                texture.ReferenceCount++;
                return name;
            }

            // Create a new resource
            texture = new TextureResource()
            {
                Source          = Path.Combine(ResourcePath, source),
                State           = EResourceState.Unloaded,
                ReferenceCount  = 1
            };
            textureFactory[name] = texture;

            // Create resource load context
            SResourceLoadContext context = new()
            {
                completeCallback = null,
                parameters = parameters,
                source = texture.Source,
                name = name
            };

            // Load the resource data
            textureFactory.LoadExplicit(context);

            return name;
        }

        public static string Load(string source, out ModelResource model)
        {
            string name = Path.GetFileNameWithoutExtension(source);

            if(modelFactory.Exists(name))
            {
                model = modelFactory[name];
                model.ReferenceCount++;
                return name;
            }

            model = new ModelResource()
            {
                Source         = Path.Combine(ResourcePath, source),
                State          = EResourceState.Unloaded,
                ReferenceCount = 1
            };
            modelFactory[name] = model;

            SResourceLoadContext context = new()
            {
                completeCallback = null,
                parameters = null,
                source = model.Source,
                name = name
            };

            modelFactory.LoadExplicit(context);

            return name;
        }

        public static string Load(string source, out ShaderResource shader)
        {
            string name = Path.GetFileNameWithoutExtension(source);

            if (shaderFactory.Exists(name))
            {
                shader = shaderFactory[name];
                shader.ReferenceCount++;
                return name;
            }

            shader = new ShaderResource()
            {
                Source = Path.Combine(ResourcePath, source),
                State = EResourceState.Unloaded,
                ReferenceCount = 1
            };
            shaderFactory[name] = shader;

            SResourceLoadContext context = new()
            {
                completeCallback = null,
                parameters = null,
                source = shader.Source,
                name = name
            };

            shaderFactory.LoadExplicit(context);

            return name;
        }

        public static void Store(string name, MaterialResource material) => materialFactory.Store(name, material);
        public static bool Get(string name, out MaterialResource material) => materialFactory.Get(name, out material);
    }
}