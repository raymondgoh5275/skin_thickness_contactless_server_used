using System;
using System.Windows.Forms;

namespace ATS_Global.Forms.Components
{
    public partial class FileSelector : UserControl
    {
        public string FileName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string Filter { get; set; }

        public bool CheckFileExists { get; set; }

        public bool CheckPathExists { get; set; }

        public FileSelector()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = Filter;
                ofd.Multiselect = false;
                ofd.CheckFileExists = CheckFileExists;
                ofd.CheckPathExists = CheckPathExists;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = ofd.FileName;
                }
            }
        }
    }
}
