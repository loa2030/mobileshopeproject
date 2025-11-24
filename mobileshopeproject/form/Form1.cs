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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void LinkBACK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UserLogin objLogin = new UserLogin();
            objLogin.Show();
            this.Hide();
        }
    }
}
