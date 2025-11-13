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

namespace mobileshopeproject.form
{
    public partial class ConfirmDetails : Form
    {
        string username;
        string companyN;
        string modelNumber;
        string imei;
        string price;
        string customer;
        string Mobilenumber;
        string address;
        string Email;
        SqlConnection conn = new SqlConnection(@"Server=LAPTOP-86BEM0IS;Database=AppMobile;Integrated Security=True");
        public ConfirmDetails(string username, string company, string modelNumber,
        string imei,
        string price ,string customer,string Mbnumer,string add,string email)
        {
            InitializeComponent();
            this.username = username.Trim();
            companyN = company.Trim();
            this.modelNumber = modelNumber.Trim();
            this.imei = imei.Trim();
            this.price = price.Trim();
            this.customer = customer.Trim();
            Mobilenumber = Mbnumer.Trim();
            address = add.Trim();
            Email = email.Trim();
            Load_detail();
        }
        private void Load_detail( )
        {
            lbCus.Text = customer;
            lbMN.Text = Mobilenumber;
            lbAdd.Text=address;
            lbEmail.Text = Email;
            lbCN.Text=companyN;
            lbModelN.Text = modelNumber;
            lbIMEI.Text = imei;
            lbPrice.Text = price;
            try
            {
                conn.Open();
                string query = "SELECT Warranty FROM tbl_Mobile WHERE IMEINO = @imei";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@imei", imei);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0 && dt.Rows[0]["Warranty"] != DBNull.Value)
                {
                    lbWarranly.Text = dt.Rows[0]["Warranty"].ToString();
                }
                else
                {
                    lbWarranly.Text = "Không có thông tin";
                }
            }
            catch (Exception)
            {
                throw;
            }
           
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
