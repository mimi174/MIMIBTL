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
using OfficeOpenXml; // Cần cho Excel
using System.IO; // Cần cho FileInfo

namespace N4_BTCM
{
    public partial class UCQuanLyHoaDon : UserControl
    {
        public UCQuanLyHoaDon()
        {
            InitializeComponent();
            // Gán sự kiện Load cho UserControl
            this.Load += new EventHandler(UCQuanLyHoaDon_Load);

            // Gán sự kiện Click cho các nút chức năng chính
            this.btnThem.Click += new EventHandler(this.btnThem_Click);
            this.btnLuu.Click += new EventHandler(this.btnLuu_Click);
            this.btnXoa.Click += new EventHandler(this.btnXoa_Click);
            this.btnLammoi.Click += new EventHandler(this.btnHuy_Click);

            // Gán sự kiện Click cho các nút điều khiển chi tiết đơn hàng
            this.btnAddDetail.Click += new EventHandler(this.btnAddDetail_Click);
            this.btnEditDetail.Click += new EventHandler(this.btnEditDetail_Click);
            this.btnRemoveDetail.Click += new EventHandler(this.btnRemoveDetail_Click);

            // Gán sự kiện CellClick cho dgvHoaDon để hiển thị chi tiết
            this.dgvHoaDon.CellClick += new DataGridViewCellEventHandler(this.dgvHoaDon_CellClick);
        }

        private void UCQuanLyHoaDon_Load(object sender, EventArgs e)
        {
            LoadInvoiceData(); // Tải dữ liệu hóa đơn chính
            LoadComboBoxes(); // Tải dữ liệu cho các ComboBox (OrderID, CustomerID, CreatedBy, ProductID)
        }

        private void LoadInvoiceData()
        {
            DBConnection db = new DBConnection();
            SqlConnection conn = db.GetConnection();

            if (conn == null) return;

            try
            {
                string query = @"
                    SELECT
                        I.InvoiceID,
                        I.InvoiceDate,
                        I.TotalAmount AS InvoiceTotal,
                        O.OrderID,
                        C.FullName AS CustomerName,
                        NV.FullName AS CreatedBy
                    FROM
                        Invoices I
                    JOIN
                        Orders O ON I.OrderID = O.OrderID
                    LEFT JOIN
                        Users C ON O.CustomerID = C.UserID
                    LEFT JOIN
                        Users NV ON O.CreatedBy = NV.UserID
                    ORDER BY
                        I.InvoiceDate DESC;";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvHoaDon.DataSource = dt;

                // Tùy chỉnh hiển thị cột (tùy chọn)
                dgvHoaDon.Columns["InvoiceID"].HeaderText = "Mã HĐ";
                dgvHoaDon.Columns["InvoiceDate"].HeaderText = "Ngày Lập HĐ";
                dgvHoaDon.Columns["InvoiceTotal"].HeaderText = "Tổng Tiền HĐ";
                dgvHoaDon.Columns["OrderID"].HeaderText = "Mã ĐH";
                dgvHoaDon.Columns["CustomerName"].HeaderText = "Tên Khách hàng";
                dgvHoaDon.Columns["CreatedBy"].HeaderText = "Người Lập";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void LoadComboBoxes()
        {
            DBConnection db = new DBConnection();
            SqlConnection conn = db.GetConnection();

            if (conn == null) return;

            try
            {
                conn.Open();

                // Load Orders vào cboOrderID (chỉ các đơn hàng chưa có hóa đơn)
                string ordersQuery = "SELECT OrderID, OrderDate, TotalAmount FROM Orders WHERE OrderID NOT IN (SELECT OrderID FROM Invoices)";
                SqlDataAdapter daOrders = new SqlDataAdapter(ordersQuery, conn);
                DataTable dtOrders = new DataTable();
                daOrders.Fill(dtOrders);
                cboOrderID.DataSource = dtOrders;
                cboOrderID.DisplayMember = "OrderID"; // Hiển thị OrderID
                cboOrderID.ValueMember = "OrderID"; // Giá trị thực tế là OrderID

                // Load Customers vào cboCustomerID
                string customersQuery = "SELECT UserID, FullName FROM Users WHERE RoleID = 3"; // RoleID 3 là khách hàng
                SqlDataAdapter daCustomers = new SqlDataAdapter(customersQuery, conn);
                DataTable dtCustomers = new DataTable();
                daCustomers.Fill(dtCustomers);
                cboCustomerID.DataSource = dtCustomers;
                cboCustomerID.DisplayMember = "FullName";
                cboCustomerID.ValueMember = "UserID";

                // Load CreatedBy (Nhân viên) vào cboCreatedBy
                string employeesQuery = "SELECT UserID, FullName FROM Users WHERE RoleID = 2"; // RoleID 2 là nhân viên
                SqlDataAdapter daEmployees = new SqlDataAdapter(employeesQuery, conn);
                DataTable dtEmployees = new DataTable();
                daEmployees.Fill(dtEmployees);
                cboCreatedBy.DataSource = dtEmployees;
                cboCreatedBy.DisplayMember = "FullName";
                cboCreatedBy.ValueMember = "UserID";

                // Load Products vào cboProductID
                string productsQuery = "SELECT ProductID, Name, UnitPrice FROM Products";
                SqlDataAdapter daProducts = new SqlDataAdapter(productsQuery, conn);
                DataTable dtProducts = new DataTable();
                daProducts.Fill(dtProducts);
                cboProductID.DataSource = dtProducts;
                cboProductID.DisplayMember = "Name";
                cboProductID.ValueMember = "ProductID";
                cboProductID.SelectedIndexChanged += new EventHandler(cboProductID_SelectedIndexChanged); // Gán sự kiện để tự điền đơn giá

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu ComboBoxes: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void dgvHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvHoaDon.Rows[e.RowIndex];
                txtInvoiceID.Text = row.Cells["InvoiceID"].Value.ToString();
                dtpInvoiceDate.Value = Convert.ToDateTime(row.Cells["InvoiceDate"].Value);
                txtInvoiceTotalAmount.Text = row.Cells["InvoiceTotal"].Value.ToString();

                // Load chi tiết đơn hàng khi chọn hóa đơn
                if (row.Cells["OrderID"].Value != DBNull.Value)
                {
                    int orderId = Convert.ToInt32(row.Cells["OrderID"].Value);
                    LoadOrderDetailsData(orderId);
                    cboOrderID.SelectedValue = orderId; // Chọn đúng OrderID trong ComboBox
                }
            }
        }

        private void LoadOrderDetailsData(int orderId)
        {
            DBConnection db = new DBConnection();
            SqlConnection conn = db.GetConnection();

            if (conn == null) return;

            try
            {
                string query = @"
                    SELECT
                        OD.OrderDetailID,
                        OD.ProductID,
                        P.Name AS ProductName,
                        OD.Quantity,
                        OD.UnitPrice,
                        (OD.Quantity * OD.UnitPrice) AS Subtotal
                    FROM
                        OrderDetails OD
                    JOIN
                        Products P ON OD.ProductID = P.ProductID
                    WHERE
                        OD.OrderID = @OrderID;";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@OrderID", orderId);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvOrderDetails.DataSource = dt;

                // Tùy chỉnh hiển thị cột (tùy chọn)
                dgvOrderDetails.Columns["OrderDetailID"].HeaderText = "Mã CTĐH";
                dgvOrderDetails.Columns["ProductID"].HeaderText = "Mã SP";
                dgvOrderDetails.Columns["ProductName"].HeaderText = "Tên Sản phẩm";
                dgvOrderDetails.Columns["Quantity"].HeaderText = "Số lượng";
                dgvOrderDetails.Columns["UnitPrice"].HeaderText = "Đơn giá";
                dgvOrderDetails.Columns["Subtotal"].HeaderText = "Thành tiền";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải chi tiết đơn hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        // Xử lý sự kiện khi chọn sản phẩm trong cboProductID, tự động điền đơn giá
        private void cboProductID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProductID.SelectedValue != null && cboProductID.SelectedValue != DBNull.Value)
            {
                // Lấy ProductID từ ComboBox
                int productId = Convert.ToInt32(cboProductID.SelectedValue);

                // Tìm đơn giá của sản phẩm đó từ nguồn dữ liệu của cboProductID (hoặc truy vấn lại DB)
                // Nếu cboProductID.DataSource là DataTable
                DataRowView drv = cboProductID.SelectedItem as DataRowView;
                if (drv != null)
                {
                    decimal unitPrice = Convert.ToDecimal(drv["UnitPrice"]);
                    txtUnitPriceDetail.Text = unitPrice.ToString();
                }
                // Hoặc truy vấn lại DB nếu không chắc nguồn dữ liệu combo có UnitPrice
                // DBConnection db = new DBConnection();
                // SqlConnection conn = db.GetConnection();
                // try { ... query Price ... }
                // finally { conn.Close(); }
            }
        }


        // ************ Logic cho các nút chức năng chính ************
        private void btnThem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng 'Thêm Hóa đơn' đang được phát triển");
            // Logic: Xóa trắng các trường nhập liệu, chuẩn bị cho việc nhập hóa đơn mới
            // txtInvoiceID.Clear(); dtpInvoiceDate.Value = DateTime.Now; ...
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng 'Lưu Hóa đơn' đang được phát triển");
            // Logic: Lưu thông tin hóa đơn từ các trường nhập liệu vào DB
            // (INSERT/UPDATE vào Invoices, Orders, OrderDetails)
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng 'Xóa Hóa đơn' đang được phát triển");
            // Logic: Xóa hóa đơn khỏi DB (cần hỏi xác nhận)
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng 'Hủy' đang được phát triển");
            // Logic: Hủy bỏ các thay đổi, quay về trạng thái trước đó
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng 'Tìm kiếm' đang được phát triển");
            // Logic: Lọc dữ liệu trong dgvHoaDon dựa trên txtTimKiem
        }

        private void btnAddDetail_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng 'Thêm sản phẩm vào chi tiết đơn hàng' đang được phát triển");
            // Logic: Thêm một dòng mới vào dgvOrderDetails (tạm thời)
            // Sau đó, khi lưu hóa đơn, các dòng này sẽ được lưu vào bảng OrderDetails
        }

        private void btnEditDetail_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng 'Sửa sản phẩm trong chi tiết đơn hàng' đang được phát triển");
            // Logic: Sửa thông tin của dòng được chọn trong dgvOrderDetails
        }

        private void btnRemoveDetail_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng 'Xóa sản phẩm khỏi chi tiết đơn hàng' đang được phát triển");
            // Logic: Xóa dòng được chọn khỏi dgvOrderDetails
        }

        // ************ Logic phân trang (cần triển khai logic cho phân trang thực tế) ************
        private void btnFirstPage_Click(object sender, EventArgs e) { MessageBox.Show("Đang ở trang đầu tiên."); }
        private void btnPrevPage_Click(object sender, EventArgs e) { MessageBox.Show("Chức năng lùi trang đang được phát triển."); }
        private void btnNextPage_Click(object sender, EventArgs e) { MessageBox.Show("Chức năng tiến trang đang được phát triển."); }
        private void btnLastPage_Click(object sender, EventArgs e) { MessageBox.Show("Đang ở trang cuối cùng."); }

        private void btnLammoi_Click(object sender, EventArgs e)
        {

        }
    }
}