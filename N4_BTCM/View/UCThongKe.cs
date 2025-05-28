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
using System.Windows.Forms.DataVisualization.Charting; // Thêm using này

namespace N4_BTCM
{
    public partial class UCThongKe : UserControl
    {
        public UCThongKe()
        {
            InitializeComponent();
            this.Load += new EventHandler(UCThongKe_Load);
            this.cboThongKeTheo.SelectedIndexChanged += new EventHandler(cboThongKeTheo_SelectedIndexChanged);
        }

        private void UCThongKe_Load(object sender, EventArgs e)
        {
            // Thiết lập giá trị mặc định ban đầu
            cboThongKeTheo.SelectedItem = "Ngày"; // Chọn mặc định thống kê theo ngày
            dtpTuNgay.Value = DateTime.Today.AddDays(-30); // Mặc định 30 ngày trước
            dtpDenNgay.Value = DateTime.Today; // Đến hôm nay

            ConfigureChart(); // Cấu hình biểu đồ ban đầu
            LoadAndDisplayChartData(); // Tải và hiển thị dữ liệu lần đầu
        }

        private void cboThongKeTheo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Khi thay đổi loại thống kê (Ngày/Tháng/Năm), có thể điều chỉnh hiển thị DatePicker
            // Ví dụ: nếu chọn 'Tháng', chỉ cho phép chọn Tháng/Năm.
            string selectedType = cboThongKeTheo.SelectedItem.ToString();
            if (selectedType == "Ngày")
            {
                dtpTuNgay.CustomFormat = "dd/MM/yyyy";
                dtpDenNgay.CustomFormat = "dd/MM/yyyy";
                dtpTuNgay.ShowUpDown = false;
                dtpDenNgay.ShowUpDown = false;
            }
            else if (selectedType == "Tháng")
            {
                dtpTuNgay.CustomFormat = "MM/yyyy";
                dtpDenNgay.CustomFormat = "MM/yyyy";
                dtpTuNgay.ShowUpDown = true; // Cho phép chọn tháng/năm bằng up/down
                dtpDenNgay.ShowUpDown = true;
            }
            else if (selectedType == "Năm")
            {
                dtpTuNgay.CustomFormat = "yyyy";
                dtpDenNgay.CustomFormat = "yyyy";
                dtpTuNgay.ShowUpDown = true;
                dtpDenNgay.ShowUpDown = true;
            }
        }

        private void ConfigureChart()
        {
            chartThongKe.Series.Clear();
            chartThongKe.Titles.Clear();
            chartThongKe.ChartAreas.Clear();
            chartThongKe.Legends.Clear();

            chartThongKe.ChartAreas.Add(new ChartArea("MainArea"));
            chartThongKe.Legends.Add(new Legend("Legend1"));

            // Cấu hình trục X (AxisX) và Y (AxisY)
            chartThongKe.ChartAreas["MainArea"].AxisX.LabelStyle.Format = "dd/MM"; // Mặc định
            chartThongKe.ChartAreas["MainArea"].AxisX.IntervalType = DateTimeIntervalType.Days;
            chartThongKe.ChartAreas["MainArea"].AxisX.Interval = 1;
            chartThongKe.ChartAreas["MainArea"].AxisX.MajorGrid.Enabled = false; // Ẩn lưới dọc

            chartThongKe.ChartAreas["MainArea"].AxisY.Title = "Doanh thu (VND)";
            chartThongKe.ChartAreas["MainArea"].AxisY.Minimum = 0; // Đảm bảo bắt đầu từ 0
            chartThongKe.ChartAreas["MainArea"].AxisY.LabelStyle.Format = "#,##0 VNĐ"; // Định dạng tiền tệ

            // Thêm Series (dữ liệu biểu đồ)
            Series salesSeries = new Series("Doanh Thu");
            salesSeries.ChartType = SeriesChartType.Line; // Kiểu biểu đồ đường (có thể là Column, Area...)
            salesSeries.XValueType = ChartValueType.DateTime;
            salesSeries.IsValueShownAsLabel = true; // Hiển thị giá trị trên điểm
            salesSeries.BorderWidth = 2; // Độ rộng đường
            chartThongKe.Series.Add(salesSeries);

            chartThongKe.Titles.Add("BIỂU ĐỒ DOANH THU THEO THỜI GIAN");
        }


        private void btnXemThongKe_Click(object sender, EventArgs e)
        {
            LoadAndDisplayChartData();
        }

        private void LoadAndDisplayChartData()
        {
            DBConnection db = new DBConnection();
            using (SqlConnection conn = db.GetConnection())
            {
                if (conn == null) return;

                try
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    string selectedType = cboThongKeTheo.SelectedItem.ToString();
                    DateTime startDate = dtpTuNgay.Value;
                    DateTime endDate = dtpDenNgay.Value;

                    string groupByFormat = "";
                    string selectColumns = "";
                    string orderByColumn = "";
                    string titleSuffix = "";

                    // Xóa dữ liệu cũ
                    chartThongKe.Series["Doanh Thu"].Points.Clear();

                    switch (selectedType)
                    {
                        case "Ngày":
                            groupByFormat = "CAST(OrderDate AS DATE)";
                            selectColumns = "CAST(OrderDate AS DATE) AS ThoiGian, SUM(TotalAmount) AS TongDoanhThu";
                            orderByColumn = "ThoiGian ASC";
                            titleSuffix = "theo Ngày";
                            chartThongKe.ChartAreas["MainArea"].AxisX.LabelStyle.Format = "dd/MM";
                            chartThongKe.ChartAreas["MainArea"].AxisX.IntervalType = DateTimeIntervalType.Days;
                            chartThongKe.ChartAreas["MainArea"].AxisX.Interval = 1;
                            break;
                        case "Tháng":
                            startDate = new DateTime(startDate.Year, startDate.Month, 1);
                            endDate = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));
                            groupByFormat = "FORMAT(OrderDate, 'yyyy-MM')";
                            selectColumns = "FORMAT(OrderDate, 'yyyy-MM') AS ThoiGian, SUM(TotalAmount) AS TongDoanhThu";
                            orderByColumn = "ThoiGian ASC";
                            titleSuffix = "theo Tháng";
                            chartThongKe.ChartAreas["MainArea"].AxisX.LabelStyle.Format = "MM/yyyy";
                            chartThongKe.ChartAreas["MainArea"].AxisX.IntervalType = DateTimeIntervalType.Months;
                            chartThongKe.ChartAreas["MainArea"].AxisX.Interval = 1;
                            break;
                        case "Năm":
                            startDate = new DateTime(startDate.Year, 1, 1);
                            endDate = new DateTime(endDate.Year, 12, 31);
                            groupByFormat = "YEAR(OrderDate)";
                            selectColumns = "YEAR(OrderDate) AS ThoiGian, SUM(TotalAmount) AS TongDoanhThu";
                            orderByColumn = "ThoiGian ASC";
                            titleSuffix = "theo Năm";
                            chartThongKe.ChartAreas["MainArea"].AxisX.LabelStyle.Format = "yyyy";
                            chartThongKe.ChartAreas["MainArea"].AxisX.IntervalType = DateTimeIntervalType.Years;
                            chartThongKe.ChartAreas["MainArea"].AxisX.Interval = 1;
                            break;
                    }

                    string query = $@"
                SELECT
                    {selectColumns}
                FROM
                    Orders
                WHERE
                    OrderDate >= @StartDate AND OrderDate <= @EndDate
                GROUP BY
                    {groupByFormat}
                ORDER BY
                    {orderByColumn};";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime timePoint;
                                if (selectedType == "Ngày")
                                {
                                    timePoint = reader.GetDateTime(reader.GetOrdinal("ThoiGian"));
                                }
                                else if (selectedType == "Tháng")
                                {
                                    timePoint = DateTime.ParseExact(reader["ThoiGian"].ToString(), "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else // Năm
                                {
                                    timePoint = new DateTime(Convert.ToInt32(reader["ThoiGian"]), 1, 1);
                                }

                                decimal doanhThu = reader.GetDecimal(reader.GetOrdinal("TongDoanhThu"));
                                chartThongKe.Series["Doanh Thu"].Points.AddXY(timePoint, doanhThu);
                            }
                        }
                    }
                    chartThongKe.Titles[0].Text = $"BIỂU ĐỒ DOANH THU {titleSuffix}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu thống kê: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}