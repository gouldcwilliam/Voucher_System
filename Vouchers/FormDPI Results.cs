using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vouchers
{
    public partial class FormDPI_Results : Form
    {
        public FormDPI_Results()
        {
            InitializeComponent();
        }
        public FormDPI_Results(DataTable DataTable)
        {
            InitializeComponent();
            dataGridView1.DataSource = DataTable;
        }

        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0) { MessageBox.Show("No data to Export!"); return; }
            DataTable dt = (DataTable)dataGridView1.DataSource;
            System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".xlsx";
            sfd.Title = "Export data to location";
            sfd.FileName = "Export.xlsx";

            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            if (sfd.FileName == string.Empty) { MessageBox.Show("No output file selected"); return; }
            MessageBox.Show("This takes forever so be patient, it's not frozen  :)", "Output", MessageBoxButtons.OK);
            Functions.Excel_WriteDataTableToFile(dt, "DPI Results", sfd.FileName, "DPI");
            MessageBox.Show("Completed", "Output");
        }



    }
}
