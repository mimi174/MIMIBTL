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
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            LoadQuanLyThuoc(); // Gọi hàm này để hiển thị UCQuanLyThuoc mặc định
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            LoadQuanLyThuoc();
        }

        private void LoadQuanLyThuoc()
        {
            this.panelContent.Controls.Clear();
            var uc = new UCQuanLyThuoc();
            uc.Dock = DockStyle.Fill;
            this.panelContent.Controls.Add(uc);
        }
        private void qlThuocMenuItem_Click(object sender, EventArgs e)
        {
            this.panelContent.Controls.Clear();
            var uc = new UCQuanLyThuoc();
            uc.Dock = DockStyle.Fill;
            this.panelContent.Controls.Add(uc);
        }

        private void khachHangMenuItem_Click(object sender, EventArgs e)
        {
            this.panelContent.Controls.Clear();
            var uc = new UCQuanLyKhachHang();
            uc.Dock = DockStyle.Fill;
            this.panelContent.Controls.Add(uc);
        }

        private void nhanVienMenuItem_Click(object sender, EventArgs e)
        {
            this.panelContent.Controls.Clear();
            var uc = new UCQuanLyNhanVien();
            uc.Dock = DockStyle.Fill;
            this.panelContent.Controls.Add(uc);
        }

        private void hoaDonMenuItem_Click(object sender, EventArgs e)
        {
            this.panelContent.Controls.Clear();
            var uc = new UCQuanLyHoaDon();
            uc.Dock = DockStyle.Fill;
            this.panelContent.Controls.Add(uc);
        }

        private void thongKeMenuItem_Click(object sender, EventArgs e)
        {
            this.panelContent.Controls.Clear();
            var uc = new UCThongKe();
            uc.Dock = DockStyle.Fill;
            this.panelContent.Controls.Add(uc);
        }

        private void dangXuatMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide(); // Ẩn MainMenu trước
            Login loginForm = new Login();
            loginForm.ShowDialog(); // Khi Login form tắt (đăng nhập lại hoặc cancel)
            this.Close(); // Đóng MainMenu
        }

        private void qlNccMenuItem_Click(object sender, EventArgs e)
        {
            this.panelContent.Controls.Clear();
            var uc = new UCQuanLyLoaiThuoc();
            uc.Dock = DockStyle.Fill;
            this.panelContent.Controls.Add(uc);
        }
    }
}