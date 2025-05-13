using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions; // Thêm directive này để sử dụng Regex

namespace N4_BTCM
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide(); // Ẩn form đăng ký
            Login loginForm = new Login();
            loginForm.ShowDialog();
            this.Close(); // Đóng hẳn form đăng ký sau khi login form đóng lại
        }

        private void Register_Load(object sender, EventArgs e)
        {
            // Có thể thêm logic khởi tạo khi form load nếu cần
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            // 1. Kiểm tra dữ liệu đầu vào: Tất cả các trường phải được điền
            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tất cả các thông tin bắt buộc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra định dạng Email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ Email hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Kiểm tra độ dài mật khẩu
            if (password.Length < 8) 
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phonePattern = @"^0[0-9]{9}$";
            if (!Regex.IsMatch(phone, phonePattern))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại hợp lệ (ví dụ: 0xxxxxxxxxx).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // mật khẩu
            string hashedPassword = PasswordHasher.HashPassword(password);

            DBConnection db = new DBConnection();
            SqlConnection conn = db.GetConnection();

            if (conn == null)
            {
                return; // Lỗi kết nối đã được xử lý trong GetConnection
            }

            try
            {
                // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                SqlCommand checkCmd = new SqlCommand(checkUserQuery, conn);
                checkCmd.Parameters.AddWithValue("@Username", username);
                int userCount = (int)checkCmd.ExecuteScalar();

                if (userCount > 0)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.", "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra xem Email đã tồn tại chưa
                string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                SqlCommand checkEmailCmd = new SqlCommand(checkEmailQuery, conn);
                checkEmailCmd.Parameters.AddWithValue("@Email", email);
                int emailCount = (int)checkEmailCmd.ExecuteScalar();

                if (emailCount > 0)
                {
                    MessageBox.Show("Địa chỉ Email này đã được sử dụng bởi một tài khoản khác.", "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Kiểm tra xem Số điện thoại đã tồn tại chưa
                string checkPhoneQuery = "SELECT COUNT(*) FROM Users WHERE Phone = @Phone";
                SqlCommand checkPhoneCmd = new SqlCommand(checkPhoneQuery, conn);
                checkPhoneCmd.Parameters.AddWithValue("@Phone", phone);
                int phoneCount = (int)checkPhoneCmd.ExecuteScalar();

                if (phoneCount > 0)
                {
                    MessageBox.Show("Số điện thoại này đã được sử dụng bởi một tài khoản khác.", "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                // Insert người dùng mới vào cơ sở dữ liệu
                string insertQuery = @"INSERT INTO Users (Username, PasswordHash, FullName, Email, Phone, Address, RoleID, CreatedAt)
                                       VALUES (@Username, @PasswordHash, @FullName, @Email, @Phone, @Address, @RoleID, GETDATE())";
                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);

                insertCmd.Parameters.AddWithValue("@Username", username);
                insertCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                insertCmd.Parameters.AddWithValue("@FullName", fullName);
                insertCmd.Parameters.AddWithValue("@Email", email);
                insertCmd.Parameters.AddWithValue("@Phone", phone);
                insertCmd.Parameters.AddWithValue("@Address", address);
                insertCmd.Parameters.AddWithValue("@RoleID", 3); // Mặc định RoleID = 3 (vai trò khách hàng)

                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Đăng ký tài khoản thành công! Vui lòng đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    Login loginForm = new Login();
                    loginForm.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Đăng ký tài khoản thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng ký tài khoản: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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