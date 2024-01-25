using OpenFieldEditor.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenFieldEditor
{
    public partial class SplashWindow : Form
    {
        public SplashWindow()
        {
            InitializeComponent();


        }

        private void SetLoadingHint(string loadingHint)
        {
            lbLoadHint.Text = loadingHint;
            Refresh();
        }

        private void SplashWindow_Shown(object sender, EventArgs e)
        {
            //Is this the editors first launch?
            if (!Path.Exists(EditorConfiguration.EditorDataPath))
            {
                SetLoadingHint("Preparing For First Time Use...");
                Thread.Sleep(1000);

                Directory.CreateDirectory(EditorConfiguration.EditorDataPath);
            }
            

            //Check for editor updates
            SetLoadingHint("Checking for updates...");
            Thread.Sleep(1000);

            //Change Main Window
            if (Program.Context == null)
                throw new Exception("Program Context is null!");

            Program.Context.ChangePrimaryWindow(new ProjectsWindow());
        }
    }
}
