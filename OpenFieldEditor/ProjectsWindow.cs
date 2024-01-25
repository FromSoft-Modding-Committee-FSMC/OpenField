using static System.Net.Mime.MediaTypeNames;

namespace OpenFieldEditor
{
    public partial class ProjectsWindow : Form
    {
        public ProjectsWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void btOpenProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = false,
                Filter = "Open Field Project (*.epf)|*.epf",
                FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Open Field Projects/")
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                //Make double sure this is a valid file
                if(!File.Exists(ofd.FileName))
                {
                    //It's valid for this to happen, so don't make a big deal out of it.
                    return;
                }

                Console.WriteLine(ofd.FileName);
            }
        }
    }
}