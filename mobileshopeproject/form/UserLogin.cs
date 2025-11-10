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
    public partial class UserLogin : Form
    {
        public UserLogin()
        {
            InitializeComponent();
        }

        //void linklabel_linkClick()
        //{
        //    AdminLogin objLogin = new AdminLogin();
        //    objLogin.Show();
        //    this.Hide();
        //}
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminLogin objLogin = new AdminLogin();
            objLogin.Show();
            this.Hide();
        }
    }
}
