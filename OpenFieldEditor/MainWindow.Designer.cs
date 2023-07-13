namespace OpenFieldEditor
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.flRecentProjects = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btNewProject = new System.Windows.Forms.Button();
            this.btOpenProject = new System.Windows.Forms.Button();
            this.flRecentProjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // flRecentProjects
            // 
            this.flRecentProjects.AutoScroll = true;
            this.flRecentProjects.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.flRecentProjects.Controls.Add(this.pictureBox2);
            this.flRecentProjects.Controls.Add(this.pictureBox1);
            this.flRecentProjects.Location = new System.Drawing.Point(12, 12);
            this.flRecentProjects.Name = "flRecentProjects";
            this.flRecentProjects.Size = new System.Drawing.Size(776, 132);
            this.flRecentProjects.TabIndex = 0;
            this.flRecentProjects.WrapContents = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(134, 2);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.InitialImage")));
            this.pictureBox2.Location = new System.Drawing.Point(2, 2);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(128, 128);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // btNewProject
            // 
            this.btNewProject.FlatAppearance.BorderSize = 0;
            this.btNewProject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btNewProject.ForeColor = System.Drawing.SystemColors.Highlight;
            this.btNewProject.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btNewProject.Location = new System.Drawing.Point(14, 150);
            this.btNewProject.Name = "btNewProject";
            this.btNewProject.Size = new System.Drawing.Size(128, 23);
            this.btNewProject.TabIndex = 1;
            this.btNewProject.Text = "New Project...";
            this.btNewProject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btNewProject.UseVisualStyleBackColor = false;
            // 
            // btOpenProject
            // 
            this.btOpenProject.FlatAppearance.BorderSize = 0;
            this.btOpenProject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btOpenProject.ForeColor = System.Drawing.SystemColors.Highlight;
            this.btOpenProject.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btOpenProject.Location = new System.Drawing.Point(14, 179);
            this.btOpenProject.Name = "btOpenProject";
            this.btOpenProject.Size = new System.Drawing.Size(128, 23);
            this.btOpenProject.TabIndex = 2;
            this.btOpenProject.Text = "Open Project...";
            this.btOpenProject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btOpenProject.UseVisualStyleBackColor = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 210);
            this.Controls.Add(this.btOpenProject);
            this.Controls.Add(this.btNewProject);
            this.Controls.Add(this.flRecentProjects);
            this.Name = "MainWindow";
            this.Text = "Open Field Editor";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.flRecentProjects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flRecentProjects;
        private PictureBox pictureBox1;
        private Button btNewProject;
        private Button btOpenProject;
        private PictureBox pictureBox2;
    }
}