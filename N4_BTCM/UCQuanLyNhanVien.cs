using OfficeOpenXml;
using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace N4_BTCM
{
    public partial class UCQuanLyNhanVien : UserControl
    {
        public UCQuanLyNhanVien()
        {
            InitializeComponent();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để thêm nhân viên
            MessageBox.Show("Chức năng thêm nhân viên đang được phát triển");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            //  TODO: Thêm logic để xóa nhân viên
            MessageBox.Show("Chức năng xóa nhân viên đang được phát triển");
        }

        private void dgvNhanVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("NhanVien"); // Tên worksheet

                    // Tiêu đề các cột (lấy từ header của DataGridView)
                    for (int i = 1; i <= dgvNhanVien.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i].Value = dgvNhanVien.Columns[i - 1].HeaderText;
                    }

                    // Đổ dữ liệu từ DataGridView vào Excel
                    for (int i = 0; i < dgvNhanVien.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgvNhanVien.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 2, j + 1].Value = dgvNhanVien.Rows[i].Cells[j].Value?.ToString();
                        }
                    }

                    // Tự động điều chỉnh kích thước cột
                    worksheet.Cells.AutoFitColumns();

                    // Chọn thư mục để lưu file Excel
                    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog(); // Thay thế CommonOpenFileDialog
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = folderBrowserDialog.SelectedPath; // Lấy đường dẫn thư mục
                        string filePath = Path.Combine(folderPath, "DanhSachNhanVien.xlsx"); // Tên file

                        // Lưu file Excel
                        FileInfo excelFile = new FileInfo(filePath);
                        excelPackage.SaveAs(excelFile);

                        MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNhapExcel_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                openFileDialog.Title = "Chọn file Excel";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo excelFile = new FileInfo(openFileDialog.FileName);
                    using (ExcelPackage excelPackage = new ExcelPackage(excelFile))
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0]; // Lấy worksheet đầu tiên

                        // Xóa dữ liệu cũ trong DataGridView
                        dgvNhanVien.Rows.Clear();

                        // Đọc dữ liệu từ Excel và thêm vào DataGridView
                        for (int i = 2; i <= worksheet.Dimension.Rows; i++) // Bắt đầu từ dòng 2 (bỏ qua header)
                        {
                            object[] rowValues = new object[worksheet.Dimension.Columns];
                            for (int j = 1; j <= worksheet.Dimension.Columns; j++)
                            {
                                rowValues[j - 1] = worksheet.Cells[i, j].Value?.ToString();
                            }
                            dgvNhanVien.Rows.Add(rowValues);
                        }

                        MessageBox.Show("Nhập Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nhập Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}