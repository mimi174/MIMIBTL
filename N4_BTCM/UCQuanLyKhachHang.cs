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
using OfficeOpenXml; // Cần thiết cho Excel
using System.IO; // Cần thiết cho FileInfo
using Microsoft.WindowsAPICodePack.Dialogs; // Cần thiết cho CommonOpenFileDialog nếu muốn dùng

namespace N4_BTCM
{
    public partial class UCQuanLyKhachHang : UserControl
    {
        public UCQuanLyKhachHang()
        {
            InitializeComponent();
            // Gán sự kiện Load cho UserControl khi nó được khởi tạo
            this.Load += new EventHandler(UCQuanLyKhachHang_Load);
        }

        private void UCQuanLyKhachHang_Load(object sender, EventArgs e)
        {
            //LoadCustomerData(); // Tải dữ liệu khách hàng khi UserControl được hiển thị
        }

        //private void LoadCustomerData()
        //{
        //    DBConnection db = new DBConnection();
        //    SqlConnection conn = db.GetConnection();

        //    if (conn == null)
        //    {
        //        return; // Lỗi kết nối đã được xử lý trong GetConnection
        //    }

        //    try
        //    {
        //        // Truy vấn dữ liệu khách hàng (RoleID = 3)
        //        string query = @"
        //            SELECT
        //                UserID,
        //                Username,
        //                FullName,
        //                Email,
        //                Phone,
        //                Address,
        //                CreatedAt
        //            FROM
        //                Users
        //            WHERE
        //                RoleID = 3;
        //        ";

        //        SqlDataAdapter da = new SqlDataAdapter(query, conn);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        dgvKhachHang.DataSource = dt; // Gán dữ liệu vào DataGridView

        //        dgvKhachHang.Columns["UserID"].HeaderText = "Mã KH";
        //        dgvKhachHang.Columns["Username"].HeaderText = "Tên đăng nhập";
        //        dgvKhachHang.Columns["FullName"].HeaderText = "Họ và tên";
        //        dgvKhachHang.Columns["Email"].HeaderText = "Email";
        //        dgvKhachHang.Columns["Phone"].HeaderText = "Số điện thoại";
        //        dgvKhachHang.Columns["Address"].HeaderText = "Địa chỉ";
        //        dgvKhachHang.Columns["CreatedAt"].HeaderText = "Ngày tạo";

        //        if (dgvKhachHang.Columns.Contains("PasswordHash"))
        //        {
        //            dgvKhachHang.Columns["PasswordHash"].Visible = false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi khi tải dữ liệu khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

        private void btnThem_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để thêm khách hàng
            MessageBox.Show("Chức năng thêm khách hàng đang được phát triển");
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để sửa khách hàng
            MessageBox.Show("Chức năng sửa khách hàng đang được phát triển");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để xóa khách hàng
            MessageBox.Show("Chức năng xóa khách hàng đang được phát triển");
        }

        private void panelInfo_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvKhachHang_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        // ************ Logic Nhập/Xuất Excel ************
        //private void btnNhapExcel_Click(object sender, EventArgs e)
        //{
        //    if (!DesignMode) // Chỉ chạy khi không ở chế độ thiết kế
        //    {
        //        try
        //        {
        //            OpenFileDialog openFileDialog = new OpenFileDialog();
        //            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
        //            openFileDialog.Title = "Chọn file Excel để nhập dữ liệu khách hàng";
        //            if (openFileDialog.ShowDialog() == DialogResult.OK)
        //            {
        //                FileInfo excelFile = new FileInfo(openFileDialog.FileName);
        //                using (ExcelPackage excelPackage = new ExcelPackage(excelFile))
        //                {
        //                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0]; // Lấy worksheet đầu tiên

        //                    // Xóa dữ liệu cũ trong DataGridView trước khi nhập mới
        //                    dgvKhachHang.Rows.Clear();

        //                    // Đọc dữ liệu từ Excel và thêm vào DataGridView
        //                    // Bắt đầu từ dòng 2 để bỏ qua header
        //                    for (int rowNum = 2; rowNum <= worksheet.Dimension.Rows; rowNum++)
        //                    {
        //                        DataGridViewRow dgvRow = new DataGridViewRow();
        //                        // Tạo một mảng object chứa các giá trị của một hàng từ Excel
        //                        object[] rowValues = new object[worksheet.Dimension.Columns];
        //                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
        //                        {
        //                            rowValues[col - 1] = worksheet.Cells[rowNum, col].Value?.ToString();
        //                        }
        //                        dgvKhachHang.Rows.Add(rowValues);
        //                    }
        //                    MessageBox.Show("Nhập dữ liệu khách hàng từ Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Lỗi khi nhập Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}

        //private void btnXuatExcel_Click(object sender, EventArgs e)
        //{
        //    if (!DesignMode) // Chỉ chạy khi không ở chế độ thiết kế
        //    {
        //        try
        //        {
        //            using (ExcelPackage excelPackage = new ExcelPackage())
        //            {
        //                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("KhachHang"); // Tên worksheet

        //                // Đổ tiêu đề cột từ DataGridView vào Excel
        //                for (int i = 0; i < dgvKhachHang.Columns.Count; i++)
        //                {
        //                    worksheet.Cells[1, i + 1].Value = dgvKhachHang.Columns[i].HeaderText;
        //                }

        //                // Đổ dữ liệu từ DataGridView vào Excel
        //                for (int i = 0; i < dgvKhachHang.Rows.Count; i++)
        //                {
        //                    for (int j = 0; j < dgvKhachHang.Columns.Count; j++)
        //                    {
        //                        worksheet.Cells[i + 2, j + 1].Value = dgvKhachHang.Rows[i].Cells[j].Value?.ToString();
        //                    }
        //                }

        //                worksheet.Cells.AutoFitColumns(); // Tự động điều chỉnh kích thước cột

        //                // Sử dụng FolderBrowserDialog làm giải pháp thay thế cho CommonOpenFileDialog
        //                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        //                folderBrowserDialog.Description = "Chọn thư mục để lưu file Excel khách hàng";

        //                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        //                {
        //                    string folderPath = folderBrowserDialog.SelectedPath;
        //                    string filePath = Path.Combine(folderPath, "DanhSachKhachHang.xlsx"); // Tên file

        //                    FileInfo excelFile = new FileInfo(filePath);
        //                    excelPackage.SaveAs(excelFile);

        //                    MessageBox.Show("Xuất dữ liệu khách hàng ra Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}
    }
}