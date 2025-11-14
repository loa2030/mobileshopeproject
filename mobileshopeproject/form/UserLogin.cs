using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace mobileshopeproject.form
{
    public partial class UserLogin : Form
    {
        private static readonly string conString = ConfigurationManager.ConnectionStrings["AppMobileConnection"].ConnectionString;
        private SqlConnection conn = new SqlConnection(conString);
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

        private void btnUserLogin_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                string query = "SELECT * FROM tbl_User WHERE UserName = @uname AND PWD = @pwd";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uname", txtuserID.Text.Trim());
                cmd.Parameters.AddWithValue("@pwd", txtUserPass.Text.Trim());

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    string username = reader["UserName"].ToString();
                    reader.Close();
                    conn.Close();

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở form UserHomepage và truyền tên người dùng
                    UserHomepage userForm = new UserHomepage(username);
                    userForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
    }
}
