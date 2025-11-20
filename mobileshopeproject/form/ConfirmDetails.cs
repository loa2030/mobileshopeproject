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
using System.Xml.Linq;
using mobileshopeproject.Data;

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
        private SqlConnection conn = Database.GetConnection();
        public ConfirmDetails(string username, string company, string modelNumber,
        string imei,
        string price, string customer, string Mbnumer, string add, string email)
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
        private void Load_detail()
        {
            lbCus.Text = customer;
            lbMN.Text = Mobilenumber;
            lbAdd.Text = address;
            lbEmail.Text = Email;
            lbCN.Text = companyN;
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
        private string GenerateCustomerID()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(CustId) FROM tbl_Customer", conn);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                {
                    string cusIDmax = result.ToString();   // ví dụ CU005
                    int number = int.Parse(cusIDmax.Substring(2));
                    number++;
                    return "CU" + number.ToString("D3");
                }

                return "CU001";
            }
        }
        private string GenerateSalesID()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(SlsId) FROM tbl_Sales", conn);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                {
                    string max = result.ToString();  // S005
                    int number = int.Parse(max.Substring(1));
                    number++;
                    return "S" + number.ToString("D3");
                }

                return "S001";
            }
        }
        private string CheckExistingCustomer(string mobile)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT CustId FROM tbl_Customer WHERE MobNumber = @m", conn);

                cmd.Parameters.AddWithValue("@m", mobile);

                conn.Open();
                object cid = cmd.ExecuteScalar();
                return cid == DBNull.Value ? null : cid?.ToString();
            }
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            string cidcust = CheckExistingCustomer(Mobilenumber);
            string cidsal = GenerateSalesID();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    if (cidcust == null)
                    {
                        cidcust = GenerateCustomerID();

                        SqlCommand cmdCust = new SqlCommand(
                            "INSERT INTO tbl_Customer (CustId, Cust_Name, MobNumber, EmailId, Address) " +
                            "VALUES (@id, @name, @mobile, @email, @address)", conn, tran);

                        cmdCust.Parameters.AddWithValue("@id", cidcust);
                        cmdCust.Parameters.AddWithValue("@name", customer);
                        cmdCust.Parameters.AddWithValue("@mobile", Mobilenumber);
                        cmdCust.Parameters.AddWithValue("@email", Email);
                        cmdCust.Parameters.AddWithValue("@address", address);

                        cmdCust.ExecuteNonQuery();
                    }

                    SqlCommand cmdSales = new SqlCommand(
                        "INSERT INTO tbl_Sales (SlsId, IMEINO, PurchaseDate, Price, CustId) " +
                        "VALUES (@sid, @imei, @date, @price, @cid)", conn, tran);

                    cmdSales.Parameters.AddWithValue("@sid", cidsal);
                    cmdSales.Parameters.AddWithValue("@imei", imei);
                    cmdSales.Parameters.AddWithValue("@cid", cidcust);
                    cmdSales.Parameters.AddWithValue("@price", price);
                    cmdSales.Parameters.AddWithValue("@date", DateTime.Now);

                    cmdSales.ExecuteNonQuery();

                    // Update mobile
                    SqlCommand cmdUpdateMobile = new SqlCommand(
                        "UPDATE tbl_Mobile SET Status='Sold' WHERE IMEINO=@imei", conn, tran);

                    cmdUpdateMobile.Parameters.AddWithValue("@imei", imei);
                    cmdUpdateMobile.ExecuteNonQuery();

                    // Update AvailableQty
                    SqlCommand cmdUpdateQty = new SqlCommand(
                        "UPDATE tbl_Model SET AvailableQty = AvailableQty - 1 " +
                        "WHERE ModelId = (SELECT ModelId FROM tbl_Mobile WHERE IMEINO=@imei)", conn, tran);

                    cmdUpdateQty.Parameters.AddWithValue("@imei", imei);
                    cmdUpdateQty.ExecuteNonQuery();

                    tran.Commit();
                    MessageBox.Show("Đã bán hàng thành công!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
    }
}
