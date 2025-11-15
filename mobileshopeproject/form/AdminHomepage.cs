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
            LoadCompany();
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
        void LoadCompany()
        {
            cboCompany.Items.Clear();

            cmd = new SqlCommand("SELECT CompID, CompName FROM tbl_Company", conn);
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                cboCompany.Items.Add(new ComboItem(dr["CompName"].ToString(), dr["CompID"].ToString()));
            }

            conn.Close();

            if (cboCompany.Items.Count > 0)
                cboCompany.SelectedIndex = 0;
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

        private void btnAddModel_Click(object sender, EventArgs e)
        {
            string comID = txtModelID.Text;
            string compName = txtModelNumber.Text;

            cmd = new SqlCommand(
                "INSERT INTO tbl_Model VALUES(@compID, @compName)", conn
            );

            cmd.Parameters.AddWithValue("@compID", comID);
            cmd.Parameters.AddWithValue("@compName", compName);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            AutoGenID();
            txtCompName.Clear();
        }

    }
}
