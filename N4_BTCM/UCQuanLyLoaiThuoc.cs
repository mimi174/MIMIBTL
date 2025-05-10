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
    public partial class UCQuanLyLoaiThuoc : UserControl
    {
        public UCQuanLyLoaiThuoc()
        {
            InitializeComponent();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để thêm loại thuốc
            MessageBox.Show("Chức năng thêm loại thuốc đang được phát triển");
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để sửa loại thuốc
            MessageBox.Show("Chức năng sửa loại thuốc đang được phát triển");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để xóa loại thuốc
            MessageBox.Show("Chức năng xóa loại thuốc đang được phát triển");
        }

        private void UCQuanLyLoaiThuoc_Load(object sender, EventArgs e)
        {

        }
    }
}