using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N4_BTCM
{
    public partial class KhachHang: Form
    {
        public KhachHang()
        {
            InitializeComponent();
            LoadUser();
        }

        private void LoadUser()
        {
            lblChao.Text = "Xin chào " + Login.LoggedInFullName;
        }

        private void btnHoSo_Click(object sender, EventArgs e)
        {
            HoSoCaNhan form = new HoSoCaNhan();
            form.ShowDialog();
        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void KhachHang_Load(object sender, EventArgs e)
        {

        }

        private void btnLammoi_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
