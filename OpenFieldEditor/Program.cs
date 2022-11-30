using OFC.Asset.Factory;
using OFC.Utility;
using System;

namespace OFE
{
    class Program
    {
        public static TextureFactory textureFactory;

        static void Main()
        {
            //Program Initialization
            Log.EnableColour(true);

            //Initialize Resource Handlers
            textureFactory = new TextureFactory();
            textureFactory.RegisterHandler(new OFF.FileFormat.FileFormatTIM()); //Register TIM

            Log.Info("Exportable Formats: ");
            foreach(string s in textureFactory.EnumerateExportableFormats())
            {
                Log.Info($"(Texture) {s}");
            }

            Log.Info("Importable Formats: ");
            foreach (string s in textureFactory.EnumerateImportableFormats())
            {
                Log.Info($"(Texture) {s}");
            }

            using Game game = new Game();
            game.Run();
        }
    }
}
