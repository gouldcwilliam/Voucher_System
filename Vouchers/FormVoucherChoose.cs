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
    public partial class FormVoucherChoose : Form
    {
        public FormVoucherChoose()
        {
            InitializeComponent();
        }

        private void FormVoucherChoose_Load(object sender, EventArgs e)
        {
            comboBoxNationalAccount.DataSource = SQL.Select("select RAILROAD from [NATIONAL ACCOUNTS1] order by RAILROAD");
            comboBoxNationalAccount.DisplayMember = "Railroad";
            comboBoxNationalAccount.ValueMember = "Railroad";
        }
    }
}
