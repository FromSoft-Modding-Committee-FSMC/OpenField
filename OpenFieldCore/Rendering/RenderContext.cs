using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Rendering
{
    public static class RenderContext
    {
        public static uint CurrentMaterial = 0;
        public static uint CurrentTexture  = 0;
        public static uint CurrentShader   = 0;

        public static Camera CurrentCamera = null;

        public static void Reset()
        {
            CurrentTexture  = 0xFFFFFFFF;
            CurrentMaterial = 0xFFFFFFFF;
            CurrentShader   = 0xFFFFFFFF;

            CurrentCamera   = null;
        }

        public static void Dump()
        {
            Console.WriteLine("Current Render Context: ");
            Console.WriteLine($"\tBound Texture FNV1 = 0x{CurrentTexture:X8}");
        }
    }
}
