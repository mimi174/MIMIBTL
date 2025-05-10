using System;
using System.Drawing;
using System.Windows.Forms;

namespace N4_BTCM
{
    public partial class MainMenu : Form
    {
        private System.ComponentModel.IContainer components = null;

        private MenuStrip menuStrip;
        private ToolStripMenuItem qlThuocMenuItem;
        private ToolStripMenuItem loaiThuocMenuItem;
        private ToolStripMenuItem khachHangMenuItem;
        private ToolStripMenuItem nhanVienMenuItem;
        private ToolStripMenuItem hoaDonMenuItem;
        private ToolStripMenuItem thongKeMenuItem;
        private ToolStripMenuItem dangXuatMenuItem;
        private Panel panelContent;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.qlThuocMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loaiThuocMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.khachHangMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nhanVienMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hoaDonMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thongKeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dangXuatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelContent = new System.Windows.Forms.Panel();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.qlThuocMenuItem,
            this.loaiThuocMenuItem,
            this.khachHangMenuItem,
            this.nhanVienMenuItem,
            this.hoaDonMenuItem,
            this.thongKeMenuItem,
            this.dangXuatMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1920, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // qlThuocMenuItem
            // 
            this.qlThuocMenuItem.Name = "qlThuocMenuItem";
            this.qlThuocMenuItem.Size = new System.Drawing.Size(94, 20);
            this.qlThuocMenuItem.Text = "Quản lý thuốc";
            this.qlThuocMenuItem.Click += new System.EventHandler(this.qlThuocMenuItem_Click);
            // 
            // loaiThuocMenuItem
            // 
            this.loaiThuocMenuItem.Name = "loaiThuocMenuItem";
            this.loaiThuocMenuItem.Size = new System.Drawing.Size(75, 20);
            this.loaiThuocMenuItem.Text = "Loại thuốc";
            this.loaiThuocMenuItem.Click += new System.EventHandler(this.loaiThuocMenuItem_Click);
            // 
            // khachHangMenuItem
            // 
            this.khachHangMenuItem.Name = "khachHangMenuItem";
            this.khachHangMenuItem.Size = new System.Drawing.Size(82, 20);
            this.khachHangMenuItem.Text = "Khách hàng";
            this.khachHangMenuItem.Click += new System.EventHandler(this.khachHangMenuItem_Click);
            // 
            // nhanVienMenuItem
            // 
            this.nhanVienMenuItem.Name = "nhanVienMenuItem";
            this.nhanVienMenuItem.Size = new System.Drawing.Size(73, 20);
            this.nhanVienMenuItem.Text = "Nhân viên";
            this.nhanVienMenuItem.Click += new System.EventHandler(this.nhanVienMenuItem_Click);
            // 
            // hoaDonMenuItem
            // 
            this.hoaDonMenuItem.Name = "hoaDonMenuItem";
            this.hoaDonMenuItem.Size = new System.Drawing.Size(65, 20);
            this.hoaDonMenuItem.Text = "Hóa đơn";
            this.hoaDonMenuItem.Click += new System.EventHandler(this.hoaDonMenuItem_Click);
            // 
            // thongKeMenuItem
            // 
            this.thongKeMenuItem.Name = "thongKeMenuItem";
            this.thongKeMenuItem.Size = new System.Drawing.Size(122, 20);
            this.thongKeMenuItem.Text = "Báo cáo - Thống kê";
            this.thongKeMenuItem.Click += new System.EventHandler(this.thongKeMenuItem_Click);
            // 
            // dangXuatMenuItem
            // 
            this.dangXuatMenuItem.Name = "dangXuatMenuItem";
            this.dangXuatMenuItem.Size = new System.Drawing.Size(72, 20);
            this.dangXuatMenuItem.Text = "Đăng xuất";
            this.dangXuatMenuItem.Click += new System.EventHandler(this.dangXuatMenuItem_Click);
            // 
            // panelContent
            // 
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 24);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1920, 1037);
            this.panelContent.TabIndex = 0;
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1061);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainMenu";
            this.Text = "Hệ thống quản lý thuốc bảo vệ thực vật";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}