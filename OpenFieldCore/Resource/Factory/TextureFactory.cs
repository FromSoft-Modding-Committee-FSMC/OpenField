using System;
using System.IO;
using System.Collections.Concurrent;

using OFC.Resource.FileFormat;
using OFC.Resource.Format;
using OFC.Resource.Texture;
using OFC.Utility;
using OFC.Hashing;

namespace OFC.Resource.Factory
{
    public class TextureFactory : FormatFactory<TextureResource>
    {
        // Data
        ConcurrentDictionary<string, TextureResource> cache;

        // Indexer
        public TextureResource this[string name]
        {
            get { return cache[name];  }
            set { cache[name] = value; }
        }

        // Constructor
        public TextureFactory()
        {
            cache = new ConcurrentDictionary<string, TextureResource>();

            RegisterFormat(new TGAFormat());
        }

        public void LoadExplicit(SResourceLoadContext context)
        {
            // Grab resource
            TextureResource resource = cache[context.name];

            // Ensure the file exists
            if (!File.Exists(context.source))
            {
                Log.Error($"Couldn't find file '{context.source}'!");
                goto LoadFailed;
            }

            // Load the contents of the file into a buffer
            byte[] buffer = File.ReadAllBytes(context.source);

            // Search for possible format handler
            IFormat<TextureResource> format = GetFormat(buffer);

            if (format == null)
            {
                Log.Error($"Texture File '{resource.Source}' is in an unknown format!");
                goto LoadFailed;
            }

            // We generate a hash for the resource
            resource.Hash = FNV1A.Instance.Calculate(ref buffer);

            // Attempt to load resource content
            resource.State = EResourceState.LoadingIntermediate;

            if (!format.Load(buffer, ref resource, context.parameters))
                goto LoadFailed;

            // Run Callback
            context.completeCallback?.Invoke();

            // [Success] Set WaitGPUTransfer state and return
            resource.State = EResourceState.WaitGPUTransfer;
            return;

            // [Failing] Set Failed state and throw exception
        LoadFailed:
            resource.State = EResourceState.Failed;
            throw new Exception("Failed to load texture resource!");
        }

        public bool Exists(string name)
        {
            return cache.ContainsKey(name);
        }

        public void Dump()
        {
            Console.WriteLine("TextureFactory Contents: ");
            foreach(string key in cache.Keys)
                Console.WriteLine($"\tTexture2D [name = {key}, refcount = {this[key].ReferenceCount}]");
        }
    }
}
