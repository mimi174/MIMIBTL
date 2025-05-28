using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N4_BTCM
{
    public partial class UC_DonHang : UserControl
    {
        // Lưu trữ danh sách đơn hàng đã lấy từ DB
        private DataTable ordersTable = new DataTable();

        public UC_DonHang()
        {
            InitializeComponent();
            this.Load += UC_DonHang_Load;
            this.txtTimKiem.GotFocus += TxtTimKiem_GotFocus;
            this.txtTimKiem.LostFocus += TxtTimKiem_LostFocus;
            this.txtTimKiem.KeyDown += TxtTimKiem_KeyDown;
            this.search.Click += Search_Click;
        }

        private void UC_DonHang_Load(object sender, EventArgs e)
        {
            LoadCustomerOrders();
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của khách hàng hiện tại từ DB và hiển thị lên giao diện
        /// </summary>
        private void LoadCustomerOrders(string searchKeyword = "")
        {
           
            int customerId = Login.LoggedInUserID;

            DBConnection db = new DBConnection();
            SqlConnection conn = db.GetConnection();
            if (conn == null) return;

            try
            {
                conn.Open();
                string query = @"
                    SELECT 
                        O.OrderID,
                        O.OrderDate,
                        O.TotalAmount,
                        OD.ProductID,
                        P.Name AS ProductName,
                        OD.Quantity,
                        OD.UnitPrice,
                        (OD.Quantity * OD.UnitPrice) AS Subtotal,
                        O.CreatedBy,
                        U.FullName AS NhanVien,
                        OID.InvoiceID,
                        OID.InvoiceDate
                    FROM Orders O
                    LEFT JOIN OrderDetails OD ON O.OrderID = OD.OrderID
                    LEFT JOIN Products P ON OD.ProductID = P.ProductID
                    LEFT JOIN Users U ON O.CreatedBy = U.UserID
                    LEFT JOIN Invoices OID ON O.OrderID = OID.OrderID
                    WHERE O.CustomerID = @CustomerID
                ";

                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    query += " AND P.Name LIKE @Keyword";
                }

                query += " ORDER BY O.OrderDate DESC, O.OrderID DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + searchKeyword + "%");
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ordersTable.Clear();
                da.Fill(ordersTable);

                // Hiển thị dữ liệu lên các panel (panel2, panel3, panel5) - demo cho 3 đơn hàng gần nhất
                DisplayOrderToPanel(ordersTable, 0, panel2, pictureBox1, label2, label3, label4, label5, label6, label13);
                DisplayOrderToPanel(ordersTable, 1, panel3, pictureBox2, label11, label10, label9, label8, label7, label12);
                DisplayOrderToPanel(ordersTable, 2, panel5, pictureBox4, label20, label19, label18, label23, label22, label21);

                // Nếu muốn hiển thị nhiều hơn, có thể tạo động các panel hoặc dùng DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải đơn hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        /// <summary>
        /// Hiển thị thông tin đơn hàng lên panel chỉ định
        /// </summary>
        private void DisplayOrderToPanel(DataTable dt, int orderIndex, Panel panel, PictureBox pic, Label lblProduct, Label lblQty, Label lblPrice, Label lblTotalText, Label lblTotal, Label lblStatus)
        {
            if (dt.Rows.Count > orderIndex)
            {
                var row = dt.Rows[orderIndex];
                panel.Visible = true;

                // Sản phẩm
                lblProduct.Text = row["ProductName"]?.ToString() ?? "N/A";
                lblQty.Text = "x" + (row["Quantity"] != DBNull.Value ? row["Quantity"].ToString() : "0");
                lblPrice.Text = string.Format("{0:N0} đ", row["UnitPrice"] ?? 0);

                // Tổng tiền
                lblTotalText.Text = "Tổng tiền:";
                lblTotal.Text = string.Format("{0:N0} đ", row["TotalAmount"] ?? 0);

                // Trạng thái đơn hàng (ví dụ: đã giao hàng nếu có InvoiceID)
                lblStatus.Text = row["InvoiceID"] != DBNull.Value ? "Giao hàng thành công" : "Đang xử lý";

                // Ảnh sản phẩm (nếu có thể lấy đường dẫn ảnh từ DB, ở đây demo giữ nguyên ảnh cũ)
                // pic.Image = ...;
            }
            else
            {
                panel.Visible = false;
            }
        }

        // Xử lý tìm kiếm khi nhấn Enter trong ô tìm kiếm
        private void TxtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchOrders();
            }
        }

        // Xử lý tìm kiếm khi click vào icon search
        private void Search_Click(object sender, EventArgs e)
        {
            SearchOrders();
        }

        private void SearchOrders()
        {
            string keyword = txtTimKiem.Text.Trim();
            if (keyword == "Tìm kiếm") keyword = "";
            LoadCustomerOrders(keyword);
        }

        // Placeholder cho ô tìm kiếm
        private void TxtTimKiem_GotFocus(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Tìm kiếm")
            {
                txtTimKiem.Text = "";
                txtTimKiem.ForeColor = Color.Black;
            }
        }

        private void TxtTimKiem_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
            {
                txtTimKiem.Text = "Tìm kiếm";
                txtTimKiem.ForeColor = Color.Gray;
            }
        }
        }
}
