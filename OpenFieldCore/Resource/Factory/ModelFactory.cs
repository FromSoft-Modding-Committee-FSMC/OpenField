using System;
using System.IO;
using System.Collections.Concurrent;

using OFC.Resource.FileFormat;
using OFC.Resource.Format;
using OFC.Resource.Model;
using OFC.Utility;
using OFC.Resource.Texture;

namespace OFC.Resource.Factory
{
    public class ModelFactory : FormatFactory<ModelResource>
    {
        // Data
        readonly ConcurrentDictionary<string, ModelResource> cache;

        // Indexer
        public ModelResource this[string name]
        {
            get { return cache[name]; }
            set { cache[name] = value; }
        }

        // Constructor
        public ModelFactory()
        {
            cache = new ConcurrentDictionary<string, ModelResource>();

            RegisterFormat(new MS3DFormat());
        }

        public void LoadExplicit(SResourceLoadContext context)
        {
            ModelResource resource = cache[context.name];

            if (!File.Exists(context.source))
            {
                Log.Error($"Couldn't find file '{context.source}'!");
                goto LoadFailed;
            }

            byte[] buffer = File.ReadAllBytes(context.source);

            IFormat<ModelResource> format = GetFormat(buffer);

            if (format == null)
            {
                Log.Error($"Model File '{resource.Source}' is in an unknown format!");
                goto LoadFailed;
            }

            // Attempt to load resource content
            resource.State = EResourceState.LoadingIntermediate;

            if (!format.Load(buffer, ref resource, context.parameters))
            {
                Log.Error($"Model File '{resource.Source}' failed to load!");
                goto LoadFailed;
            }
                

            // Run Callback
            context.completeCallback?.Invoke();

            // [Success] Set WaitGPUTransfer state and return
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
    }
}
