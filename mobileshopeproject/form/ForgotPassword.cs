using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mobileshopeproject.Data;

namespace mobileshopeproject.form
{
    public partial class ForgotPassword : Form
    {
        private SqlConnection conn = Database.GetConnection();
        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            string username= textBox1.Text;
            string hint= textBox2.Text;
            try
            {
                conn.Open();
                string query = "SELECT PWD FROM tbl_User WHERE UserName = @user AND Hint = @hint";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user", textBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@hint", textBox2.Text.Trim());
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) 
                {
                    label3.Text += reader["PWD"].ToString();
                }
                else
                {
                    MessageBox.Show("Hint hoặc Username không đúng!");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UserLogin userLogin = new UserLogin();
            userLogin.Show();
            this.Close();
        }
    }
}
