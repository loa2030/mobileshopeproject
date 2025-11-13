using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mobileshopeproject.form
{
    public partial class UserHomepage : Form
    {
        string username;
        string customer;
        string companyN;
        string modelNumber;
        string imei;
        string price;
        string Mobilenumber;
        string address;
        string Email;
        SqlConnection conn = new SqlConnection(@"Server=LAPTOP-86BEM0IS;Database=AppMobile;Integrated Security=True");
        public UserHomepage(string user)
        {
            InitializeComponent();
            username = user;
            LoadCompaines();
            WireEvents();
        }
        private void WireEvents()
        {
            cbCompName.SelectedIndexChanged += cbCompName_SelectedIndexChanged;
            //cbMdNumber.SelectedIndexChanged += cbMdNumber_SelectedIndexChanged;
            //cbIMEINumber.SelectedIndexChanged += cbIMEINumber_SelectedIndexChanged;
        }
        private void LoadCompaines()
        {
            try
            {
                conn.Open();
                string query = "SELECT CompId, CompName FROM tbl_Company";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbCompName.DataSource = dt;
                cbCompName.DisplayMember = "CompName";
                cbCompName.ValueMember = "CompId";
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu!" + ex.Message);
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
        private void cbCompName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCompName.SelectedValue == null || cbCompName.SelectedValue == DBNull.Value)
                return;

            try
            {
                companyN = cbCompName.Text;
                string compId = cbCompName.SelectedValue.ToString();
                LoadModels(compId); // Truyền string
                cbMdNumber.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }

        }

        private void LoadModels(string id)
        {
            try
            {
                conn.Open();
                string query = "SELECT ModelNum,ModelId FROM tbl_Model WHERE CompID = @CompanyID";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@CompanyID", id);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cbMdNumber.SelectedIndexChanged -= cbMdNumber_SelectedIndexChanged;
                cbMdNumber.DataSource = dt;
                cbMdNumber.DisplayMember = "ModelNum";
                cbMdNumber.ValueMember = "ModelId";
                modelNumber=cbMdNumber.Text;

                //cbMdNumber.SelectedIndexChanged += cbMdNumber_SelectedIndexChanged;
                cbIMEINumber.DataSource = null;
                cbIMEINumber.Enabled = false;
                txtPP.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách model 1:" + ex.Message);
            }
            finally
            {
                conn.Close();
                cbMdNumber.SelectedIndexChanged += cbMdNumber_SelectedIndexChanged;
            }
        }

        private void cbMdNumber_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if(cbMdNumber.SelectedValue!=null && cbMdNumber.SelectedValue != DBNull.Value)
            {
                try
                {
                    string modelID = cbMdNumber.SelectedValue.ToString();
                    LoadIMEI(modelID); // Truyền string
                    cbIMEINumber.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi:"+ex.Message);
                }
            }
        }

        private void LoadIMEI(string modelID)
        {
            try
            {
                conn.Open();
                string query = "SELECT IMEINO, Price FROM tbl_Mobile WHERE ModelId = @ModelID AND Status = 'Available'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@ModelID", modelID);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbIMEINumber.DataSource = dt;
                cbIMEINumber.DisplayMember = "IMEINO";
                cbIMEINumber.ValueMember = "IMEINO";
                imei=cbIMEINumber.Text;
       
                txtPP.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("lỗi tải danh sách IMEI:" + ex.Message);
                throw;
            }
            finally
            {
                conn.Close();
                cbIMEINumber.SelectedIndexChanged += cbIMEINumber_SelectedIndexChanged;
            }
        }

        private void cbIMEINumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIMEINumber.SelectedItem is DataRowView row)
            {
                txtPP.Text = row["Price"].ToString();
            }
            price=txtPP.Text;
        }

        private void tabSales_Click(object sender, EventArgs e)
        {

        }

        private void btnSubmitUP_Click(object sender, EventArgs e)
        {
            customer=txtCusN.Text;
            Mobilenumber=txtMNumber.Text;
            address=txtAdd.Text;
            Email=txtEmailID.Text;
            try
            {
                if (ValidateSalesForm())
                {
                    conn.Open();

                    MessageBox.Show("Thêm hóa đơn thành công!", "Thông báo");

                    try
                    {
                        
                        ConfirmDetails userForm = new ConfirmDetails(username, companyN, modelNumber, imei, price, customer, Mobilenumber, address, Email);
                        userForm.Show();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("Lỗi khi mở ConfirmDetails: " + ex2.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }


        }
        private bool ValidateSalesForm()
        {
            if (string.IsNullOrWhiteSpace(txtCusN.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!");
                return false;
            }

            if (cbIMEINumber.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn IMEI số!");
                return false;
            }

            return true;
        }



        private void ClearSalesForm()
        {
            txtCusN.Clear();
            txtMNumber.Clear();
            txtAdd.Clear();
            txtEmailID.Clear();
            txtPP.Clear();

            cbCompName.SelectedIndex = -1;
            cbMdNumber.DataSource = null;
            cbIMEINumber.DataSource = null;
            cbMdNumber.Enabled = false;
            cbIMEINumber.Enabled = false;
        }
    }
}
