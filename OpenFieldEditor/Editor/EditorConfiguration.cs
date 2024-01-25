using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFieldEditor.Editor
{
    internal class EditorConfiguration
    {
        // Properties
        public static string ConfigurationFilepath => configurationFilepath;

        public static string EditorDataPath => editorDataPath;

        // Private Data
        private static string editorDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Open Field Editor");
        private static string editorProgramPath = Application.StartupPath;
        private static string configurationFilepath = Path.Combine(editorDataPath, "EditorConfig.json");
    }
}
