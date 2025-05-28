using N4_BTCM.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel; // Thêm nếu cần
using System.Data; // Thêm nếu cần
using System.Data.SqlClient; // Thêm nếu cần
using System.Drawing;
using System.IO; // Cần thiết cho xử lý file ảnh
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
// using static System.Windows.Forms.VisualStyles.VisualStyleElement; // Không cần thiết nếu không dùng

namespace N4_BTCM
{
    public partial class UCQuanLyKhachHang : UserControl
    {
        private QuanLyNguoiDung userManager = new QuanLyNguoiDung();
        private string _tempSelectedImagePath = ""; // Đường dẫn gốc của ảnh được chọn từ OpenFileDialog

        public UCQuanLyKhachHang()
        {
            InitializeComponent();
            SetupDataGridView(); // Khởi tạo DataGridView trước
            LoadCustomerData();  // Sau đó mới tải dữ liệu

            this.Load += new EventHandler(UCQuanLyKhachHang_Load); // Giữ lại Load event nếu cần logic Load khác
            dgvKhachHang.CellClick += dgvKhachHang_CellClick; // Đảm bảo sự kiện này được gán

            txtTimKiem.TextChanged += txtTimKiem_TextChanged;
            txtTimKiem.Enter += txtTimKiem_Enter;
            txtTimKiem.Leave += txtTimKiem_Leave;

            // Gán sự kiện cho PictureBox để chọn ảnh
            // Bạn cần đảm bảo PictureBox của bạn có tên là userImg trên form
            userImg.Click += userImg_Click;
        }

        private void UCQuanLyKhachHang_Load(object sender, EventArgs e)
        {
            // SetupDataGridView(); // Đã gọi trong constructor
            // LoadCustomerData();  // Đã gọi trong constructor
            txtTimKiem.Text = "Tìm kiếm";
            txtTimKiem.ForeColor = Color.Gray;
        }

        // Phương thức công khai để làm mới dữ liệu từ MainMenu
        public void RefreshData()
        {
            LoadCustomerData();
            ClearForm();
        }

        private void SetupDataGridView()
        {
            dgvKhachHang.Columns.Clear();
            dgvKhachHang.AutoGenerateColumns = false;
            dgvKhachHang.AllowUserToAddRows = false;
            dgvKhachHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKhachHang.RowTemplate.Height = 100; // Chiều cao hàng để hiển thị ảnh

            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Mã KH",
                DataPropertyName = "UserID",
                Name = "UserID"
            });

            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tên đăng nhập",
                DataPropertyName = "Username",
                Name = "Username"
            });

            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Họ tên",
                DataPropertyName = "Fullname",
                Name = "Fullname"
            });

            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Email",
                DataPropertyName = "Email",
                Name = "Email"
            });

            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "SĐT",
                DataPropertyName = "Phone",
                Name = "Phone"
            });

            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Địa chỉ",
                DataPropertyName = "Address",
                Name = "Address"
            });

            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ngày tạo",
                DataPropertyName = "CreatedAt",
                Name = "CreateAt"
            });

            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Trạng thái",
                DataPropertyName = "Status",
                Name = "Status"
            });

            // Thêm cột ImagePath ẩn để lưu đường dẫn tương đối từ DB
            DataGridViewTextBoxColumn imagePathCol = new DataGridViewTextBoxColumn();
            imagePathCol.Name = "ImagePath";
            imagePathCol.HeaderText = "Đường dẫn ảnh"; // Không hiển thị
            imagePathCol.DataPropertyName = "ImagePath";
            imagePathCol.Visible = false; // Ẩn cột này
            dgvKhachHang.Columns.Add(imagePathCol);
        }

        private void LoadCustomerData()
        {
            List<User> users = userManager.GetAllUsers();

            var dataToBind = users.Select(u => new
            {
                u.UserID,
                u.Username,
                u.Fullname,
                u.Email,
                u.Phone,
                u.Address,
                u.CreatedAt,
                Status = u.Status == UserStatus.Active ? "Hoạt động" : "Ngừng hoạt động",
                u.ImagePath // Giữ lại ImagePath ở đây
            }).ToList();

            dgvKhachHang.DataSource = dataToBind;

        }

        private void dgvKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvKhachHang.Rows[e.RowIndex];
                txtMaKH.Text = row.Cells["UserID"].Value?.ToString() ?? "";
                txtUsername.Text = row.Cells["Username"].Value?.ToString() ?? "";
                txtTenKH.Text = row.Cells["Fullname"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                txtSDT.Text = row.Cells["Phone"].Value?.ToString() ?? "";
                txtDiaChi.Text = row.Cells["Address"].Value?.ToString() ?? "";
                dtpNgayTao.Value = Convert.ToDateTime(row.Cells["CreateAt"].Value); // Đảm bảo control dtpNgayTao tồn tại

                string status = row.Cells["Status"].Value?.ToString() ?? "";
                if (status == "Ngừng hoạt động")
                {
                    MessageBox.Show("Khách hàng này đã ngừng hoạt động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Lấy ImagePath (ẩn) từ hàng đã chọn để hiển thị ảnh và cho việc sửa đổi
                string imagePathFromHiddenColumn = (row.DataBoundItem as dynamic)?.ImagePath;
                if (!string.IsNullOrEmpty(imagePathFromHiddenColumn))
                {
                    // Đường dẫn đầy đủ của ảnh trong thư mục ứng dụng
                    string fullPath = Path.Combine(Application.StartupPath, imagePathFromHiddenColumn);
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            // Tải ảnh vào PictureBox và cập nhật _tempSelectedImagePath
                            using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                            {
                                userImg.Image = Image.FromStream(stream);
                                _tempSelectedImagePath = fullPath; // Lưu đường dẫn đầy đủ của ảnh đang hiển thị
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi tải ảnh hiển thị: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            userImg.Image = null;
                            _tempSelectedImagePath = "";
                        }
                    }
                    else
                    {
                        userImg.Image = null;
                        _tempSelectedImagePath = "";
                    }
                }
                else
                {
                    userImg.Image = null;
                    _tempSelectedImagePath = "";
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateUserInput(out string errorMsg))
            {
                MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra trùng lặp Username, Email, Phone
            if (userManager.IsValueExists("Username", txtUsername.Text.Trim()))
            {
                MessageBox.Show("Tên đăng nhập đã được sử dụng. Vui lòng chọn giá trị khác.", "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (userManager.IsValueExists("Email", txtEmail.Text.Trim()))
            {
                MessageBox.Show("Email đã được sử dụng. Vui lòng chọn giá trị khác.", "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (userManager.IsValueExists("Phone", txtSDT.Text.Trim()))
            {
                MessageBox.Show("Số điện thoại đã được sử dụng. Vui lòng chọn giá trị khác.", "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string relativeImagePathForDB = null; // Mặc định là null nếu không có ảnh

            // Xử lý sao chép ảnh nếu có ảnh mới được chọn
            if (!string.IsNullOrEmpty(_tempSelectedImagePath) && File.Exists(_tempSelectedImagePath))
            {
                try
                {
                    string fileName = Path.GetFileName(_tempSelectedImagePath);
                    string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName); // Tên file duy nhất
                    string imageFolder = Path.Combine(Application.StartupPath, "Resource", "UserImg"); // Thư mục lưu ảnh User

                    if (!Directory.Exists(imageFolder))
                    {
                        Directory.CreateDirectory(imageFolder);
                    }

                    string fullDestinationPath = Path.Combine(imageFolder, newFileName);
                    File.Copy(_tempSelectedImagePath, fullDestinationPath, true); // Sao chép file, ghi đè nếu tồn tại
                    relativeImagePathForDB = Path.Combine("Resource", "UserImg", newFileName); // Đường dẫn tương đối
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sao chép ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            User newUser = new User
            {
                Username = txtUsername.Text.Trim(),
                Fullname = txtTenKH.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtSDT.Text.Trim(),
                Address = txtDiaChi.Text.Trim(),
                RoleID = 3, // RoleID cho khách hàng
                CreatedAt = DateTime.Now,
                Status = UserStatus.Active,
                ImagePath = relativeImagePathForDB // Lưu ImagePath
            };

            if (userManager.AddUser(newUser))
            {
                MessageBox.Show("Thêm khách hàng thành công.");
                LoadCustomerData();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Thêm khách hàng thất bại.");
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa.");
                return;
            }

            string statusText = dgvKhachHang.SelectedRows[0].Cells["Status"].Value?.ToString();
            if (statusText == "Ngừng hoạt động")
            {
                MessageBox.Show("Không thể cập nhật thông tin khách hàng đã ngừng hoạt động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateUserInput(out string errorMsg))
            {
                MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int currentUserId = int.Parse(txtMaKH.Text);

            // Kiểm tra trùng lặp Username, Email, Phone, nhưng bỏ qua bản thân người dùng đang sửa
            if (userManager.IsValueExists("Username", txtUsername.Text.Trim(), currentUserId))
            {
                MessageBox.Show("Tên đăng nhập đã được sử dụng bởi người dùng khác. Vui lòng chọn giá trị khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (userManager.IsValueExists("Email", txtEmail.Text.Trim(), currentUserId))
            {
                MessageBox.Show("Email đã được sử dụng bởi người dùng khác. Vui lòng chọn giá trị khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (userManager.IsValueExists("Phone", txtSDT.Text.Trim(), currentUserId))
            {
                MessageBox.Show("Số điện thoại đã được sử dụng bởi người dùng khác. Vui lòng chọn giá trị khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Lấy đường dẫn ảnh hiện tại từ đối tượng User trong list đã load
            string currentRelativeImagePath = userManager.GetAllUsers().FirstOrDefault(u => u.UserID == currentUserId)?.ImagePath;
            string newRelativeImagePathForDB = currentRelativeImagePath; // Mặc định giữ ảnh cũ

            // Xử lý ảnh mới
            if (!string.IsNullOrEmpty(_tempSelectedImagePath) && File.Exists(_tempSelectedImagePath))
            {
                // Nếu _tempSelectedImagePath khác với đường dẫn ảnh hiện tại của User này
                string tempSelectedFileName = Path.GetFileName(_tempSelectedImagePath);
                string currentFileName = string.IsNullOrEmpty(currentRelativeImagePath) ? "" : Path.GetFileName(currentRelativeImagePath);

                if (!string.Equals(tempSelectedFileName, currentFileName, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(currentRelativeImagePath))
                {
                    try
                    {
                        // Xóa ảnh cũ nếu tồn tại và khác với ảnh mới
                        if (!string.IsNullOrEmpty(currentRelativeImagePath))
                        {
                            string oldFullPath = Path.Combine(Application.StartupPath, currentRelativeImagePath);
                            if (File.Exists(oldFullPath) && !string.Equals(oldFullPath, _tempSelectedImagePath, StringComparison.OrdinalIgnoreCase))
                            {
                                File.Delete(oldFullPath);
                            }
                        }

                        string fileName = Path.GetFileName(_tempSelectedImagePath);
                        string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                        string imageFolder = Path.Combine(Application.StartupPath, "Resource", "UserImg");

                        if (!Directory.Exists(imageFolder))
                        {
                            Directory.CreateDirectory(imageFolder);
                        }

                        string fullDestinationPath = Path.Combine(imageFolder, newFileName);
                        File.Copy(_tempSelectedImagePath, fullDestinationPath, true);
                        newRelativeImagePathForDB = Path.Combine("Resource", "UserImg", newFileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi sao chép ảnh mới: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else // Nếu _tempSelectedImagePath rỗng, nghĩa là người dùng không chọn ảnh mới, hoặc đã xóa ảnh
            {
                // Nếu trước đó có ảnh và bây giờ PictureBox trống (người dùng muốn xóa ảnh)
                if (!string.IsNullOrEmpty(currentRelativeImagePath) && userImg.Image == null)
                {
                    string oldFullPath = Path.Combine(Application.StartupPath, currentRelativeImagePath);
                    if (File.Exists(oldFullPath))
                    {
                        File.Delete(oldFullPath);
                    }
                    newRelativeImagePathForDB = null; // Đặt đường dẫn ảnh thành null trong DB
                }
                // Nếu không có ảnh mới, và cũng không muốn xóa ảnh, thì giữ nguyên đường dẫn cũ
            }


            try
            {
                User updatedUser = new User
                {
                    UserID = currentUserId,
                    Username = txtUsername.Text.Trim(),
                    Fullname = txtTenKH.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtSDT.Text.Trim(),
                    Address = txtDiaChi.Text.Trim(),
                    RoleID = 3, // Giữ nguyên RoleID
                    Status = UserStatus.Active, // Giữ nguyên Status (hoặc có thể cập nhật thông qua nút Active/Deactive riêng)
                    ImagePath = newRelativeImagePathForDB // Cập nhật đường dẫn ảnh
                };

                if (userManager.UpdateUser(updatedUser))
                {
                    MessageBox.Show("Cập nhật khách hàng thành công.");
                    LoadCustomerData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvKhachHang.SelectedRows.Count == 0 || string.IsNullOrEmpty(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn khách hàng để vô hiệu hóa.");
                return;
            }

            string status = dgvKhachHang.SelectedRows[0].Cells["Status"].Value?.ToString();

            if (status == "Ngừng hoạt động")
            {
                MessageBox.Show("Tài khoản này đã ngừng hoạt động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = int.Parse(txtMaKH.Text);
            if (MessageBox.Show("Bạn có chắc chắn muốn vô hiệu hóa tài khoản khách hàng này?", "Xác nhận vô hiệu hóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Lấy đường dẫn ảnh hiện tại để xóa file ảnh vật lý nếu cần
                string currentRelativeImagePath = userManager.GetAllUsers().FirstOrDefault(u => u.UserID == userId)?.ImagePath;

                if (userManager.DeactivateUser(userId))
                {
                    // Tùy chọn: Xóa file ảnh vật lý khi tài khoản bị vô hiệu hóa
                    if (!string.IsNullOrEmpty(currentRelativeImagePath))
                    {
                        string fullPathToDelete = Path.Combine(Application.StartupPath, currentRelativeImagePath);
                        if (File.Exists(fullPathToDelete))
                        {
                            try
                            {
                                File.Delete(fullPathToDelete);
                                Console.WriteLine($"Đã xóa file ảnh khách hàng: {fullPathToDelete}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Lỗi khi xóa file ảnh khách hàng {fullPathToDelete}: {ex.Message}");
                            }
                        }
                    }

                    MessageBox.Show("Đã ngừng hoạt động tài khoản khách hàng.");
                    LoadCustomerData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Vô hiệu hóa thất bại.");
                }
            }
        }

        private void ClearForm()
        {
            txtMaKH.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtTenKH.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtSDT.Text = string.Empty;
            txtDiaChi.Text = string.Empty;
            userImg.Image = null; // Xóa ảnh trong PictureBox
            _tempSelectedImagePath = ""; // Xóa đường dẫn ảnh tạm thời đã chọn
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (dgvKhachHang.SelectedRows.Count == 0 || string.IsNullOrEmpty(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn một khách hàng trước khi reset mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string status = dgvKhachHang.SelectedRows[0].Cells["Status"].Value?.ToString();

            if (status == "Ngừng hoạt động")
            {
                MessageBox.Show("Không thể reset mật khẩu cho khách hàng đã ngừng hoạt động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = txtUsername.Text;
            string newPassword = username + "123";

            if (userManager.ResetPassword(int.Parse(txtMaKH.Text), newPassword))
            {
                MessageBox.Show($"Mật khẩu đã được reset thành '{newPassword}'", "Thông báo");
                LoadCustomerData();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Lỗi khi reset mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel đang được phát triển");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadCustomerData();
        }

        private bool ValidateUserInput(out string errorMessage)
        {
            errorMessage = "";

            string username = txtUsername.Text.Trim();
            string fullName = txtTenKH.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtSDT.Text.Trim();
            string address = txtDiaChi.Text.Trim();

            if (string.IsNullOrEmpty(username) // Thêm kiểm tra Username
                || string.IsNullOrEmpty(fullName)
                || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone)
                || string.IsNullOrEmpty(address))
            {
                errorMessage = "Vui lòng nhập đầy đủ tất cả các thông tin bắt buộc (Tên đăng nhập, Họ tên, Email, SĐT, Địa chỉ).";
                return false;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errorMessage = "Vui lòng nhập địa chỉ Email hợp lệ.";
                return false;
            }

            if (!Regex.IsMatch(phone, @"^0[0-9]{9}$"))
            {
                errorMessage = "Vui lòng nhập số điện thoại hợp lệ (ví dụ: 0xxxxxxxxx).";
                return false;
            }

            return true;
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            if (dgvKhachHang.SelectedRows.Count == 0 || string.IsNullOrEmpty(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn tài khoản khách hàng để kích hoạt.");
                return;
            }

            string status = dgvKhachHang.SelectedRows[0].Cells["Status"].Value?.ToString();

            if (status == "Hoạt động")
            {
                MessageBox.Show("Tài khoản này đã kích hoạt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = int.Parse(txtMaKH.Text);
            if (MessageBox.Show("Bạn có chắc chắn muốn kích hoạt tài khoản khách hàng này?", "Xác nhận kích hoạt", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (userManager.ActivateUser(userId))
                {
                    MessageBox.Show("Tài khoản khách hàng đã được kích hoạt.");
                    LoadCustomerData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Kích hoạt thất bại.");
                }
            }
        }

        private void search_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword) || keyword == "tìm kiếm")
            {
                LoadCustomerData(); // load lại tất cả
                return;
            }

            List<User> users = userManager.GetAllUsers(); // Tải lại dữ liệu gốc

            var filteredUsers = users.Where(u =>
                u.UserID.ToString().ToLower().Contains(keyword) ||
                (u.Username ?? "").ToLower().Contains(keyword) ||
                (u.Fullname ?? "").ToLower().Contains(keyword) ||
                (u.Email ?? "").ToLower().Contains(keyword) ||
                (u.Phone ?? "").ToLower().Contains(keyword) ||
                (u.Address ?? "").ToLower().Contains(keyword) ||
                u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss").ToLower().Contains(keyword) ||
                (u.Status == UserStatus.Active ? "hoạt động" : "ngừng hoạt động").ToLower().Contains(keyword)
            ).Select(u => new
            {
                u.UserID,
                u.Username,
                u.Fullname,
                u.Email,
                u.Phone,
                u.Address,
                u.CreatedAt,
                Status = u.Status == UserStatus.Active ? "Hoạt động" : "Ngừng hoạt động",
                u.ImagePath // Giữ lại ImagePath để hiển thị ảnh sau khi lọc
            }).ToList();

            dgvKhachHang.DataSource = filteredUsers;

            // Tải ảnh cho các hàng đã lọc
            foreach (DataGridViewRow row in dgvKhachHang.Rows)
            {
                if (row.DataBoundItem != null)
                {
                    string imagePath = (row.DataBoundItem as dynamic)?.ImagePath;
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        string fullPath = Path.Combine(Application.StartupPath, imagePath);
                        if (File.Exists(fullPath))
                        {
                            try
                            {
                                using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                                {
                                    row.Cells["Image"].Value = Image.FromStream(stream);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Lỗi khi tải ảnh cho kết quả tìm kiếm {fullPath}: {ex.Message}");
                                row.Cells["Image"].Value = null;
                            }
                        }
                        else
                        {
                            row.Cells["Image"].Value = null;
                        }
                    }
                    else
                    {
                        row.Cells["Image"].Value = null;
                    }
                }
            }
        }
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            search_Click(sender, e);
        }
        private void txtTimKiem_Enter(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Tìm kiếm")
            {
                txtTimKiem.Text = "";
                txtTimKiem.ForeColor = Color.Black;
            }
        }
        private void txtTimKiem_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
            {
                txtTimKiem.Text = "Tìm kiếm";
                txtTimKiem.ForeColor = Color.Gray;
            }
        }

        // Sự kiện click cho PictureBox để chọn ảnh người dùng
        private void userImg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh khách hàng";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _tempSelectedImagePath = ofd.FileName; // Lưu đường dẫn gốc của ảnh được chọn
                    try
                    {
                        // Hiển thị ảnh ngay lập tức lên PictureBox để xem trước
                        userImg.Image = Image.FromFile(_tempSelectedImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi hiển thị ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        userImg.Image = null;
                        _tempSelectedImagePath = ""; // Đặt lại nếu có lỗi
                    }
                }
            }
        }
    }
}