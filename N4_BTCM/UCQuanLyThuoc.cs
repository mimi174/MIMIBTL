using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N4_BTCM
{
    public partial class UCQuanLyThuoc : UserControl
    {
        public UCQuanLyThuoc()
        {
            InitializeComponent();
        }

        private void UCQuanLyThuoc_Load(object sender, EventArgs e)
        {

        }

        private void dgvThuoc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để thêm thuốc
            MessageBox.Show("Chức năng thêm thuốc đang được phát triển");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để xóa thuốc
            MessageBox.Show("Chức năng xóa thuốc đang được phát triển");
        }
    }
}