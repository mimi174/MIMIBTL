using N4_BTCM.Controller;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace N4_BTCM
{
    public partial class UCQuanLyThuoc : UserControl
    {
        private QuanLySP productManager = new QuanLySP();
        private List<Product> products = new List<Product>();
        private List<Suppliers> suppliers = new List<Suppliers>();
        private string _tempSelectedImagePath = "";

        public void RefreshData()
        {
            LoadProducts();
        }
        public UCQuanLyThuoc()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadSuppliers(); // Nạp nhà cung cấp trước để ComboBox có dữ liệu
            LoadProducts();  // Sau đó nạp sản phẩm

            txtTimKiem.Text = "Tìm kiếm";
            txtTimKiem.ForeColor = Color.Gray;
            txtTimKiem.TextChanged += txtTimKiem_TextChanged;
            txtTimKiem.Enter += txtTimKiem_Enter;
            txtTimKiem.Leave += txtTimKiem_Leave;
            dgvThuoc.CellClick += dgvThuoc_CellClick; // Thay CellContentClick bằng CellClick để chọn toàn bộ ô
        }

        private void InitializeDataGridView()
        {
            dgvThuoc.Columns.Clear();
            dgvThuoc.AutoGenerateColumns = false;
            dgvThuoc.AllowUserToAddRows = false;
            dgvThuoc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvThuoc.RowTemplate.Height = 100; // Chiều cao hàng để hiển thị ảnh tốt hơn

            // ProductID Column
            DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn();
            idColumn.Name = "ProductID";
            idColumn.HeaderText = "ID";
            idColumn.DataPropertyName = "ProductID";
            dgvThuoc.Columns.Add(idColumn);

            // Name Column
            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.Name = "Name";
            nameColumn.HeaderText = "Tên Thuốc";
            nameColumn.DataPropertyName = "Name";
            dgvThuoc.Columns.Add(nameColumn);

            // Description Column
            DataGridViewTextBoxColumn descriptionColumn = new DataGridViewTextBoxColumn();
            descriptionColumn.Name = "Description";
            descriptionColumn.HeaderText = "Mô Tả";
            descriptionColumn.DataPropertyName = "Description";
            dgvThuoc.Columns.Add(descriptionColumn);

            // UnitPrice Column
            DataGridViewTextBoxColumn priceColumn = new DataGridViewTextBoxColumn();
            priceColumn.Name = "UnitPrice";
            priceColumn.HeaderText = "Giá";
            priceColumn.DataPropertyName = "UnitPrice";
            dgvThuoc.Columns.Add(priceColumn);

            // Unit Column
            DataGridViewTextBoxColumn unitColumn = new DataGridViewTextBoxColumn();
            unitColumn.Name = "Unit";
            unitColumn.HeaderText = "Đơn vị";
            unitColumn.DataPropertyName = "Unit";
            dgvThuoc.Columns.Add(unitColumn);

            // Quantity Column
            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn();
            quantityColumn.Name = "Quantity";
            quantityColumn.HeaderText = "Số Lượng";
            quantityColumn.DataPropertyName = "Quantity";
            dgvThuoc.Columns.Add(quantityColumn);

            // SupplierName Column (Display Supplier Name)
            DataGridViewTextBoxColumn supplierNameColumn = new DataGridViewTextBoxColumn();
            supplierNameColumn.Name = "SupplierName";
            supplierNameColumn.HeaderText = "NCC";
            supplierNameColumn.DataPropertyName = "SupplierName";
            dgvThuoc.Columns.Add(supplierNameColumn);

            // ImportDate Column
            DataGridViewTextBoxColumn importDateColumn = new DataGridViewTextBoxColumn();
            importDateColumn.Name = "ImportDate";
            importDateColumn.HeaderText = "Ngày Nhập";
            importDateColumn.DataPropertyName = "ImportDate";
            dgvThuoc.Columns.Add(importDateColumn);

            // Image Column (đây là cột sẽ hiển thị ảnh)
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "Image";
            imgCol.HeaderText = "Ảnh sản phẩm";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCol.Width = 100; // Cho ảnh to hơn
            dgvThuoc.Columns.Add(imgCol);

            // Cột ImagePath (ẩn) để giữ đường dẫn gốc cho mục đích internal
            DataGridViewTextBoxColumn imagePathCol = new DataGridViewTextBoxColumn();
            imagePathCol.Name = "ImagePath";
            imagePathCol.HeaderText = "Đường dẫn ảnh"; // Header này không cần hiển thị
            imagePathCol.DataPropertyName = "ImagePath"; // Kết nối với thuộc tính ImagePath của anonymous object
            imagePathCol.Visible = false; // Ẩn cột này đi
            dgvThuoc.Columns.Add(imagePathCol);
        }

        private void LoadProducts()
        {
            products = productManager.GetAllProducts();

            // Vẫn dùng Select để tạo anonymous type, bao gồm cả ImagePath
            var dataToBind = products.Select(p => new
            {
                p.ProductID,
                p.Name,
                p.Description,
                p.UnitPrice,
                p.Unit,
                p.Quantity,
                SupplierName = GetSupplierName(p.SupplierID), // Đảm bảo GetSupplierName hoạt động đúng
                p.ImportDate,
                p.ImagePath // Giữ ImagePath ở đây để có thể truy cập sau
            }).ToList();

            dgvThuoc.DataSource = dataToBind;

            // Sau khi DataSource được gán, lặp qua từng hàng để tải ảnh
            foreach (DataGridViewRow row in dgvThuoc.Rows)
            {
                if (row.DataBoundItem != null)
                {
                    // Truy cập ImagePath từ DataBoundItem (anonymous type)
                    string imagePath = (row.DataBoundItem as dynamic)?.ImagePath;

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        // Đường dẫn đầy đủ của ảnh trong thư mục ứng dụng
                        string fullPath = Path.Combine(Application.StartupPath, imagePath);
                        if (File.Exists(fullPath))
                        {
                            try
                            {
                                // Tạo bản sao của ảnh để tránh bị khóa file
                                using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                                {
                                    productImg.Image = Image.FromStream(stream); // Tải vào PictureBox
                                    row.Cells["Image"].Value = Image.FromStream(stream); // Gán vào DataGridView
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Lỗi khi tải ảnh từ đường dẫn {fullPath}: {ex.Message}");
                                row.Cells["Image"].Value = null; // Để trống nếu lỗi
                            }
                        }
                        else
                        {
                            Console.WriteLine($"File ảnh không tồn tại tại đường dẫn: {fullPath}");
                            row.Cells["Image"].Value = null; // Để trống nếu không tìm thấy file
                        }
                    }
                    else
                    {
                        row.Cells["Image"].Value = null; // Để trống nếu không có đường dẫn ảnh
                    }
                }
            }
        }

        private string GetSupplierName(int? supplierId)
        {
            if (supplierId.HasValue)
            {
                Suppliers supplier = suppliers.FirstOrDefault(s => s.ID == supplierId.Value);
                if (supplier != null)
                {
                    return supplier.TenNCC;
                }
            }
            return "Không xác định";
        }

        private void LoadSuppliers()
        {
            suppliers = productManager.GetAllSuppliers();
            cbbNcc.DataSource = suppliers;
            cbbNcc.DisplayMember = "TenNCC"; // Hiển thị tên nhà cung cấp
            cbbNcc.ValueMember = "ID";    // Giá trị thực tế là ID
            cbbNcc.SelectedIndex = -1; // Xóa lựa chọn mặc định ban đầu
        }

        private void dgvThuoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvThuoc.Rows[e.RowIndex];
                txtID.Text = row.Cells["ProductID"].Value?.ToString();
                txtTenSP.Text = row.Cells["Name"].Value?.ToString() ?? "";
                txtDescription.Text = row.Cells["Description"].Value?.ToString() ?? "";

                // Xử lý nudPrice và nudQuantity có thể ném FormatException nếu giá trị không đúng định dạng số
                try
                {
                    nudPrice.Value = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                }
                catch { nudPrice.Value = 0; } // Gán giá trị mặc định nếu lỗi

                cbbUnit.Text = row.Cells["Unit"].Value?.ToString() ?? "";

                try
                {
                    nudQuantity.Value = Convert.ToInt32(row.Cells["Quantity"].Value);
                }
                catch { nudQuantity.Value = 0; } // Gán giá trị mặc định nếu lỗi

                // Chọn nhà cung cấp trong ComboBox
                string supplierNameFromGrid = row.Cells["SupplierName"].Value?.ToString();
                if (!string.IsNullOrEmpty(supplierNameFromGrid))
                {
                    cbbNcc.SelectedValue = suppliers.FirstOrDefault(s => s.TenNCC == supplierNameFromGrid)?.ID;
                }
                else
                {
                    cbbNcc.SelectedIndex = -1; // Không chọn gì nếu không có NCC
                }

                dtpNgayNhap.Value = Convert.ToDateTime(row.Cells["ImportDate"].Value);

                // Lấy ImagePath (ẩn) từ hàng đã chọn để hiển thị ảnh và cho việc sửa đổi
                string imagePathFromHiddenColumn = (row.DataBoundItem as dynamic)?.ImagePath;
                if (!string.IsNullOrEmpty(imagePathFromHiddenColumn))
                {
                    string fullPath = Path.Combine(Application.StartupPath, imagePathFromHiddenColumn);
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            // Tải ảnh vào PictureBox và cập nhật _tempSelectedImagePath
                            using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                            {
                                productImg.Image = Image.FromStream(stream);
                                _tempSelectedImagePath = fullPath; // Lưu đường dẫn đầy đủ của ảnh đang hiển thị
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi tải ảnh hiển thị: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            productImg.Image = null;
                            _tempSelectedImagePath = "";
                        }
                    }
                    else
                    {
                        productImg.Image = null;
                        _tempSelectedImagePath = "";
                    }
                }
                else
                {
                    productImg.Image = null;
                    _tempSelectedImagePath = "";
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Validation (đã có)
            if (string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                string.IsNullOrWhiteSpace(txtDescription.Text) ||
                string.IsNullOrWhiteSpace(cbbUnit.Text) ||
                string.IsNullOrWhiteSpace(nudPrice.Text) ||
                string.IsNullOrWhiteSpace(nudQuantity.Text) ||
                cbbNcc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string relativeImagePathForDB = ""; // Đường dẫn tương đối sẽ lưu vào DB

            // Nếu người dùng đã chọn ảnh (selectedImagePath không rỗng)
            if (!string.IsNullOrEmpty(_tempSelectedImagePath))
            {
                try
                {
                    string fileName = Path.GetFileName(_tempSelectedImagePath);
                    string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName); // Tạo tên file duy nhất
                    string imageFolder = Path.Combine(Application.StartupPath, "Resource", "ProductImg");

                    if (!Directory.Exists(imageFolder))
                    {
                        Directory.CreateDirectory(imageFolder);
                    }

                    string fullDestinationPath = Path.Combine(imageFolder, newFileName);

                    File.Copy(_tempSelectedImagePath, fullDestinationPath, true); // Sao chép file, ghi đè nếu tồn tại
                    relativeImagePathForDB = Path.Combine("Resource", "ProductImg", newFileName); // Đường dẫn tương đối
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sao chép ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Dừng lại nếu không sao chép được ảnh
                }
            }

            Product newProduct = new Product
            {
                Name = txtTenSP.Text,
                Description = txtDescription.Text,
                UnitPrice = nudPrice.Value, // Lấy giá trị trực tiếp từ NumericUpDown
                Unit = cbbUnit.Text,
                Quantity = (int)nudQuantity.Value, // Lấy giá trị trực tiếp từ NumericUpDown
                SupplierID = cbbNcc.SelectedValue != null ? (int?)Convert.ToInt32(cbbNcc.SelectedValue) : null,
                ImportDate = dtpNgayNhap.Value,
                ImagePath = relativeImagePathForDB // Lưu đường dẫn tương đối đã được sao chép
            };

            if (productManager.AddProduct(newProduct))
            {
                MessageBox.Show("Thêm thuốc thành công");
                ClearInputFields();
                LoadProducts(); // Tải lại dữ liệu để cập nhật DataGridView
            }
            else
            {
                MessageBox.Show("Thêm thuốc thất bại");
            }
        }

        private void ClearInputFields()
        {
            txtID.Text = "";
            txtTenSP.Text = "";
            txtDescription.Text = "";
            nudPrice.Text = "1000";
            cbbUnit.Text = "";
            nudQuantity.Text = "0";
            cbbNcc.SelectedIndex = -1;
            dtpNgayNhap.Value = DateTime.Now;
            productImg.Image = null;
            _tempSelectedImagePath = "";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            LoadProducts(); // Tải lại toàn bộ dữ liệu (bao gồm cả lọc nếu có)
        }

        private void btnLuu_Click(object sender, EventArgs e) // Đây là nút "Sửa"
        {
            if (dgvThuoc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một thuốc để cập nhật", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int productId = Convert.ToInt32(txtID.Text); // Lấy ID từ textbox đã gán khi chọn hàng

            // Validation (đã có)
            if (string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                string.IsNullOrWhiteSpace(txtDescription.Text) ||
                string.IsNullOrWhiteSpace(cbbUnit.Text) ||
                string.IsNullOrWhiteSpace(nudPrice.Text) ||
                string.IsNullOrWhiteSpace(nudQuantity.Text) ||
                cbbNcc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lấy đường dẫn ảnh hiện tại từ DataBoundItem (ẩn)
            string currentRelativeImagePath = products.FirstOrDefault(p => p.ProductID == productId)?.ImagePath;
            string newRelativeImagePathForDB = currentRelativeImagePath; // Mặc định giữ ảnh cũ

            // Kiểm tra xem người dùng có chọn ảnh mới không
            if (!string.IsNullOrEmpty(_tempSelectedImagePath) && File.Exists(_tempSelectedImagePath))
            {
                // Nếu đường dẫn tạm thời khác với đường dẫn ảnh cũ đã lưu
                // Hoặc nếu ảnh cũ không tồn tại nhưng có ảnh mới được chọn
                string tempSelectedFileName = Path.GetFileName(_tempSelectedImagePath);
                string currentFileName = string.IsNullOrEmpty(currentRelativeImagePath) ? "" : Path.GetFileName(currentRelativeImagePath);

                // Chỉ sao chép nếu ảnh mới được chọn hoặc ảnh cũ không có/khác
                if (!string.Equals(tempSelectedFileName, currentFileName, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(currentRelativeImagePath))
                {
                    try
                    {
                        // Xóa ảnh cũ nếu nó tồn tại và là file khác (tránh xóa nhầm nếu ảnh mới trùng tên với ảnh cũ)
                        if (!string.IsNullOrEmpty(currentRelativeImagePath))
                        {
                            string oldFullPath = Path.Combine(Application.StartupPath, currentRelativeImagePath);
                            if (File.Exists(oldFullPath) && !string.Equals(oldFullPath, _tempSelectedImagePath, StringComparison.OrdinalIgnoreCase))
                            {
                                File.Delete(oldFullPath);
                            }
                        }

                        string fileName = Path.GetFileName(_tempSelectedImagePath);
                        string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName); // Tên file duy nhất
                        string imageFolder = Path.Combine(Application.StartupPath, "Resource", "ProductImg");

                        if (!Directory.Exists(imageFolder))
                        {
                            Directory.CreateDirectory(imageFolder);
                        }

                        string fullDestinationPath = Path.Combine(imageFolder, newFileName);
                        File.Copy(_tempSelectedImagePath, fullDestinationPath, true); // Sao chép file, ghi đè nếu tồn tại
                        newRelativeImagePathForDB = Path.Combine("Resource", "ProductImg", newFileName); // Đường dẫn tương đối mới
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
                // Nếu trước đó có ảnh và bây giờ không có ảnh nào được chọn (người dùng muốn xóa ảnh)
                if (!string.IsNullOrEmpty(currentRelativeImagePath) && productImg.Image == null)
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


            Product updatedProduct = new Product
            {
                ProductID = productId,
                Name = txtTenSP.Text,
                Description = txtDescription.Text,
                UnitPrice = nudPrice.Value,
                Unit = cbbUnit.Text,
                Quantity = (int)nudQuantity.Value,
                SupplierID = cbbNcc.SelectedValue != null ? (int?)Convert.ToInt32(cbbNcc.SelectedValue) : null,
                ImportDate = dtpNgayNhap.Value,
                ImagePath = newRelativeImagePathForDB // Cập nhật đường dẫn tương đối mới hoặc cũ
            };

            if (productManager.UpdateProduct(updatedProduct))
            {
                MessageBox.Show("Cập nhật thuốc thành công");
                ClearInputFields();
                LoadProducts(); // Tải lại dữ liệu để cập nhật DataGridView
            }
            else
            {
                MessageBox.Show("Cập nhật thuốc thất bại");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvThuoc.SelectedRows.Count > 0)
            {
                int productId = Convert.ToInt32(dgvThuoc.SelectedRows[0].Cells["ProductID"].Value);
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa thuốc này? (Thuốc sẽ được vô hiệu hóa bằng cách đặt số lượng về 0)", "Xác nhận xóa", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // Lấy đường dẫn ảnh hiện tại để xóa file ảnh vật lý nếu cần
                    string currentRelativeImagePath = products.FirstOrDefault(p => p.ProductID == productId)?.ImagePath;

                    if (productManager.DeleteProduct(productId)) // Hàm DeleteProduct sẽ đặt Quantity = 0
                    {
                        // Tùy chọn: Xóa file ảnh vật lý khi vô hiệu hóa sản phẩm
                        if (!string.IsNullOrEmpty(currentRelativeImagePath))
                        {
                            string fullPathToDelete = Path.Combine(Application.StartupPath, currentRelativeImagePath);
                            if (File.Exists(fullPathToDelete))
                            {
                                try
                                {
                                    File.Delete(fullPathToDelete);
                                    Console.WriteLine($"Đã xóa file ảnh: {fullPathToDelete}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Lỗi khi xóa file ảnh {fullPathToDelete}: {ex.Message}");
                                    // Có thể hiển thị MessageBox nếu muốn cảnh báo người dùng về lỗi xóa ảnh
                                }
                            }
                        }

                        MessageBox.Show("Vô hiệu hóa thuốc thành công (Số lượng đã được đặt thành 0)");
                        LoadProducts();
                        ClearInputFields();
                    }
                    else
                    {
                        MessageBox.Show("Vô hiệu hóa thuốc thất bại");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một thuốc để vô hiệu hóa");
            }
        }

        private void btnNhapExcel_Click(object sender, EventArgs e)
        {
            // Code nhập Excel, bạn có thể triển khai sau
            MessageBox.Show("Chức năng nhập Excel đang được phát triển");
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            // Code xuất Excel, bạn có thể triển khai sau
            MessageBox.Show("Chức năng xuất Excel đang được phát triển");
        }

        private void search_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword) || keyword == "tìm kiếm")
            {
                LoadProducts(); // Tải lại toàn bộ dữ liệu nếu không có từ khóa tìm kiếm
                return;
            }

            var filtered = products.Where(p =>
                p.ProductID.ToString().ToLower().Contains(keyword) ||
                (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(p.Unit) && p.Unit.ToLower().Contains(keyword)) ||
                p.UnitPrice.ToString().ToLower().Contains(keyword) ||
                p.Quantity.ToString().ToLower().Contains(keyword) ||
                (!string.IsNullOrEmpty(GetSupplierName(p.SupplierID)) && GetSupplierName(p.SupplierID).ToLower().Contains(keyword)) ||
                p.ImportDate.ToString("dd/MM/yyyy").ToLower().Contains(keyword)
            ).Select(p => new
            {
                p.ProductID,
                p.ImagePath, // Cần giữ ImagePath ở đây để LoadProducts sau này vẫn hoạt động
                p.Name,
                p.Description,
                p.UnitPrice,
                p.Unit,
                p.Quantity,
                SupplierName = GetSupplierName(p.SupplierID),
                p.ImportDate
            }).ToList();

            dgvThuoc.DataSource = filtered;
            // Sau khi lọc và gán DataSource mới, cần tải lại ảnh cho các hàng đã lọc
            foreach (DataGridViewRow row in dgvThuoc.Rows)
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
            // Tự động tìm kiếm khi người dùng gõ
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

        private void productImg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh sản phẩm";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _tempSelectedImagePath = ofd.FileName; // Lưu đường dẫn gốc của ảnh được chọn
                    try
                    {
                        // Hiển thị ảnh ngay lập tức lên PictureBox để xem trước
                        productImg.Image = Image.FromFile(_tempSelectedImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi hiển thị ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        productImg.Image = null;
                        _tempSelectedImagePath = ""; // Đặt lại nếu có lỗi
                    }
                }
            }
        }
    }
}