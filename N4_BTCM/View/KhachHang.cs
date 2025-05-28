using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using N4_BITCM;

namespace N4_BTCM
{
    public partial class KhachHang : Form
    {
        private class Product
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public int Quantity { get; set; } = 1;
        }

        private List<Product> allProducts = new List<Product>();
        private List<Product> cart = new List<Product>();

        public KhachHang()
        {
            InitializeComponent();

            // Lấy dữ liệu sản phẩm từ database
            LoadProductsFromDatabase();

            // Gán dữ liệu động cho các PictureBox sản phẩm (nếu đủ sản phẩm)
            if (allProducts.Count >= 8)
            {
                pictureBox1.Tag = allProducts[0];
                pictureBox3.Tag = allProducts[1];
                pictureBox5.Tag = allProducts[2];
                pictureBox7.Tag = allProducts[3];
                pictureBox10.Tag = allProducts[4];
                pictureBox11.Tag = allProducts[5];
                pictureBox15.Tag = allProducts[6];
                pictureBox13.Tag = allProducts[7];
            }

            // Gắn handler cho các PictureBox sản phẩm
            pictureBox1.Click += PictureBoxProduct_Click;
            pictureBox3.Click += PictureBoxProduct_Click;
            pictureBox5.Click += PictureBoxProduct_Click;
            pictureBox7.Click += PictureBoxProduct_Click;
            pictureBox10.Click += PictureBoxProduct_Click;
            pictureBox11.Click += PictureBoxProduct_Click;
            pictureBox15.Click += PictureBoxProduct_Click;
            pictureBox13.Click += PictureBoxProduct_Click;

            // Gắn handler cho các nút chức năng
            btnTimKiem.Click += btnTimKiem_Click;
            btnQuanLyDon.Click += btnQuanLyDon_Click;
            btnHoSo.Click += btnHoSo_Click;
            btnLogout.Click += btnLogout_Click;
        }
        private void LoadUser()
        {
            lblChao.Text = "Xin chào " + Login.LoggedInFullName;
        }

        private void LoadProductsFromDatabase()
        {
            allProducts.Clear();
            DBConnection db = new DBConnection();
            SqlConnection conn = db.GetConnection();

            if (conn == null) return;

            try
            {
                conn.Open();
                string query = "SELECT Name, UnitPrice, Quantity FROM Products";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    allProducts.Add(new Product
                    {
                        Name = reader["Name"].ToString(),
                        Price = Convert.ToInt32(reader["UnitPrice"]),
                        Quantity = Convert.ToInt32(reader["Quantity"])
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void PictureBoxProduct_Click(object sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            if (pic?.Tag is Product product)
            {
                var existing = cart.FirstOrDefault(p => p.Name == product.Name);
                if (existing != null)
                {
                    existing.Quantity++;
                }
                else
                {
                    cart.Add(new Product { Name = product.Name, Price = product.Price, Quantity = 1 });
                }
                MessageBox.Show($"Đã thêm {product.Name} vào giỏ hàng!", "Thông báo");
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Sử dụng một form nhập liệu đơn giản thay cho Microsoft.VisualBasic.Interaction.InputBox
            string keyword = ShowInputDialog("Nhập tên sản phẩm cần tìm:", "Tìm kiếm sản phẩm");
            if (string.IsNullOrWhiteSpace(keyword))
                return;

            var results = allProducts.Where(p => p.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (results.Count > 0)
            {
                string msg = string.Join("\n", results.Select(p => $"{p.Name} - {p.Price:N0} đ"));
                MessageBox.Show("Kết quả tìm kiếm:\n" + msg, "Tìm kiếm");
            }
            else
            {
                MessageBox.Show("Không tìm thấy sản phẩm.", "Tìm kiếm");
            }
        }

        // Hàm thay thế InputBox
        private string ShowInputDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 340 };
            TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 340 };
            Button confirmation = new Button() { Text = "OK", Left = 270, Width = 90, Top = 80, DialogResult = DialogResult.OK };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
        }

        private void btnQuanLyDon_Click(object sender, EventArgs e)
        {
            var donHangForm = new Form();
            donHangForm.Text = "Đơn hàng của bạn";
            var ucDonHang = new UC_DonHang();
            donHangForm.Controls.Add(ucDonHang);
            ucDonHang.Dock = DockStyle.Fill;
            donHangForm.ShowDialog();
        }

        private void btnHoSo_Click(object sender, EventArgs e)
        {
            var hoSoForm = new Form();
            hoSoForm.Text = "Hồ sơ cá nhân";
            var ucHoSo = new HoSoCaNhan();
            hoSoForm.Controls.Add(ucHoSo);
            ucHoSo.Dock = DockStyle.Fill;
            hoSoForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }


        private void pictureBox5_Click(object sender, EventArgs e)
        {
            PictureBoxProduct_Click(sender, e);
        }
    }
}
