// File: N4_BTCM/UCQuanLyLoaiThuoc.cs
using N4_BTCM.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; // Thêm namespace này
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; // Thêm namespace này
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N4_BTCM
{
    public partial class UCQuanLyLoaiThuoc : UserControl
    {
        private QuanLySP quanLySP = new QuanLySP();
        private List<Suppliers> suppliers = new List<Suppliers>();
        private string _tempSelectedImagePath = ""; // Đường dẫn gốc của ảnh được chọn từ OpenFileDialog

        public UCQuanLyLoaiThuoc()
        {
            InitializeComponent();
            InitializeDataGridView(); // Khởi tạo DataGridView trước
            LoadNhaCungCap();       // Sau đó mới tải dữ liệu
            txtTimKiem.Text = "Tìm kiếm";
            txtTimKiem.ForeColor = Color.Gray;
            txtTimKiem.TextChanged += txtTimKiem_TextChanged;
            txtTimKiem.Enter += txtTimKiem_Enter;
            txtTimKiem.Leave += txtTimKiem_Leave;

            // Gán sự kiện cho PictureBox để chọn ảnh
            // Bạn cần đảm bảo PictureBox của bạn có tên là supplierImg hoặc thay đổi tên này
            supplierImg.Click += supplierImg_Click;
        }

        // Phương thức công khai để làm mới dữ liệu từ MainMenu
        public void RefreshData()
        {
            LoadNhaCungCap();
            ClearForm();
        }

        private void InitializeDataGridView()
        {
            dgvNcc.Columns.Clear();
            dgvNcc.AutoGenerateColumns = false;
            dgvNcc.AllowUserToAddRows = false;
            dgvNcc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNcc.RowTemplate.Height = 100; // Chiều cao hàng để hiển thị ảnh tốt hơn

            // Các cột hiện có của nhà cung cấp
            DataGridViewTextBoxColumn idCol = new DataGridViewTextBoxColumn();
            idCol.Name = "ID";
            idCol.HeaderText = "ID";
            idCol.DataPropertyName = "ID";
            dgvNcc.Columns.Add(idCol);

            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.Name = "TenNCC";
            nameCol.HeaderText = "Tên NCC";
            nameCol.DataPropertyName = "TenNCC";
            dgvNcc.Columns.Add(nameCol);

            DataGridViewTextBoxColumn phoneCol = new DataGridViewTextBoxColumn();
            phoneCol.Name = "SDT";
            phoneCol.HeaderText = "Điện thoại";
            phoneCol.DataPropertyName = "SDT";
            dgvNcc.Columns.Add(phoneCol);

            DataGridViewTextBoxColumn emailCol = new DataGridViewTextBoxColumn();
            emailCol.Name = "Email";
            emailCol.HeaderText = "Email";
            emailCol.DataPropertyName = "Email";
            dgvNcc.Columns.Add(emailCol);

            DataGridViewTextBoxColumn addressCol = new DataGridViewTextBoxColumn();
            addressCol.Name = "DiaChi";
            addressCol.HeaderText = "Địa chỉ";
            addressCol.DataPropertyName = "DiaChi";
            dgvNcc.Columns.Add(addressCol);

            // Thêm cột ImagePath ẩn để lưu đường dẫn tương đối từ DB
            DataGridViewTextBoxColumn imagePathCol = new DataGridViewTextBoxColumn();
            imagePathCol.Name = "ImagePath";
            imagePathCol.HeaderText = "Đường dẫn ảnh"; // Không cần hiển thị
            imagePathCol.DataPropertyName = "ImagePath";
            imagePathCol.Visible = false; // Ẩn cột này
            dgvNcc.Columns.Add(imagePathCol);
        }

        private void dgvNcc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Đảm bảo người dùng click vào một hàng hợp lệ
            {
                DataGridViewRow row = dgvNcc.Rows[e.RowIndex];
                txtID.Text = row.Cells["ID"].Value?.ToString() ?? "";
                txtTenNCC.Text = row.Cells["TenNCC"].Value?.ToString() ?? "";
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString() ?? "";
                txtSDT.Text = row.Cells["SDT"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";

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
                                supplierImg.Image = Image.FromStream(stream);
                                _tempSelectedImagePath = fullPath; // Lưu đường dẫn đầy đủ của ảnh đang hiển thị
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi tải ảnh hiển thị: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            supplierImg.Image = null;
                            _tempSelectedImagePath = "";
                        }
                    }
                    else
                    {
                        supplierImg.Image = null;
                        _tempSelectedImagePath = "";
                    }
                }
                else
                {
                    supplierImg.Image = null;
                    _tempSelectedImagePath = "";
                }
            }
        }


        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^\d{10}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        private void LoadNhaCungCap()
        {
            suppliers = quanLySP.GetAllSuppliers();

            // Sử dụng anonymous type để hiển thị dữ liệu nếu cần, bao gồm ImagePath
            var dataToBind = suppliers.Select(s => new
            {
                s.ID,
                s.TenNCC,
                s.SDT,
                s.Email,
                s.DiaChi,
                s.ImagePath
            }).ToList();

            dgvNcc.DataSource = dataToBind;
        }

        private void ClearForm()
        {
            txtID.Text = "";
            txtTenNCC.Text = "";
            txtDiaChi.Text = "";
            txtSDT.Text = "";
            txtEmail.Text = "";
            supplierImg.Image = null; // Xóa ảnh trong PictureBox
            _tempSelectedImagePath = ""; // Xóa đường dẫn ảnh tạm thời đã chọn
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTenNCC.Text) ||
                    string.IsNullOrWhiteSpace(txtDiaChi.Text) ||
                    string.IsNullOrWhiteSpace(txtSDT.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy lại danh sách để kiểm tra tên trùng
                suppliers = quanLySP.GetAllSuppliers();
                if (suppliers.Any(s => s.TenNCC.Equals(txtTenNCC.Text, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Tên nhà cung cấp đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!IsValidEmail(txtEmail.Text))
                {
                    MessageBox.Show("Email không đúng định dạng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!IsValidPhoneNumber(txtSDT.Text))
                {
                    MessageBox.Show("Số điện thoại phải có đúng 10 chữ số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string relativeImagePathForDB = ""; // Đường dẫn tương đối sẽ lưu vào DB

                // Xử lý sao chép ảnh nếu có ảnh mới được chọn
                if (!string.IsNullOrEmpty(_tempSelectedImagePath) && File.Exists(_tempSelectedImagePath))
                {
                    try
                    {
                        string fileName = Path.GetFileName(_tempSelectedImagePath);
                        string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName); // Tên file duy nhất
                        string imageFolder = Path.Combine(Application.StartupPath, "Resource", "SupplierImg"); // Thư mục lưu ảnh NCC

                        if (!Directory.Exists(imageFolder))
                        {
                            Directory.CreateDirectory(imageFolder);
                        }

                        string fullDestinationPath = Path.Combine(imageFolder, newFileName);
                        File.Copy(_tempSelectedImagePath, fullDestinationPath, true); // Sao chép file, ghi đè nếu tồn tại
                        relativeImagePathForDB = Path.Combine("Resource", "SupplierImg", newFileName); // Đường dẫn tương đối
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi sao chép ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                Suppliers nhaCungCap = new Suppliers()
                {
                    TenNCC = txtTenNCC.Text,
                    DiaChi = txtDiaChi.Text,
                    SDT = txtSDT.Text,  
                    Email = txtEmail.Text,
                    ImagePath = relativeImagePathForDB // Lưu đường dẫn ảnh đã xử lý
                };

                bool result = quanLySP.AddNcc(nhaCungCap);

                if (result)
                {
                    MessageBox.Show("Thêm nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadNhaCungCap();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Thêm nhà cung cấp thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e) // Đây là nút "Sửa"
        {
            try
            {
                if (dgvNcc.SelectedRows.Count == 0 || string.IsNullOrWhiteSpace(txtID.Text))
                {
                    MessageBox.Show("Vui lòng chọn một nhà cung cấp để cập nhật!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTenNCC.Text) ||
                    string.IsNullOrWhiteSpace(txtDiaChi.Text) ||
                    string.IsNullOrWhiteSpace(txtSDT.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra trùng tên khi sửa (không tính tên của chính NCC đang sửa)
                int currentId = int.Parse(txtID.Text);
                suppliers = quanLySP.GetAllSuppliers();
                if (suppliers.Any(s => s.TenNCC.Equals(txtTenNCC.Text, StringComparison.OrdinalIgnoreCase) && s.ID != currentId))
                {
                    MessageBox.Show("Tên nhà cung cấp đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!IsValidEmail(txtEmail.Text))
                {
                    MessageBox.Show("Email không đúng định dạng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!IsValidPhoneNumber(txtSDT.Text))
                {
                    MessageBox.Show("Số điện thoại phải có đúng 10 chữ số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy đường dẫn ảnh hiện tại từ đối tượng Suppliers trong list đã load
                string currentRelativeImagePath = suppliers.FirstOrDefault(s => s.ID == currentId)?.ImagePath;
                string newRelativeImagePathForDB = currentRelativeImagePath; // Mặc định giữ ảnh cũ

                // Xử lý ảnh mới
                if (!string.IsNullOrEmpty(_tempSelectedImagePath) && File.Exists(_tempSelectedImagePath))
                {
                    // Nếu _tempSelectedImagePath khác với đường dẫn ảnh hiện tại của NCC này
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
                            string imageFolder = Path.Combine(Application.StartupPath, "Resource", "SupplierImg");

                            if (!Directory.Exists(imageFolder))
                            {
                                Directory.CreateDirectory(imageFolder);
                            }

                            string fullDestinationPath = Path.Combine(imageFolder, newFileName);
                            File.Copy(_tempSelectedImagePath, fullDestinationPath, true);
                            newRelativeImagePathForDB = Path.Combine("Resource", "SupplierImg", newFileName);
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
                    if (!string.IsNullOrEmpty(currentRelativeImagePath) && supplierImg.Image == null)
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


                Suppliers nhaCungCap = new Suppliers()
                {
                    ID = currentId,
                    TenNCC = txtTenNCC.Text,
                    DiaChi = txtDiaChi.Text,
                    SDT = txtSDT.Text,
                    Email = txtEmail.Text,
                    ImagePath = newRelativeImagePathForDB // Cập nhật đường dẫn ảnh
                };

                bool result = quanLySP.UpdateNcc(nhaCungCap);

                if (result)
                {
                    MessageBox.Show("Cập nhật nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadNhaCungCap();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Cập nhật nhà cung cấp thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("ID nhà cung cấp không hợp lệ. Vui lòng chọn một nhà cung cấp từ danh sách.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadNhaCungCap(); // Làm mới DataGridView
        }


        private void search_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword) || keyword == "tìm kiếm")
            {
                LoadNhaCungCap(); // Tải lại toàn bộ nếu không có từ khóa
                return;
            }

            var filtered = suppliers.Where(s =>
                s.ID.ToString().ToLower().Contains(keyword) ||
                (!string.IsNullOrEmpty(s.TenNCC) && s.TenNCC.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(s.DiaChi) && s.DiaChi.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(s.SDT) && s.SDT.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(s.Email) && s.Email.ToLower().Contains(keyword))
            ).Select(s => new
            {
                s.ID,
                s.TenNCC,
                s.SDT,
                s.Email,
                s.DiaChi,
                s.ImagePath // Giữ lại ImagePath để hiển thị ảnh sau khi lọc
            }).ToList();

            dgvNcc.DataSource = filtered;

            // Tải ảnh cho các hàng đã lọc
            foreach (DataGridViewRow row in dgvNcc.Rows)
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

        // Sự kiện click cho PictureBox chọn ảnh
        private void supplierImg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh nhà cung cấp";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _tempSelectedImagePath = ofd.FileName; // Lưu đường dẫn gốc của ảnh được chọn
                    try
                    {
                        // Hiển thị ảnh ngay lập tức lên PictureBox để xem trước
                        supplierImg.Image = Image.FromFile(_tempSelectedImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi hiển thị ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        supplierImg.Image = null;
                        _tempSelectedImagePath = ""; // Đặt lại nếu có lỗi
                    }
                }
            }
        }
    }
}