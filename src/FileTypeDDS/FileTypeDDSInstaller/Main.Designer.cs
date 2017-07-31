namespace FileTypeDDSInstaller
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.InstallGo = new System.Windows.Forms.Button();
            this.BackOut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ProgressLoad = new System.Windows.Forms.ProgressBar();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // InstallGo
            // 
            this.InstallGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.InstallGo.Location = new System.Drawing.Point(417, 254);
            this.InstallGo.Name = "InstallGo";
            this.InstallGo.Size = new System.Drawing.Size(103, 38);
            this.InstallGo.TabIndex = 0;
            this.InstallGo.Text = "Install";
            this.InstallGo.UseVisualStyleBackColor = true;
            this.InstallGo.Click += new System.EventHandler(this.InstallGo_Click);
            // 
            // BackOut
            // 
            this.BackOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BackOut.Location = new System.Drawing.Point(12, 254);
            this.BackOut.Name = "BackOut";
            this.BackOut.Size = new System.Drawing.Size(103, 38);
            this.BackOut.TabIndex = 1;
            this.BackOut.Text = "Cancel";
            this.BackOut.UseVisualStyleBackColor = true;
            this.BackOut.Click += new System.EventHandler(this.BackOut_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(172, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 33);
            this.label1.TabIndex = 2;
            this.label1.Text = "FileTypeDDS";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(115, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(303, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "A badass DDS plugin for the latest Paint.NET";
            // 
            // ProgressLoad
            // 
            this.ProgressLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressLoad.Location = new System.Drawing.Point(12, 150);
            this.ProgressLoad.MarqueeAnimationSpeed = 30;
            this.ProgressLoad.Name = "ProgressLoad";
            this.ProgressLoad.Size = new System.Drawing.Size(508, 23);
            this.ProgressLoad.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ProgressLoad.TabIndex = 4;
            this.ProgressLoad.Visible = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "PaintDotNet Application (*.exe)|*.exe";
            this.openFileDialog.Title = "Select your PaintDotNet.exe";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(532, 304);
            this.Controls.Add(this.ProgressLoad);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BackOut);
            this.Controls.Add(this.InstallGo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FileTypeDDS Plugin Installer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button InstallGo;
        private System.Windows.Forms.Button BackOut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar ProgressLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}

