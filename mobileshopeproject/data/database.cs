using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mobileshopeproject.Data
{
    public static class Database
    {
        public static SqlConnection GetConnection()
        {
            try
            {
                SqlConnection conn = new SqlConnection("Server=LONG;Database=AppSaleMobile;Integrated Security=True;");

                return conn;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối DB: " + ex.Message);
                return null;
            }
        }
    }
}
