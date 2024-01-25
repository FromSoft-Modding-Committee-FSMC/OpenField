using OpenFieldEditor.Editor;

namespace OpenFieldEditor
{
    internal static class Program
    {
        //Properties
        public static ProgramContext? Context => context;

        public static EditorConfiguration? EditorConfiguration => editorConfig;

        //Private Data
        private static EditorConfiguration? editorConfig;
        private static ProgramContext? context;



        [STAThread]
        static void Main()
        {
            //Do this annoying shit winforms needs
            ApplicationConfiguration.Initialize();
            context = new ProgramContext(new SplashWindow());

            //Initialize some major classes
            
            editorConfig = new();

            //Start it.
            Application.Run(context);
        }
    }
}