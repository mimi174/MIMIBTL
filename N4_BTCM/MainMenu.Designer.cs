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
        private ToolStripMenuItem qlNccMenuItem;
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
            this.qlNccMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.menuStrip.AutoSize = false;
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.qlThuocMenuItem,
            this.qlNccMenuItem,
            this.khachHangMenuItem,
            this.nhanVienMenuItem,
            this.hoaDonMenuItem,
            this.thongKeMenuItem,
            this.dangXuatMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(6, 6, 0, 6);
            this.menuStrip.Size = new System.Drawing.Size(1920, 57);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // qlThuocMenuItem
            // 
            this.qlThuocMenuItem.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qlThuocMenuItem.ForeColor = System.Drawing.Color.Teal;
            this.qlThuocMenuItem.Name = "qlThuocMenuItem";
            this.qlThuocMenuItem.Size = new System.Drawing.Size(144, 45);
            this.qlThuocMenuItem.Text = "Quản lý thuốc";
            this.qlThuocMenuItem.Click += new System.EventHandler(this.qlThuocMenuItem_Click);
            // 
            // qlNccMenuItem
            // 
            this.qlNccMenuItem.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qlNccMenuItem.ForeColor = System.Drawing.Color.Teal;
            this.qlNccMenuItem.Name = "qlNccMenuItem";
            this.qlNccMenuItem.Size = new System.Drawing.Size(142, 45);
            this.qlNccMenuItem.Text = "Nhà cung cấp";
            this.qlNccMenuItem.Click += new System.EventHandler(this.qlNccMenuItem_Click);
            // 
            // khachHangMenuItem
            // 
            this.khachHangMenuItem.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.khachHangMenuItem.ForeColor = System.Drawing.Color.Teal;
            this.khachHangMenuItem.Name = "khachHangMenuItem";
            this.khachHangMenuItem.Size = new System.Drawing.Size(125, 45);
            this.khachHangMenuItem.Text = "Khách hàng";
            this.khachHangMenuItem.Click += new System.EventHandler(this.khachHangMenuItem_Click);
            // 
            // nhanVienMenuItem
            // 
            this.nhanVienMenuItem.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nhanVienMenuItem.ForeColor = System.Drawing.Color.Teal;
            this.nhanVienMenuItem.Name = "nhanVienMenuItem";
            this.nhanVienMenuItem.Size = new System.Drawing.Size(112, 45);
            this.nhanVienMenuItem.Text = "Nhân viên";
            this.nhanVienMenuItem.Click += new System.EventHandler(this.nhanVienMenuItem_Click);
            // 
            // hoaDonMenuItem
            // 
            this.hoaDonMenuItem.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hoaDonMenuItem.ForeColor = System.Drawing.Color.Teal;
            this.hoaDonMenuItem.Name = "hoaDonMenuItem";
            this.hoaDonMenuItem.Size = new System.Drawing.Size(99, 45);
            this.hoaDonMenuItem.Text = "Hóa đơn";
            this.hoaDonMenuItem.Click += new System.EventHandler(this.hoaDonMenuItem_Click);
            // 
            // thongKeMenuItem
            // 
            this.thongKeMenuItem.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.thongKeMenuItem.ForeColor = System.Drawing.Color.Teal;
            this.thongKeMenuItem.Name = "thongKeMenuItem";
            this.thongKeMenuItem.Size = new System.Drawing.Size(188, 45);
            this.thongKeMenuItem.Text = "Báo cáo - Thống kê";
            this.thongKeMenuItem.Click += new System.EventHandler(this.thongKeMenuItem_Click);
            // 
            // dangXuatMenuItem
            // 
            this.dangXuatMenuItem.BackColor = System.Drawing.Color.Teal;
            this.dangXuatMenuItem.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dangXuatMenuItem.ForeColor = System.Drawing.Color.White;
            this.dangXuatMenuItem.Name = "dangXuatMenuItem";
            this.dangXuatMenuItem.Size = new System.Drawing.Size(113, 45);
            this.dangXuatMenuItem.Text = "Đăng xuất";
            this.dangXuatMenuItem.Click += new System.EventHandler(this.dangXuatMenuItem_Click);
            // 
            // panelContent
            // 
            this.panelContent.Location = new System.Drawing.Point(0, 60);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1920, 1061);
            this.panelContent.TabIndex = 0;
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1061);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainMenu";
            this.Text = "Hệ thống quản lý thuốc bảo vệ thực vật";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}