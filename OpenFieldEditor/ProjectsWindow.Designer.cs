namespace OpenFieldEditor
{
    partial class ProjectsWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectsWindow));
            flRecentProjects = new FlowLayoutPanel();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            btNewProject = new Button();
            btOpenProject = new Button();
            flRecentProjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // flRecentProjects
            // 
            flRecentProjects.AutoScroll = true;
            flRecentProjects.BackColor = SystemColors.AppWorkspace;
            flRecentProjects.Controls.Add(pictureBox2);
            flRecentProjects.Controls.Add(pictureBox1);
            flRecentProjects.Location = new Point(12, 12);
            flRecentProjects.Name = "flRecentProjects";
            flRecentProjects.Size = new Size(776, 132);
            flRecentProjects.TabIndex = 0;
            flRecentProjects.WrapContents = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.BorderStyle = BorderStyle.Fixed3D;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.InitialImage = (Image)resources.GetObject("pictureBox2.InitialImage");
            pictureBox2.Location = new Point(2, 2);
            pictureBox2.Margin = new Padding(2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(128, 128);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(134, 2);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(128, 128);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btNewProject
            // 
            btNewProject.FlatAppearance.BorderSize = 0;
            btNewProject.FlatStyle = FlatStyle.Flat;
            btNewProject.ForeColor = SystemColors.Highlight;
            btNewProject.ImageAlign = ContentAlignment.MiddleLeft;
            btNewProject.Location = new Point(14, 150);
            btNewProject.Name = "btNewProject";
            btNewProject.Size = new Size(128, 23);
            btNewProject.TabIndex = 1;
            btNewProject.Text = "New Project...";
            btNewProject.TextAlign = ContentAlignment.MiddleLeft;
            btNewProject.UseVisualStyleBackColor = false;
            // 
            // btOpenProject
            // 
            btOpenProject.FlatAppearance.BorderSize = 0;
            btOpenProject.FlatStyle = FlatStyle.Flat;
            btOpenProject.ForeColor = SystemColors.Highlight;
            btOpenProject.ImageAlign = ContentAlignment.MiddleLeft;
            btOpenProject.Location = new Point(14, 179);
            btOpenProject.Name = "btOpenProject";
            btOpenProject.Size = new Size(128, 23);
            btOpenProject.TabIndex = 2;
            btOpenProject.Text = "Open Project...";
            btOpenProject.TextAlign = ContentAlignment.MiddleLeft;
            btOpenProject.UseVisualStyleBackColor = false;
            btOpenProject.Click += btOpenProject_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 210);
            Controls.Add(btOpenProject);
            Controls.Add(btNewProject);
            Controls.Add(flRecentProjects);
            Name = "MainWindow";
            Text = "Open Field Editor";
            Load += MainWindow_Load;
            flRecentProjects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flRecentProjects;
        private PictureBox pictureBox1;
        private Button btNewProject;
        private Button btOpenProject;
        private PictureBox pictureBox2;
    }
}