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
    public partial class UCQuanLyKhachHang : UserControl
    {
        public UCQuanLyKhachHang()
        {
            InitializeComponent();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để thêm khách hàng
            MessageBox.Show("Chức năng thêm khách hàng đang được phát triển");
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để sửa khách hàng
            MessageBox.Show("Chức năng sửa khách hàng đang được phát triển");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để xóa khách hàng
            MessageBox.Show("Chức năng xóa khách hàng đang được phát triển");
        }

        private void dgvKhachHang_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}