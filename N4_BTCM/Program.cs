// File: N4_BTCM/Program.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N4_BTCM
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Bắt đầu với form Login
            Login loginForm = new Login();

            // Hiển thị form Login dưới dạng Dialog
            // ShowDialog() chặn luồng cho đến khi form Login đóng
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Nếu login thành công (DialogResult.OK được trả về từ form Login)
                // thì hiển thị form MainMenu
                Application.Run(new MainMenu());
            }
            else
            {
                // Nếu login không thành công hoặc người dùng đóng form Login mà không phải OK
                // thì ứng dụng sẽ thoát
                Application.Exit();
            }
        }
    }
}