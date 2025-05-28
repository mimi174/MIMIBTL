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
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                switch (Login.LoggedInRoleID)
                {
                    case 1:
                        Application.Run(new MainMenu());
                        break;
                    case 2:
                        
                        break;
                    case 3:
                        Application.Run(new KhachHang());
                        break;
                    default:
                        MessageBox.Show("Không xác định quyền truy cập.");
                        break;
                }
            }
            else
            {
                Application.Exit();
            }
        }
    }
}