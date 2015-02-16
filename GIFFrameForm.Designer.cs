namespace GIFFrame
{
    partial class GIFFrameForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GIFFrameForm));
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSaveCurrent = new System.Windows.Forms.ToolStripButton();
            this.tsbSaveAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.clickToViewCommercialIOSAppsByAuthorwebLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tspbLoadProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tsmiGitHubLink = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.Location = new System.Drawing.Point(12, 28);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(64, 222);
            this.listBox1.TabIndex = 2;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpen,
            this.toolStripSeparator1,
            this.tsbSaveCurrent,
            this.tsbSaveAll,
            this.toolStripDropDownButton1,
            this.tspbLoadProgress});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(590, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbOpen
            // 
            this.tsbOpen.Image = global::GIFFrame.Properties.Resources.openHS;
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(65, 22);
            this.tsbOpen.Text = "Open...";
            this.tsbOpen.Click += new System.EventHandler(this.tsbOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbSaveCurrent
            // 
            this.tsbSaveCurrent.Image = global::GIFFrame.Properties.Resources.saveHS;
            this.tsbSaveCurrent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveCurrent.Name = "tsbSaveCurrent";
            this.tsbSaveCurrent.Size = new System.Drawing.Size(135, 22);
            this.tsbSaveCurrent.Text = "Save current frame...";
            this.tsbSaveCurrent.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tsbSaveAll
            // 
            this.tsbSaveAll.Image = global::GIFFrame.Properties.Resources.SaveAllHS;
            this.tsbSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveAll.Name = "tsbSaveAll";
            this.tsbSaveAll.Size = new System.Drawing.Size(114, 22);
            this.tsbSaveAll.Text = "Save all frames...";
            this.tsbSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clickToViewCommercialIOSAppsByAuthorwebLinkToolStripMenuItem,
            this.tsmiGitHubLink});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(85, 22);
            this.toolStripDropDownButton1.Text = "More apps...";
            // 
            // clickToViewCommercialIOSAppsByAuthorwebLinkToolStripMenuItem
            // 
            this.clickToViewCommercialIOSAppsByAuthorwebLinkToolStripMenuItem.Name = "clickToViewCommercialIOSAppsByAuthorwebLinkToolStripMenuItem";
            this.clickToViewCommercialIOSAppsByAuthorwebLinkToolStripMenuItem.Size = new System.Drawing.Size(387, 22);
            this.clickToViewCommercialIOSAppsByAuthorwebLinkToolStripMenuItem.Text = "Click to view commercial iOS apps by author (web link)";
            this.clickToViewCommercialIOSAppsByAuthorwebLinkToolStripMenuItem.Click += new System.EventHandler(this.btnGoToiOSApps_Click);
            // 
            // tspbLoadProgress
            // 
            this.tspbLoadProgress.Name = "tspbLoadProgress";
            this.tspbLoadProgress.Size = new System.Drawing.Size(100, 22);
            this.tspbLoadProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.tspbLoadProgress.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(82, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(496, 222);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // tsmiGitHubLink
            // 
            this.tsmiGitHubLink.Name = "tsmiGitHubLink";
            this.tsmiGitHubLink.Size = new System.Drawing.Size(387, 22);
            this.tsmiGitHubLink.Text = "Click to view open source apps by author (GitHub web link)";
            this.tsmiGitHubLink.Click += new System.EventHandler(this.tsmiGitHubLink_Click);
            // 
            // GIFFrameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(590, 262);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listBox1);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "GIFFrameForm";
            this.Text = "Animated GIF Frame Extractor v1.2";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbSaveCurrent;
        private System.Windows.Forms.ToolStripButton tsbSaveAll;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem clickToViewCommercialIOSAppsByAuthorwebLinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar tspbLoadProgress;
        private System.Windows.Forms.ToolStripMenuItem tsmiGitHubLink;
    }
}

