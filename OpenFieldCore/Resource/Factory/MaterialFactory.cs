using OFC.Resource.Material;
using OFC.Resource.Model;
using OFC.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Resource.Factory
{
    public class MaterialFactory
    {
        // Data
        ConcurrentDictionary<string, MaterialResource> cache;

        // Indexer
        public MaterialResource this[string name]
        {
            get { return cache[name]; }
            set { cache[name] = value; }
        }


        public MaterialFactory()
        {
            cache = new ConcurrentDictionary<string, MaterialResource>();
        }

        public void Dump()
        {
            Console.WriteLine($"Material Factory Contents [cached items = {cache.Count}]: ");
            foreach(string name in cache.Keys)
                Console.WriteLine($"\t{this[name].GetType().Name} [name = {name}, refcount = ???]");
        }

        /// <summary>
        /// Gets a previously stored material.
        /// </summary>
        /// <param name="name">internal name of the material</param>
        /// <param name="material">the material to store</param>
        /// <returns>True on success, False otherwise</returns>
        public bool Get(string name, out MaterialResource material) => cache.TryGetValue(name, out material);


        /// <summary>
        /// Stores a user created material
        /// </summary>
        /// <param name="name">internal name of the material</param>
        /// <param name="material">the material to store</param>
        public void Store(string name, MaterialResource material)
        {
            if (Exists(name))
            {
                Log.Warn($"Cannot store material '{name}', a material with this name already exists.");
                return;
            }          
            
            cache[name] = material;
        }


        /// <summary>
        /// Checks if a material is already loaded
        /// </summary>
        /// <param name="name">internal name of the material</param>
        /// <returns>True if the material exists and False if it doesn't</returns>
        public bool Exists(string name) => cache.ContainsKey(name);
    }
}
