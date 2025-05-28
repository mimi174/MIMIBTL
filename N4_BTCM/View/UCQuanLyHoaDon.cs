
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N4_BTCM
{
    public partial class UCQuanLyHoaDon : UserControl
    {
        private bool isSearching = false;

        public UCQuanLyHoaDon()
        {
            InitializeComponent();
            this.Load += UCQuanLyHoaDon_Load;
        }

        private void UCQuanLyHoaDon_Load(object sender, EventArgs e)
        {
            LoadInvoiceData();
            LoadComboBoxes();

            txtTimKiem.Text = "Tìm kiếm";
            txtTimKiem.ForeColor = Color.Gray;

            txtTimKiem.TextChanged += txtTimKiem_TextChanged;
            txtTimKiem.Enter += txtTimKiem_Enter;
            txtTimKiem.Leave += txtTimKiem_Leave;
            search.Click += btnTimKiem_Click;

            btnThem.Click += btnThem_Click;
            btnLuu.Click += btnLuu_Click;
            btnLammoi.Click += btnHuy_Click;
            btnAddDetail.Click += btnAddDetail_Click;
            btnLuuDetail.Click += btnEditDetail_Click;

            cboProductID.SelectedIndexChanged += cboProductID_SelectedIndexChanged;
            dgvHoaDon.CellClick += dgvHoaDon_CellClick;
            dgvOrderDetails.CellClick += dgvOrderDetails_CellClick;
        }

        private void LoadInvoiceData()
        {
            DBConnection db = new DBConnection();
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    string query = @"SELECT I.InvoiceID, I.InvoiceDate, I.TotalAmount AS InvoiceTotal, O.OrderID,
                                        C.FullName AS CustomerName, NV.FullName AS CreatedBy
                                    FROM Invoices I
                                    JOIN Orders O ON I.OrderID = O.OrderID
                                    LEFT JOIN Users C ON O.CustomerID = C.UserID
                                    LEFT JOIN Users NV ON O.CreatedBy = NV.UserID
                                    ORDER BY I.InvoiceDate DESC;";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvHoaDon.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu hóa đơn: " + ex.Message);
                }
            }
        }

        private void LoadComboBoxes()
        {
            DBConnection db = new DBConnection();
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    SqlDataAdapter da;
                    DataTable dt;

                    da = new SqlDataAdapter("SELECT OrderID FROM Orders", conn);
                    dt = new DataTable(); da.Fill(dt);
                    cboOrderID.DataSource = dt;
                    cboOrderID.DisplayMember = "OrderID";
                    cboOrderID.ValueMember = "OrderID";

                    da = new SqlDataAdapter("SELECT UserID, FullName FROM Users WHERE RoleID = 3", conn);
                    dt = new DataTable(); da.Fill(dt);
                    cboCustomerID.DataSource = dt;
                    cboCustomerID.DisplayMember = "FullName";
                    cboCustomerID.ValueMember = "UserID";

                    da = new SqlDataAdapter("SELECT UserID, FullName FROM Users WHERE RoleID = 2", conn);
                    dt = new DataTable(); da.Fill(dt);
                    cboCreatedBy.DataSource = dt;
                    cboCreatedBy.DisplayMember = "FullName";
                    cboCreatedBy.ValueMember = "UserID";

                    da = new SqlDataAdapter("SELECT ProductID, Name, UnitPrice FROM Products", conn);
                    dt = new DataTable(); da.Fill(dt);
                    cboProductID.DataSource = dt;
                    cboProductID.DisplayMember = "Name";
                    cboProductID.ValueMember = "ProductID";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu combobox: " + ex.Message);
                }
            }
        }

        private void dgvHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!isSearching && e.RowIndex >= 0)
            {
                var row = dgvHoaDon.Rows[e.RowIndex];
                txtInvoiceID.Text = row.Cells["InvoiceID"].Value.ToString();
                dtpInvoiceDate.Value = Convert.ToDateTime(row.Cells["InvoiceDate"].Value);
                txtInvoiceTotalAmount.Text = row.Cells["InvoiceTotal"].Value.ToString();
                cboOrderID.SelectedValue = Convert.ToInt32(row.Cells["OrderID"].Value);
                LoadOrderDetailsData(Convert.ToInt32(row.Cells["OrderID"].Value));
            }
        }

        private void LoadOrderDetailsData(int orderId)
        {
            DBConnection db = new DBConnection();
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    string query = @"SELECT OD.OrderDetailID, OD.ProductID, P.Name AS ProductName,
                                        OD.Quantity, OD.UnitPrice, (OD.Quantity * OD.UnitPrice) AS Subtotal
                                     FROM OrderDetails OD
                                     JOIN Products P ON OD.ProductID = P.ProductID
                                     WHERE OD.OrderID = @OrderID";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@OrderID", orderId);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvOrderDetails.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải chi tiết đơn hàng: " + ex.Message);
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            try
            {
                DBConnection db = new DBConnection();
                conn = db.GetConnection();

                if (conn == null)
                {
                    MessageBox.Show("Không thể kết nối cơ sở dữ liệu.");
                    return;
                }

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand orderCmd = new SqlCommand(
                    "INSERT INTO Orders (CustomerID, OrderDate, TotalAmount, CreatedBy) OUTPUT INSERTED.OrderID VALUES (@cus, @date, @total, @create)",
                    conn);
                orderCmd.Parameters.AddWithValue("@cus", cboCustomerID.SelectedValue);
                orderCmd.Parameters.AddWithValue("@date", dtpInvoiceDate.Value);
                orderCmd.Parameters.AddWithValue("@total", 0);
                orderCmd.Parameters.AddWithValue("@create", cboCreatedBy.SelectedValue);

                int newOrderId = (int)orderCmd.ExecuteScalar();
                cboOrderID.Text = newOrderId.ToString();

                SqlCommand invoiceCmd = new SqlCommand(
                    "INSERT INTO Invoices (InvoiceDate, TotalAmount, OrderID) VALUES (@date, @total, @oid)", conn);
                invoiceCmd.Parameters.AddWithValue("@date", dtpInvoiceDate.Value);
                invoiceCmd.Parameters.AddWithValue("@total", decimal.Parse(txtInvoiceTotalAmount.Text));
                invoiceCmd.Parameters.AddWithValue("@oid", newOrderId);
                invoiceCmd.ExecuteNonQuery();

                LoadInvoiceData();
                MessageBox.Show("Thêm hóa đơn + đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm hóa đơn: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInvoiceID.Text)) return;

            SqlConnection conn = null;
            try
            {
                DBConnection db = new DBConnection();
                conn = db.GetConnection();

                if (conn == null)
                {
                    MessageBox.Show("Không thể kết nối cơ sở dữ liệu.");
                    return;
                }

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE Invoices SET InvoiceDate=@date, TotalAmount=@total WHERE InvoiceID=@id", conn);
                cmd.Parameters.AddWithValue("@date", dtpInvoiceDate.Value);
                cmd.Parameters.AddWithValue("@total", decimal.Parse(txtInvoiceTotalAmount.Text));
                cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtInvoiceID.Text));
                cmd.ExecuteNonQuery();

                SqlCommand orderUpdate = new SqlCommand(
                    "UPDATE Orders SET CustomerID=@cus, CreatedBy=@create WHERE OrderID=@oid", conn);
                orderUpdate.Parameters.AddWithValue("@cus", cboCustomerID.SelectedValue);
                orderUpdate.Parameters.AddWithValue("@create", cboCreatedBy.SelectedValue);
                orderUpdate.Parameters.AddWithValue("@oid", cboOrderID.Text);
                orderUpdate.ExecuteNonQuery();

                LoadInvoiceData();
                MessageBox.Show("Cập nhật thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }


        private void btnAddDetail_Click(object sender, EventArgs e)
        {
            if (cboOrderID.SelectedValue == null || cboProductID.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng và sản phẩm!");
                return;
            }

            if (!decimal.TryParse(txtUnitPriceDetail.Text, out decimal price))
            {
                MessageBox.Show("Đơn giá không hợp lệ!");
                return;
            }

            int orderId = Convert.ToInt32(cboOrderID.SelectedValue);
            int productId = Convert.ToInt32(cboProductID.SelectedValue);
            int quantity = (int)nudQuantity.Value;

            DBConnection db = new DBConnection();
            SqlConnection conn = db.GetConnection();

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                // 1. Thêm chi tiết sản phẩm vào OrderDetails
                string sql = @"INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice)
                       VALUES (@OrderID, @ProductID, @Qty, @Price)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@Qty", quantity);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.ExecuteNonQuery();
                }

                // 2. Tính lại tổng tiền mới từ OrderDetails
                string totalSql = @"SELECT SUM(Quantity * UnitPrice) FROM OrderDetails WHERE OrderID = @oid";
                SqlCommand totalCmd = new SqlCommand(totalSql, conn);
                totalCmd.Parameters.AddWithValue("@oid", orderId);
                decimal newTotal = (decimal)(totalCmd.ExecuteScalar() ?? 0);

                // 3. Cập nhật vào bảng Orders
                string updateOrder = @"UPDATE Orders SET TotalAmount = @total WHERE OrderID = @oid";
                SqlCommand updateCmd1 = new SqlCommand(updateOrder, conn);
                updateCmd1.Parameters.AddWithValue("@total", newTotal);
                updateCmd1.Parameters.AddWithValue("@oid", orderId);
                updateCmd1.ExecuteNonQuery();

                // 4. Cập nhật vào bảng Invoices
                string updateInvoice = @"UPDATE Invoices SET TotalAmount = @total WHERE OrderID = @oid";
                SqlCommand updateCmd2 = new SqlCommand(updateInvoice, conn);
                updateCmd2.Parameters.AddWithValue("@total", newTotal);
                updateCmd2.Parameters.AddWithValue("@oid", orderId);
                updateCmd2.ExecuteNonQuery();

                // 5. Cập nhật lại textbox tổng tiền
                txtInvoiceTotalAmount.Text = newTotal.ToString("N0");

                // 6. Load lại chi tiết đơn hàng và danh sách hóa đơn
                LoadOrderDetailsData(orderId);
                LoadInvoiceData();

                MessageBox.Show("Thêm chi tiết thành công và tổng tiền đã được cập nhật!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm chi tiết: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }



        private void btnEditDetail_Click(object sender, EventArgs e)
        {
            if (dgvOrderDetails.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa.");
                return;
            }

            int detailId = Convert.ToInt32(dgvOrderDetails.SelectedRows[0].Cells["OrderDetailID"].Value);

            if (cboProductID.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm.");
                return;
            }

            if (!decimal.TryParse(txtUnitPriceDetail.Text, out decimal unitPrice))
            {
                MessageBox.Show("Đơn giá không hợp lệ.");
                return;
            }

            int quantity = (int)nudQuantity.Value;
            int orderId = Convert.ToInt32(cboOrderID.SelectedValue);

            SqlConnection conn = null;

            try
            {
                DBConnection db = new DBConnection();
                conn = db.GetConnection();

                if (conn == null)
                {
                    MessageBox.Show("Không thể kết nối cơ sở dữ liệu (conn null).");
                    return;
                }

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string sql = @"UPDATE OrderDetails 
                       SET ProductID = @p, Quantity = @q, UnitPrice = @u 
                       WHERE OrderDetailID = @id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@p", Convert.ToInt32(cboProductID.SelectedValue));
                    cmd.Parameters.AddWithValue("@q", quantity);
                    cmd.Parameters.AddWithValue("@u", unitPrice);
                    cmd.Parameters.AddWithValue("@id", detailId);

                    cmd.ExecuteNonQuery();
                }

                LoadOrderDetailsData(orderId);
                MessageBox.Show("Sửa chi tiết thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa SP: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }


        private void dgvOrderDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvOrderDetails.Rows[e.RowIndex];
                cboProductID.SelectedValue = row.Cells["ProductID"].Value;
                nudQuantity.Value = Convert.ToDecimal(row.Cells["Quantity"].Value);
                txtUnitPriceDetail.Text = row.Cells["UnitPrice"].Value.ToString();
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            isSearching = true;
            string kw = txtTimKiem.Text.Trim();
            LoadInvoiceData();
            if (kw != "" && kw != "Tìm kiếm")
            {
                (dgvHoaDon.DataSource as DataTable).DefaultView.RowFilter = $"Convert(OrderID, 'System.String') LIKE '%{kw}%' OR CustomerName LIKE '%{kw}%'";
            }
            isSearching = false;
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e) => btnTimKiem_Click(sender, e);
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

        private void btnHuy_Click(object sender, EventArgs e)
        {
           
            cboCreatedBy.Text = null;
            cboCustomerID.Text = null;
            cboOrderID.Text = null;
            txtInvoiceID.Clear();
            dtpInvoiceDate.Value = DateTime.Now;
            txtInvoiceTotalAmount.Text = "0";
            dgvOrderDetails.DataSource = null;
        }

        private void cboProductID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProductID.SelectedItem is DataRowView drv)
            {
                txtUnitPriceDetail.Text = drv["UnitPrice"].ToString();
            }
        }

        private void btnXoaDetail_Click(object sender, EventArgs e)
        {
            if (dgvOrderDetails.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            int detailId = Convert.ToInt32(dgvOrderDetails.SelectedRows[0].Cells["OrderDetailID"].Value);
            int orderId = Convert.ToInt32(cboOrderID.SelectedValue);

            DBConnection db = new DBConnection();
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    string deleteSql = "DELETE FROM OrderDetails WHERE OrderDetailID = @id";
                    SqlCommand deleteCmd = new SqlCommand(deleteSql, conn);
                    deleteCmd.Parameters.AddWithValue("@id", detailId);
                    deleteCmd.ExecuteNonQuery();

                    string sumSql = "SELECT SUM(Quantity * UnitPrice) FROM OrderDetails WHERE OrderID = @oid";
                    SqlCommand sumCmd = new SqlCommand(sumSql, conn);
                    sumCmd.Parameters.AddWithValue("@oid", orderId);
                    decimal newTotal = (decimal)(sumCmd.ExecuteScalar() ?? 0);

                    string updateOrder = "UPDATE Orders SET TotalAmount = @total WHERE OrderID = @oid";
                    SqlCommand cmdOrder = new SqlCommand(updateOrder, conn);
                    cmdOrder.Parameters.AddWithValue("@total", newTotal);
                    cmdOrder.Parameters.AddWithValue("@oid", orderId);
                    cmdOrder.ExecuteNonQuery();

                    string updateInvoice = "UPDATE Invoices SET TotalAmount = @total WHERE OrderID = @oid";
                    SqlCommand cmdInvoice = new SqlCommand(updateInvoice, conn);
                    cmdInvoice.Parameters.AddWithValue("@total", newTotal);
                    cmdInvoice.Parameters.AddWithValue("@oid", orderId);
                    cmdInvoice.ExecuteNonQuery();

                    txtInvoiceTotalAmount.Text = newTotal.ToString("N0");
                    LoadOrderDetailsData(orderId);
                    LoadInvoiceData();

                    MessageBox.Show("Xóa sản phẩm thành công và đã cập nhật lại tổng tiền!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa chi tiết: " + ex.Message);
                }
            }
        }
    }
}
