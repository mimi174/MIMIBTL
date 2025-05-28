using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using N4_BITCM;

namespace N4_BTCM
{
    public partial class HoSoCaNhan : UserControl
    {
        private int userId = Login.LoggedInUserID;
        private UserProfile currentUser;
        private UserProfileDAO dao = new UserProfileDAO();

        public HoSoCaNhan()
        {
            InitializeComponent();
            userId = Login.LoggedInUserID;
            LoadUserProfile();

            btnCapNhat.Click += btnCapNhat_Click;
            pbAvatar.Click += picAvatar_Click;
            lblName.Click += lblName_Click;
            pictureBox13.Click += pictureBox13_Click;
            lblSDT.Click += lblSDT_Click;
            lblGioiTinh.Click += lblGioiTinh_Click;
            txtDiaChi.TextChanged += txtAdd_TextChanged;
            lblAddress.Click += lblAddress_Click;
        }

        private void LoadUserProfile()
        {
            currentUser = dao.GetUserProfileById(userId);

            if (currentUser != null)
            {
                txtHoTen.Text = currentUser.HoTen ?? "";
                cbGioiTinh.SelectedItem = currentUser.GioiTinh ?? "";
                txtSDT.Text = currentUser.SoDienThoai ?? "";
                txtEmail.Text = currentUser.Email ?? "";
                txtDiaChi.Text = currentUser.DiaChi ?? "";

                // Ngày sinh
                if (currentUser.NgaySinh > new DateTime(1950, 1, 1))
                {
                    dtpNgaySinh.Value = currentUser.NgaySinh;
                }
                else
                {
                    dtpNgaySinh.Value = DateTime.Now;
                }

                // Avatar
                if (currentUser.Avatar != null)
                {
                    pbAvatar.Image = ByteArrayToImage(currentUser.Avatar);
                }
                else
                {
                    pbAvatar.Image = null;
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void picAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    pbAvatar.Image = Image.FromFile(open.FileName);
                }
            }
        }

        private byte[] ImageToByteArray(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            if (currentUser == null)
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentUser.HoTen = txtHoTen.Text.Trim();
            currentUser.GioiTinh = cbGioiTinh.SelectedItem?.ToString() ?? "";
            currentUser.SoDienThoai = txtSDT.Text.Trim();
            currentUser.Email = txtEmail.Text.Trim();
            currentUser.DiaChi = txtDiaChi.Text.Trim();

            // Ngày sinh
            currentUser.NgaySinh = dtpNgaySinh.Value;

            // Avatar
            if (pbAvatar.Image != null)
            {
                currentUser.Avatar = ImageToByteArray(pbAvatar.Image);
            }

            if (dao.UpdateUserProfile(currentUser))
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUserProfile();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void lblName_Click(object sender, EventArgs e) { }
        private void pictureBox13_Click(object sender, EventArgs e) { }
        private void lblSDT_Click(object sender, EventArgs e) { }
        private void lblGioiTinh_Click(object sender, EventArgs e) { }
        private void txtAdd_TextChanged(object sender, EventArgs e) { }
        private void lblAddress_Click(object sender, EventArgs e) { }

        internal void ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}
