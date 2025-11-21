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
    public partial class AdminHomepage : Form
    {

        private SqlConnection conn = Database.GetConnection();

        SqlCommand cmd;
        public AdminHomepage()
        {
            InitializeComponent();
        }
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load công ty sau khi thêm
            if (tabControl2.SelectedTab == tabPageModel)
            {
                LoadCompany(cboCompany_Model);
            }
            if (tabControl2.SelectedTab == tabPageMobile)
            {
                LoadCompany(cboCompany_Mobile);
            }
            if (chiu.SelectedTab == tabPageUpdateStock)
            {
                LoadCompany(cboCompany);
            }
        }

        //tự tăng ID công ty    
        void AutoGenID()
        {
            cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(SUBSTRING(CompID, 2, LEN(CompID)) AS INT)), 0) FROM tbl_Company", conn);
            conn.Open();

            int next = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

            conn.Close();

            txtCompID.Text = "C" + next.ToString("D3");
        }

        private void AdminHomepage_Load(object sender, EventArgs e)
        {
            AutoGenID();
            LoadCompany(cboCompany_Model);
            LoadCompany(cboCompany_Mobile);
            LoadCompany(cboCompany);
            AutoGenModelID();
            GenerateTransID();

        }


        //Thêm công ty
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string comID = txtCompID.Text;
            string compName = txtCompName.Text;

            cmd = new SqlCommand(
                "INSERT INTO tbl_Company VALUES(@compID, @compName)", conn
            );

            cmd.Parameters.AddWithValue("@compID", comID);
            cmd.Parameters.AddWithValue("@compName", compName);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();


            AutoGenID();
            txtCompName.Clear();


        }

        //
        //Modal
        //
        //hiện công ty trên Model
      

        public class ComboItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public ComboItem(string text, string value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;  // Hiện CompName trên combobox
            }
        }

        void AutoGenModelID()
        {
            cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(SUBSTRING(ModelID, 2, LEN(ModelID)) AS INT)), 0) FROM tbl_Model", conn);
            conn.Open();

            int next = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

            conn.Close();

            txtModelID.Text = "M" + next.ToString("D3");
        }

        private void btnAddModel_Click(object sender, EventArgs e)
        {
            string modelID = txtModelID.Text;
            string companyID = cboCompany_Model.SelectedValue.ToString();
            string modelNumber = txtModelNumber.Text;

            cmd = new SqlCommand(
                "INSERT INTO tbl_Model (ModelID, CompID, ModelNum) VALUES (@modelID, @compID, @modelNum)",
                conn
            );

            cmd.Parameters.AddWithValue("@modelID", modelID);
            cmd.Parameters.AddWithValue("@compID", companyID);
            cmd.Parameters.AddWithValue("@modelNum", modelNumber);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            LoadCompany(cboCompany_Model);
            LoadCompany(cboCompany_Mobile);

            AutoGenModelID();
            txtModelNumber.Clear();

        }

        //
        //Mobile
        //

        private void btnInsertMobile_Click(object sender, EventArgs e)
        {
            string modelID = cboModel_Mobile.SelectedValue.ToString();
            string imei = txtIMEI.Text;
            string price = txtPrice.Text;
            DateTime warranty = dtpWarranty.Value;

            string status = "Not sold";

            cmd = new SqlCommand(
                "INSERT INTO tbl_Mobile (ModelID, IMEINO, Price, Warranty, Status) " +
                "VALUES (@modelID, @imei, @price, @warranty, @status)", conn);

            cmd.Parameters.AddWithValue("@modelID", modelID);
            cmd.Parameters.AddWithValue("@imei", imei);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@warranty", warranty);
            cmd.Parameters.AddWithValue("@status", status);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Thêm Mobile thành công!", "Thông báo");
        }
        //
        //
        //UpdateStock
        //
        //
        private void GenerateTransID()
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT 'T' + RIGHT('000' + CAST(ISNULL(MAX(CAST(SUBSTRING(TransID, 2, LEN(TransID)) AS INT)), 0) + 1 AS VARCHAR(10)), 3) " +
                "FROM tbl_Transaction", conn))
            {
                conn.Open();
                txtTransID.Text = cmd.ExecuteScalar().ToString();
                conn.Close();
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string transID = txtTransID.Text;
            string compID = cboCompany.SelectedValue.ToString();
            string modelID = cboModel.SelectedValue.ToString();
            int qty = int.Parse(txtQuantity.Text);
            decimal amount = decimal.Parse(txtAmount.Text);

            //Thêm transid
            SqlCommand cmd1 = new SqlCommand(
                "INSERT INTO tbl_Transaction (TransID, ModelID, Quantity, Amount) " +
                "VALUES (@t, @m, @q, @a)", conn);

            cmd1.Parameters.AddWithValue("@t", transID);
            cmd1.Parameters.AddWithValue("@m", modelID);
            cmd1.Parameters.AddWithValue("@q", qty);
            cmd1.Parameters.AddWithValue("@a", amount);

            //cập nhật số lượng cho bảng model
            SqlCommand cmd2 = new SqlCommand(
                "UPDATE tbl_Model SET AvailableQty = AvailableQty + @q WHERE ModelID = @m", conn);

            cmd2.Parameters.AddWithValue("@q", qty);
            cmd2.Parameters.AddWithValue("@m", modelID);

            conn.Open();
            cmd1.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Stock updated successfully!");

            
            GenerateTransID();
        }


        void LoadModelByCompany(ComboBox modelBox, ComboBox companyBox)
        {
            if (companyBox.SelectedValue == null || companyBox.SelectedValue is DataRowView) return;

            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT ModelID, ModelNum FROM tbl_Model WHERE CompID = @cid", conn);

            da.SelectCommand.Parameters.AddWithValue("@cid", companyBox.SelectedValue.ToString());

            DataTable dt = new DataTable();
            da.Fill(dt);

            modelBox.DataSource = dt;
            modelBox.DisplayMember = "ModelNum";
            modelBox.ValueMember = "ModelID";
        }

        void LoadCompany(ComboBox combo)
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT CompID, CompName FROM tbl_Company", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            combo.DataSource = dt;
            combo.DisplayMember = "CompName";
            combo.ValueMember = "CompID";
        }

        private void cboModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboModel.SelectedValue == null) return;

            SqlCommand cmd = new SqlCommand(
                "SELECT Price FROM tbl_Mobile WHERE ModelId = @id", conn);
            cmd.Parameters.AddWithValue("@id", cboModel.SelectedValue.ToString());

            conn.Open();
            object result = cmd.ExecuteScalar();
            conn.Close();

            if (result != null)
            {
                decimal price = Convert.ToDecimal(result);
                txtAmount.Tag = price; // lưu price lại để tính Amount
            }
        }
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtQuantity.Text, out decimal qty) &&
                txtAmount.Tag != null)
            {
                decimal price = (decimal)txtAmount.Tag;
                txtAmount.Text = (qty * price).ToString();
            }
        }


        private void cboCompany_Mobile_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadModelByCompany(cboModel_Mobile, cboCompany_Mobile);
        }
        private void cboCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadModelByCompany(cboModel, cboCompany);
        }

        //
        //report
        //
        //Day
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string date = dtpDate.Value.ToString("yyyy-MM-dd");

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"SELECT 
                        s.SlsId,
                        c.CompName AS CompanyName,
                        m.ModelNum,
                        s.IMEINO,
                        s.Price
                    FROM tbl_Sales s
                    JOIN tbl_Mobile mb ON s.IMEINO = mb.IMEINO
                    JOIN tbl_Model m ON mb.ModelId = m.ModelId
                    JOIN tbl_Company c ON m.CompId = c.CompId
                    WHERE CAST(s.PurchaseDate AS DATE) = @SaleDate;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SaleDate", date);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvSalesReport.DataSource = dt;

                // Tính tổng tiền
                int total = dt.AsEnumerable().Sum(r => Convert.ToInt32(r["Price"]));

                lblTotal.Text = $"Total Sales Amount = {total:N0}";
            }
        }

        //day to day
        private void btnSearch_Click_daytoday(object sender, EventArgs e)
        {
            DateTime startDate = dtpStart.Value.Date;
            DateTime endDate = dtpEnd.Value.Date;

            using (SqlConnection conn = Database.GetConnection())
            {
                string query = @"
                    SELECT 
                        s.SlsId,
                        c.CompName AS CompanyName,
                        m.ModelNum,
                        s.IMEINO,
                        s.Price,
                        s.PurchaseDate
                    FROM tbl_Sales s
                    JOIN tbl_Mobile mo ON s.IMEINO = mo.IMEINO
                    JOIN tbl_Model m ON mo.ModelId = m.ModelId
                    JOIN tbl_Company c ON m.CompId = c.CompId
                    WHERE s.PurchaseDate BETWEEN @start AND @end
                    ORDER BY s.PurchaseDate;
        ";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@start", startDate);
                da.SelectCommand.Parameters.AddWithValue("@end", endDate);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvSalesReportDtD.DataSource = dt;

                // Tính tổng
                if (dt.Rows.Count > 0)
                {
                    decimal total = dt.AsEnumerable()
                  .Sum(r => Convert.ToDecimal(r["Price"]));
                    lblTotald2d.Text = $"Total Sales Amount = {total:N0}";
                }
                else
                {
                    lblTotald2d.Text = "Total Sales Amount = 0";
                }
            }
        }

        //
        //Emplooyee
        //

        private void btnAdd_Empe_Click(object sender, EventArgs e)
        {
            string empName = txtEmpName.Text.Trim();
            string address = txtAddress.Text.Trim();
            string mobile = txtMobile.Text.Trim();
            string user = txtUserName.Text.Trim();
            string pwd = txtPwd.Text.Trim();
            string repwd = txtRePwd.Text.Trim();
            string hint = txtHint.Text.Trim();

            // Validate
            if (empName == "" || address == "" || mobile == "" ||
                user == "" || pwd == "" || repwd == "")
            {
                MessageBox.Show("Please fill all fields!");
                return;
            }

            if (pwd != repwd)
            {
                MessageBox.Show("Password mismatch!");
                return;
            }

            SqlConnection conn = Database.GetConnection();

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO tbl_User (UserName, PWD, EmployeeName, Address, MobileNumber, Hint)
              VALUES (@u, @p, @e, @a, @m, @h)", conn);

                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@p", pwd);
                cmd.Parameters.AddWithValue("@e", empName);
                cmd.Parameters.AddWithValue("@a", address);
                cmd.Parameters.AddWithValue("@m", mobile);
                cmd.Parameters.AddWithValue("@h", hint);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Employee added successfully!");

                // Clear fields
                txtEmpName.Clear();
                txtAddress.Clear();
                txtMobile.Clear();
                txtUserName.Clear();
                txtPwd.Clear();
                txtRePwd.Clear();
                txtHint.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

    }
}
