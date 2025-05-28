// File: N4_BTCM/Login.cs
using N4_BTCM.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; // Thêm directive này
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N4_BTCM
{
    public partial class Login : Form
    {
        public static int LoggedInUserID { get; private set; }
        public static string LoggedInFullName { get; private set; }
        public static int LoggedInRoleID { get; private set; }

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            DBConnection conn = new DBConnection();
            if (!conn.TestConnection())
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình.", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Bạn có thể muốn thoát ứng dụng nếu không kết nối được database
                // Application.Exit(); 
            }

            //adminPassword = "adminpass123";
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide(); // Ẩn form login
            Register registerForm = new Register();
            registerForm.ShowDialog();
            this.Close(); // Đóng hẳn sau khi form đăng ký đóng lại
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Đặt DialogResult là Cancel khi thoát
            this.Close(); // Đóng form Login
        }

        // Thêm phương thức xử lý sự kiện cho nút Đăng nhập
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DBConnection db = new DBConnection();
            SqlConnection conn = db.GetConnection();

            if (conn == null)
            {
                return;
            }

            try
            {
                string query = "SELECT UserID, PasswordHash, RoleID, FullName, Status FROM Users WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string storedPasswordHash = reader["PasswordHash"].ToString();
                    int roleId = Convert.ToInt32(reader["RoleID"]);
                    string fullName = reader["FullName"].ToString();
                    int status = Convert.ToInt32(reader["Status"]);

                    if (status == 0)
                    {
                        MessageBox.Show("Tài khoản của bạn đã bị ngừng hoạt động. Vui lòng liên hệ quản trị viên.", "Tài khoản bị khóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (PasswordHasher.VerifyPassword(password, storedPasswordHash))
                    {
                        LoggedInUserID = Convert.ToInt32(reader["UserID"]);
                        LoggedInFullName = fullName;
                        LoggedInRoleID = roleId;
                        
                        this.Hide();

                        if (roleId == 1)
                        {
                            MessageBox.Show($"Chào mừng Quản trị viên, {fullName}.", "Đăng nhập thành công");
                            MainMenu adminForm = new MainMenu();
                            adminForm.ShowDialog();
                            this.DialogResult = DialogResult.OK;

                        }
                        else if (roleId == 2)
                        {
                            MessageBox.Show($"Đăng nhập thành công! Chào mừng {fullName}.\nHệ thống nhân viên đang được phát triển, vui lòng thử lại sau!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (roleId == 3)
                        {
                            MessageBox.Show($"Chào mừng khách hàng, {fullName}.", "Đăng nhập thành công");
                            KhachHang custForm = new KhachHang();
                            custForm.ShowDialog();
                            this.DialogResult = DialogResult.OK;

                        }

                        this.Close(); // Đóng form Login sau khi form chính kết thúc
                    }
                    else
                    {
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
    }