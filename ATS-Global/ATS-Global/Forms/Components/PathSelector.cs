// -----------------------------------------------------------------------
// <copyright file="PathSelector.cs" company="ATS-Global">
// Copyright (c) 2012 ATS Ltd.
// </copyright>
// -----------------------------------------------------------------------

namespace ATS_Global.Forms.Components
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.IO;

    public partial class PathSelector : UserControl
    {
        private bool _ReadOnly = false;
        public bool ReadOnly
        {
            get
            {
                return this._ReadOnly;
            }
            set
            {
                this._ReadOnly = value;
                UpdateReadOnly();
            }
        }

        private void UpdateReadOnly()
        {
            this.textBox1.ReadOnly = this._ReadOnly;
            this.button1.Enabled = !this._ReadOnly;
        }

        public string Path
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        public PathSelector()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (Directory.Exists(textBox1.Text))
                {
                    fbd.SelectedPath = textBox1.Text;
                }

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.textBox1.Text = fbd.SelectedPath;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
