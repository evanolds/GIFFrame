using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GIFFrame
{
    internal partial class SaveAllForm : Form
    {
        EOGIFLoader m_loader;
        
        public SaveAllForm(EOGIFLoader loader)
        {
            m_loader = loader;
        
            InitializeComponent();
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (DialogResult.OK == fbd.ShowDialog())
                {
                    tbFolder.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnOK.Visible = false;
            for (int i = 0; i < m_loader.ImageCount; i++)
            {
                string name = string.Format("{0}\\{1}", tbFolder.Text, tbFileName.Text).Replace(
                    "%", Convert.ToString(i));
                System.Drawing.Imaging.ImageFormat fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
                if (0 == cbFormats.SelectedIndex)
                {
                    fmt = System.Drawing.Imaging.ImageFormat.Png;

                    // Make sure the extension is there
                    if (!name.ToLower().EndsWith(".png"))
                    {
                        name += ".png";
                    }
                }
                else if (!name.ToLower().EndsWith(".jpg") && !name.ToLower().EndsWith(".jpeg") &&
                    !name.ToLower().EndsWith(".jfif"))
                {
                    name += ".jpg";
                }
                
                m_loader.GetBM32(i).Save(name, fmt);
            }
            
            this.DialogResult = DialogResult.OK;
        }

        private void SaveAllForm_Load(object sender, EventArgs e)
        {
            cbFormats.SelectedIndex = 0;
        }

        private void tbFolder_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = !string.IsNullOrEmpty(tbFolder.Text) && 
                System.IO.Directory.Exists(tbFolder.Text);
        }
    }
}
