using OfficeOpenXml; // Giữ lại nếu bạn có kế hoạch dùng Export/Import Excel
using System;
using System.IO; // Cần thiết cho xử lý file ảnh
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs; // Giữ lại nếu bạn dùng CommonOpenFileDialog hoặc các tính năng khác
using N4_BTCM.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing; // Cần thiết cho Image

namespace N4_BTCM
{
    public partial class UCQuanLyNhanVien : UserControl
    {
        private QuanLyNguoiDung staffsManager = new QuanLyNguoiDung();
        private string _tempSelectedImagePath = ""; // Đường dẫn gốc của ảnh được chọn từ OpenFileDialog

        public UCQuanLyNhanVien()
        {
            InitializeComponent();
            SetupDataGridView(); // Khởi tạo DataGridView trước
            LoadStaffData();     // Sau đó mới tải dữ liệu nhân viên

            this.Load += new EventHandler(UCQuanLyNhanVien_Load);
            dgvNhanVien.CellClick += dgvNhanVien_CellClick;
            txtTimKiem.TextChanged += txtTimKiem_TextChanged;
            txtTimKiem.Enter += txtTimKiem_Enter;
            txtTimKiem.Leave += txtTimKiem_Leave;

            nvImg.Click += nvImg_Click;
        }

        private void UCQuanLyNhanVien_Load(object sender, EventArgs e)
        {
            txtTimKiem.Text = "Tìm kiếm";
            txtTimKiem.ForeColor = Color.Gray;
        }

        private void SetupDataGridView()
        {
            dgvNhanVien.Columns.Clear();
            dgvNhanVien.AutoGenerateColumns = false;
            dgvNhanVien.AllowUserToAddRows = false;
            dgvNhanVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNhanVien.RowTemplate.Height = 100; // Chiều cao hàng để hiển thị ảnh

            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Mã NV",
                DataPropertyName = "UserID",
                Name = "UserID"
            });

            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tên đăng nhập",
                DataPropertyName = "Username",
                Name = "Username"
            });

            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Họ tên",
                DataPropertyName = "Fullname",
                Name = "Fullname"
            });

            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Email",
                DataPropertyName = "Email",
                Name = "Email"
            });

            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "SĐT",
                DataPropertyName = "Phone",
                Name = "Phone"
            });

            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Địa chỉ",
                DataPropertyName = "Address",
                Name = "Address"
            });

            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ngày tạo",
                DataPropertyName = "CreatedAt",
                Name = "CreateAt"
            });

            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn
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
            dgvNhanVien.Columns.Add(imagePathCol);
        }


        private void LoadStaffData()
        {
            List<User> staffs = staffsManager.GetAllStaffs(); // Lấy tất cả nhân viên

            var dataToBind = staffs.Select(nv => new
            {
                nv.UserID,
                nv.Username,
                nv.Fullname,
                nv.Email,
                nv.Phone,
                nv.Address,
                nv.CreatedAt,
                Status = nv.Status == UserStatus.Active ? "Hoạt động" : "Ngừng hoạt động",
                nv.ImagePath // Giữ lại ImagePath ở đây
            }).ToList();

            dgvNhanVien.DataSource = dataToBind;      
        }

        private void dgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e) // Đổi từ CellContentClick
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvNhanVien.Rows[e.RowIndex];
                txtMaNV.Text = row.Cells["UserID"].Value?.ToString() ?? "";
                txtUsername.Text = row.Cells["Username"].Value?.ToString() ?? "";
                txtTenNV.Text = row.Cells["Fullname"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                txtSDT.Text = row.Cells["Phone"].Value?.ToString() ?? "";
                txtDiaChi.Text = row.Cells["Address"].Value?.ToString() ?? "";
                dtpNgayTao.Value = Convert.ToDateTime(row.Cells["CreateAt"].Value); // Đảm bảo control dtpNgayTao tồn tại

                string status = row.Cells["Status"].Value?.ToString() ?? "";
                if (status == "Ngừng hoạt động")
                {
                    MessageBox.Show("Nhân viên này đã ngừng hoạt động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                                nvImg.Image = Image.FromStream(stream);
                                _tempSelectedImagePath = fullPath; // Lưu đường dẫn đầy đủ của ảnh đang hiển thị
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi tải ảnh hiển thị: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            nvImg.Image = null;
                            _tempSelectedImagePath = "";
                        }
                    }
                    else
                    {
                        nvImg.Image = null;
                        _tempSelectedImagePath = "";
                    }
                }
                else
                {
                    nvImg.Image = null;
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

            if (staffsManager.IsValueExists("Username", txtUsername.Text.Trim()))
            {
                MessageBox.Show("Tên đăng nhập đã được sử dụng. Vui lòng chọn giá trị khác.", "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (staffsManager.IsValueExists("Email", txtEmail.Text.Trim()))
            {
                MessageBox.Show("Email đã được sử dụng. Vui lòng chọn giá trị khác.", "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (staffsManager.IsValueExists("Phone", txtSDT.Text.Trim()))
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
                    string imageFolder = Path.Combine(Application.StartupPath, "Resource", "UserImg"); // Thư mục lưu ảnh User/Staff

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
                Fullname = txtTenNV.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtSDT.Text.Trim(),
                Address = txtDiaChi.Text.Trim(),
                RoleID = 2, // RoleID cho nhân viên
                CreatedAt = DateTime.Now,
                Status = UserStatus.Active,
                ImagePath = relativeImagePathForDB // Lưu ImagePath
            };

            if (staffsManager.AddUser(newUser))
            {
                MessageBox.Show("Thêm nhân viên thành công.");
                LoadStaffData(); // Cập nhật lại DataGridView
                ClearForm();
            }
            else
            {
                MessageBox.Show("Thêm nhân viên thất bại.");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadStaffData(); // Làm mới DataGridView
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa.");
                return;
            }

            string statusText = dgvNhanVien.SelectedRows[0].Cells["Status"].Value?.ToString();
            if (statusText == "Ngừng hoạt động")
            {
                MessageBox.Show("Không thể cập nhật thông tin nhân viên đã ngừng hoạt động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateUserInput(out string errorMsg))
            {
                MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int currentUserId = int.Parse(txtMaNV.Text);

            // Kiểm tra trùng lặp Username, Email, Phone, nhưng bỏ qua bản thân người dùng đang sửa
            if (staffsManager.IsValueExists("Username", txtUsername.Text.Trim(), currentUserId))
            {
                MessageBox.Show("Tên đăng nhập đã được sử dụng bởi người dùng khác. Vui lòng chọn giá trị khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (staffsManager.IsValueExists("Email", txtEmail.Text.Trim(), currentUserId))
            {
                MessageBox.Show("Email đã được sử dụng bởi người dùng khác. Vui lòng chọn giá trị khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (staffsManager.IsValueExists("Phone", txtSDT.Text.Trim(), currentUserId))
            {
                MessageBox.Show("Số điện thoại đã được sử dụng bởi người dùng khác. Vui lòng chọn giá trị khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lấy đường dẫn ảnh hiện tại từ đối tượng User trong list đã load
            string currentRelativeImagePath = staffsManager.GetAllStaffs().FirstOrDefault(u => u.UserID == currentUserId)?.ImagePath;
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
                if (!string.IsNullOrEmpty(currentRelativeImagePath) && nvImg.Image == null)
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
                    Fullname = txtTenNV.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtSDT.Text.Trim(),
                    Address = txtDiaChi.Text.Trim(),
                    RoleID = 2, // Giữ nguyên RoleID nhân viên
                    Status = UserStatus.Active, // Giữ nguyên Status (hoặc có thể cập nhật thông qua nút Active/Deactive riêng)
                    ImagePath = newRelativeImagePathForDB // Cập nhật đường dẫn ảnh
                };

                // staffsManager.UpdateUser(updatedUser)
                if (staffsManager.UpdateUser(updatedUser))
                {
                    MessageBox.Show("Cập nhật nhân viên thành công.");
                    LoadStaffData(); // Cập nhật lại DataGridView
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e) // Đây là chức năng Deactive User
        {
            if (dgvNhanVien.SelectedRows.Count == 0 || string.IsNullOrEmpty(txtMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn nhân viên để vô hiệu hóa.");
                return;
            }

            string status = dgvNhanVien.SelectedRows[0].Cells["Status"].Value?.ToString();

            if (status == "Ngừng hoạt động")
            {
                MessageBox.Show("Tài khoản này đã ngừng hoạt động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = int.Parse(txtMaNV.Text);
            if (MessageBox.Show("Bạn có chắc chắn muốn vô hiệu hóa tài khoản nhân viên này?", "Xác nhận vô hiệu hóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Lấy đường dẫn ảnh hiện tại để xóa file ảnh vật lý nếu cần
                string currentRelativeImagePath = staffsManager.GetAllStaffs().FirstOrDefault(u => u.UserID == userId)?.ImagePath;

                if (staffsManager.DeactivateUser(userId))
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
                                Console.WriteLine($"Đã xóa file ảnh nhân viên: {fullPathToDelete}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Lỗi khi xóa file ảnh nhân viên {fullPathToDelete}: {ex.Message}");
                            }
                        }
                    }

                    MessageBox.Show("Đã ngừng hoạt động tài khoản nhân viên.");
                    LoadStaffData(); // Cập nhật lại DataGridView
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Vô hiệu hóa thất bại.");
                }
            }
        }

        private void btnNhapExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng nhập Excel đang được phát triển");
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel đang được phát triển");
        }

        private void ClearForm()
        {
            txtMaNV.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtTenNV.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtSDT.Text = string.Empty;
            txtDiaChi.Text = string.Empty;
            // Xóa ảnh trong PictureBox và đường dẫn tạm thời
            nvImg.Image = null;
            _tempSelectedImagePath = "";
        }

        private bool ValidateUserInput(out string errorMessage)
        {
            errorMessage = "";

            string username = txtUsername.Text.Trim();
            string fullName = txtTenNV.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtSDT.Text.Trim();
            string address = txtDiaChi.Text.Trim();

            if (string.IsNullOrEmpty(username)
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

        private void search_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword) || keyword == "tìm kiếm")
            {
                LoadStaffData(); // load lại tất cả
                return;
            }

            List<User> staffs = staffsManager.GetAllStaffs(); // Tải lại dữ liệu gốc

            var filteredUsers = staffs.Where(nv =>
                nv.UserID.ToString().ToLower().Contains(keyword) ||
                (nv.Username ?? "").ToLower().Contains(keyword) ||
                (nv.Fullname ?? "").ToLower().Contains(keyword) ||
                (nv.Email ?? "").ToLower().Contains(keyword) ||
                (nv.Phone ?? "").ToLower().Contains(keyword) ||
                (nv.Address ?? "").ToLower().Contains(keyword) ||
                nv.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss").ToLower().Contains(keyword) ||
                (nv.Status == UserStatus.Active ? "hoạt động" : "ngừng hoạt động").ToLower().Contains(keyword)
            ).Select(nv => new
            {
                nv.UserID,
                nv.Username,
                nv.Fullname,
                nv.Email,
                nv.Phone,
                nv.Address,
                nv.CreatedAt,
                Status = nv.Status == UserStatus.Active ? "Hoạt động" : "Ngừng hoạt động",
                nv.ImagePath // Giữ lại ImagePath để hiển thị ảnh sau khi lọc
            }).ToList();

            dgvNhanVien.DataSource = filteredUsers;

            // Tải ảnh cho các hàng đã lọc
            foreach (DataGridViewRow row in dgvNhanVien.Rows)
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (dgvNhanVien.SelectedRows.Count == 0 || string.IsNullOrEmpty(txtMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn một nhân viên trước khi reset mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string status = dgvNhanVien.SelectedRows[0].Cells["Status"].Value?.ToString();

            if (status == "Ngừng hoạt động")
            {
                MessageBox.Show("Không thể reset mật khẩu cho nhân viên đã ngừng hoạt động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = txtUsername.Text;
            string newPassword = username + "123";

            if (staffsManager.ResetPassword(int.Parse(txtMaNV.Text), newPassword))
            {
                MessageBox.Show($"Mật khẩu đã được reset thành '{newPassword}'", "Thông báo");
                LoadStaffData();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Lỗi khi reset mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            if (dgvNhanVien.SelectedRows.Count == 0 || string.IsNullOrEmpty(txtMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn tài khoản nhân viên để kích hoạt.");
                return;
            }

            string status = dgvNhanVien.SelectedRows[0].Cells["Status"].Value?.ToString();

            if (status == "Hoạt động")
            {
                MessageBox.Show("Tài khoản này đã kích hoạt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = int.Parse(txtMaNV.Text);
            if (MessageBox.Show("Bạn có chắc chắn muốn kích hoạt tài khoản nhân viên này?", "Xác nhận kích hoạt", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (staffsManager.ActivateUser(userId))
                {
                    MessageBox.Show("Tài khoản nhân viên đã được kích hoạt.");
                    LoadStaffData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Kích hoạt thất bại.");
                }
            }
        }

        // Sự kiện click cho PictureBox chọn ảnh nhân viên
        private void nvImg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh nhân viên";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _tempSelectedImagePath = ofd.FileName; // Lưu đường dẫn gốc của ảnh được chọn
                    try
                    {
                        // Hiển thị ảnh ngay lập tức lên PictureBox để xem trước
                        nvImg.Image = Image.FromFile(_tempSelectedImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi hiển thị ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        nvImg.Image = null;
                        _tempSelectedImagePath = ""; // Đặt lại nếu có lỗi
                    }
                }
            }
        }
    }
}