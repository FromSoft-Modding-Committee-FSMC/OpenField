using System.Collections.Generic;

using OFC.Asset.Format;
using OFC.Asset.FileFormat;

using OFC.Utility;

namespace OFC.Asset.Factory
{
    public class TextureFactory : IFactory<TextureAsset>
    {
        private readonly List<IFormat<TextureAsset>> textureHandlers;

        public TextureFactory()
        {
            textureHandlers = new List<IFormat<TextureAsset>>
            {
                new FileFormatDDS()
            };
        }

        /// <summary>
        /// Registers a new IFormat handler
        /// </summary>
        /// <param name="handler">An instance of the handler</param>
        /// <returns>If registration was successful or not.</returns>
        public bool RegisterHandler(IFormat<TextureAsset> handler)
        {
            if(handler == null)
            {
                Log.Error("Attempted to register a null format handler.");
                return false;
            }

            textureHandlers.Add(handler);
            return true;
        }

        /// <summary>
        /// Returns a IFormat handler using an index
        /// </summary>
        /// <param name="index">Index of the handler</param>
        /// <returns>The IFormat handler</returns>
        public IFormat<TextureAsset> GetHandler(int index)
        {
            if(index < 0 || index >= textureHandlers.Count)
            {
                return null;
            }

            return textureHandlers[index];
        }

        /// <summary>
        /// Returns a IFormat handler using a name
        /// </summary>
        /// <param name="name">Name of the handler</param>
        /// <returns>The IFormat handler</returns>
        public IFormat<TextureAsset> GetHandler(string name)
        {
            foreach(IFormat<TextureAsset> handler in textureHandlers)
            {
                if(handler.Parameters.description == name)
                {
                    return handler;
                }
            }

            return null;
        }

        /// <summary> Enumerates all importable formats </summary>
        /// <returns> Returns a list of strings, which contain the description of the format and all possible extensions. </returns>
        public List<string> EnumerateImportableFormats()
        {
            List<string> importableFormats = new List<string>();

            int currentIndex = 0;
            foreach (IFormat<TextureAsset> handler in textureHandlers)
            {
                if (handler.Parameters.allowImport)
                {
                    importableFormats.Add($"[id: {currentIndex}, name: \"{handler.Parameters.name}\", description: \"{handler.Parameters.description}\", extensions: ({string.Join(", ", handler.Parameters.extensions)})]");
                }

                currentIndex++;
            }

            return importableFormats;
        }

        /// <summary> Enumerates all exportable formats </summary>
        /// <returns> Returns a list of strings, which contain the description of the format and all possible extensions. </returns>
        public List<string> EnumerateExportableFormats()
        {
            List<string> exportableFormats = new List<string>();

            int currentIndex = 0;
            foreach (IFormat<TextureAsset> handler in textureHandlers)
            {
                if (handler.Parameters.allowExport)
                {
                    exportableFormats.Add($"[id: {currentIndex}, name: \"{handler.Parameters.name}\", description: \"{handler.Parameters.description}\", extensions: ({string.Join(", ", handler.Parameters.extensions)})]");
                }

                currentIndex++;
            }

            return exportableFormats;
        }
    }
}
