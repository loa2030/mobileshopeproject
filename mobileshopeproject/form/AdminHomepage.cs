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
    public partial class AdminHomepage : Form
    {

        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AppMobileConnection"].ToString());

        SqlCommand cmd;
        public AdminHomepage()
        {
            InitializeComponent();
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
            string companyID = ((ComboItem)cboCompany_Model.SelectedItem).Value;
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

        private void cboCompany_Mobile_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadModelByCompany(cboModel_Mobile, cboCompany_Mobile);
        }
        private void cboCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadModelByCompany(cboModel, cboCompany);
        }
    }
}
