using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GIFFrame
{
    public partial class BorderedLabel : UserControl
    {
        int m_brdrWidth;
        int m_margin;
        
        public BorderedLabel()
        {
            m_brdrWidth = 2;
            m_margin = 2;
            
            InitializeComponent();
        }

        private void BorderedLabel_Paint(object sender, PaintEventArgs e)
        {
            using (Pen p = new Pen(Color.Black, (float)m_brdrWidth))
            {
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                Rectangle r;
                if (1 == m_brdrWidth)
                {
                    r = new Rectangle(0, 0, this.Width - m_brdrWidth, this.Height - m_brdrWidth);
                }
                else
                {
                    r = new Rectangle(m_brdrWidth / 2, m_brdrWidth / 2, 
                        this.Width - m_brdrWidth, this.Height - m_brdrWidth);
                }
                e.Graphics.DrawRectangle(p, r);

                // Render the text in the middle of the control
                using (Brush b = new SolidBrush(Color.Black))
                {
                    e.Graphics.DrawString(this.Text, this.Font, b,
                        new RectangleF((float)m_margin, (float)m_margin, 
                            (float)(this.Width - m_margin), (float)(this.Height - m_margin)));
                }
            }
        }

        public string Label
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.Refresh();
            }
        }
    }
}
