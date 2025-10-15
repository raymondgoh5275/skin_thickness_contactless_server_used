using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestContactless
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var serial = "serial_test3";
            var batch = "batch_test3";
            var resource = "resource_test3";
            var inspector = "inspector_test3";
            var created = DateTime.Now;
            var csvPath = @"C:\ATS_OEM\Skin Thickness\Csv_Storage";
            var inputpath = @"C:\ATS_OEM\Skin Thickness\Dat_Files_From_Oem";
            var csvFiles = new List<string> { "RRFBSGSNX46980_IF.csv", "RRFBSGSNX46980_OF.csv" };
            string constring = "Data Source=localhost; Initial Catalog=skin_thickness;Integrated Security=True;Asynchronous Processing=True;";
            var scan = new Contacless.CsvScan(csvPath, inputpath, constring, batch, serial, resource, inspector, created );
            textBox1.Text = scan.Parse(csvFiles);
        }
    }
}
