// Author:
//    Evan Olds

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GIFFrame
{
    public partial class GIFFrameForm : Form
    {
        EOGIFLoader m_loader;
        
        public GIFFrameForm()
        {
            m_loader = null;
            
            InitializeComponent();
        }

        private void btnGoToiOSApps_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                "https://itunes.apple.com/us/artist/evan-olds/id736362046");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Make sure there's an image to save
            if (null == pictureBox1.Image)
            {
                return;
            }
            
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG Files|*.jpeg;*.jpg|PNG Files|*.png";

            // Prompt for save file name and return if not OK'ed
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Save
            System.Drawing.Imaging.ImageFormat fmt = sfd.FileName.ToLower().EndsWith(".png") ?
                System.Drawing.Imaging.ImageFormat.Png : System.Drawing.Imaging.ImageFormat.Jpeg;
            pictureBox1.Image.Save(sfd.FileName, fmt);
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            // Nothing to do if they haven't opened a GIF yet
            if (null == m_loader)
            {
                return;
            }

            SaveAllForm saf = new SaveAllForm(m_loader);
            saf.ShowDialog();
            saf.Dispose();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null == m_loader || listBox1.SelectedIndex < 0)
            {
                return;
            }

            pictureBox1.Image = m_loader.GetBM32(listBox1.SelectedIndex);
        }

        private void LoadDone(string errorMsg)
        {
            if (string.IsNullOrEmpty(errorMsg))
            {
                // Load was successful
                
                // Populate the list box with numbers for the frames
                for (int i = 0; i < m_loader.ImageCount; i++)
                {
                    listBox1.Items.Add(Convert.ToString(i));
                }

                // Select the first frame by default
                listBox1.SelectedIndex = 0;
            }
            else
            {
                // Load failed
                MessageBox.Show(
                    this, errorMsg, this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }

            // Reset interface
            tsbOpen.Text = "Open...";
            tsbOpen.Enabled = true;
            tspbLoadProgress.Visible = false;
            tsbSaveCurrent.Visible = true;
            tsbSaveAll.Visible = true;
        }

        private void LoadOnThread(object objectFileName)
        {
            string fileName = objectFileName as string;

            string errMsg = null;
            try
            {
                m_loader = new EOGIFLoader(fileName);
            }
            catch (ArgumentException)
            {
                errMsg = "The specified file could not be loaded. " +
                    "The file may not be a valid GIF file or may be inaccessible.";
            }

            this.BeginInvoke(new Action<string>(LoadDone), errMsg);
        }

        private void tsbOpen_Click(object sender, EventArgs e)
        {
            // Create an open file dialog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "GIF Files|*.gif";

            // Prompt for input file
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Clear frame list and current image
            listBox1.Items.Clear();
            pictureBox1.Image = null;

            // Prepare for loading on a new thread
            tsbOpen.Text = "Opening...";
            tsbOpen.Enabled = false;
            tsbSaveCurrent.Visible = false;
            tsbSaveAll.Visible = false;
            tspbLoadProgress.Visible = true;

            // Start loading on a new thread
            ParameterizedThreadStart pts = new ParameterizedThreadStart(LoadOnThread);
            Thread t = new Thread(pts);
            t.Start(ofd.FileName);
        }

        private void tsmiGitHubLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/evanolds");
        }
    }
}