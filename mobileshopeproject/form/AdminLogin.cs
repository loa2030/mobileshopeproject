using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mobileshopeproject.form
{
    public partial class AdminLogin : Form
    {
        public AdminLogin()
        {
            InitializeComponent();
        }

        private void linkback_linkClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UserLogin objLogin = new UserLogin();
            objLogin.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtuid.Text == "admin" && txtpwd.Text == "admin")
            {
                AdminHomepage objAdminHome = new AdminHomepage();
                objAdminHome.Show();
                this.Hide();
            }
            else
            {
                lblMsg.Text = "User is not valid";
            }
        }
    }
}
