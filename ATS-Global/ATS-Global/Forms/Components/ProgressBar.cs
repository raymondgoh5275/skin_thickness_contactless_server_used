using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ATS_Global.Forms.Components
{
    public partial class ProgressBar : UserControl
    {
        public string StatusLine 
        {
            get { return this.label1.Text; }
            set { this.label1.Text = value; }
        }

        public int Progress
        {
            get { return this.progressBar1.Value; }
            set { this.progressBar1.Value = value; }
        }
        public int MaxValue
        {
            get { return this.progressBar1.Maximum; }
            set { this.progressBar1.Maximum = value; }
        }
        public int MinValue
        {
            get { return this.progressBar1.Minimum; }
            set { this.progressBar1.Minimum = value; }
        }

        public ProgressBar()
        {
            InitializeComponent();
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = 100;
            this.label1.Text = "";
        }

        private Object thisLock = new object();
        public void Update(string status, int progress)
        {
            lock (thisLock)
            {
                this.progressBar1.Value = progress;
                this.label1.Text = status;
            }
        }
    }
}
