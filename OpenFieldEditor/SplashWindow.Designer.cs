namespace OpenFieldEditor
{
    partial class SplashWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbLoadHint = new Label();
            SuspendLayout();
            // 
            // lbLoadHint
            // 
            lbLoadHint.AutoSize = true;
            lbLoadHint.BackColor = SystemColors.ActiveCaptionText;
            lbLoadHint.ForeColor = SystemColors.ButtonHighlight;
            lbLoadHint.Location = new Point(12, 336);
            lbLoadHint.Name = "lbLoadHint";
            lbLoadHint.Size = new Size(196, 15);
            lbLoadHint.TabIndex = 0;
            lbLoadHint.Text = "ExampleExampleExampleExample...";
            // 
            // SplashWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackgroundImage = Properties.Resources.ofe_splash;
            ClientSize = new Size(640, 360);
            ControlBox = false;
            Controls.Add(lbLoadHint);
            FormBorderStyle = FormBorderStyle.None;
            Name = "SplashWindow";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Open Field Editor";
            Shown += SplashWindow_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbLoadHint;
    }
}