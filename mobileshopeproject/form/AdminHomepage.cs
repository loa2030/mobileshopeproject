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
            AutoGenModelID();
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
        void LoadCompany(ComboBox combo)
        {
            combo.Items.Clear();

            cmd = new SqlCommand("SELECT CompID, CompName FROM tbl_Company", conn);
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                combo.Items.Add(new ComboItem(dr["CompName"].ToString(), dr["CompID"].ToString()));
            }

            conn.Close();

            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
        }

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

        void LoadModelByCompany(string compID, ComboBox combo)
        {
            combo.Items.Clear();

            cmd = new SqlCommand("SELECT ModelID, ModelNum FROM tbl_Model WHERE CompID = @id", conn);
            cmd.Parameters.AddWithValue("@id", compID);

            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                combo.Items.Add(new ComboItem(dr["ModelNum"].ToString(), dr["ModelID"].ToString()));
            }

            conn.Close();

            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
        }



        private void cboCompany_Mobile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCompany_Mobile.SelectedItem == null) return;

            string compID = ((ComboItem)cboCompany_Mobile.SelectedItem).Value;
            LoadModelByCompany(compID, cboModel_Mobile);
        }

        private void btnInsertMobile_Click(object sender, EventArgs e)
        {
            string modelID = ((ComboItem)cboModel_Mobile.SelectedItem).Value;
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



    }
}
