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

namespace SatisUygulamasi
{
    public partial class FormKategoriListele : Form
    {
        SqlConnection _cnn;

        public FormKategoriListele()
        {
            InitializeComponent();
            _cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["BaglantiCumlecigi"].ConnectionString);
        }

        private void FormKategoriListele_Load(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select distinct CategoryID,CategoryName from Categories";
            cmd.Connection = _cnn;

            if (_cnn.State != ConnectionState.Open)
            {
                _cnn.Open();
            }

            List<Category> categories = new List<Category>();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Category category = new Category();
                        category.CategoryID = (int)dr["CategoryID"];
                        category.CategoryName = dr["CategoryName"] != null ? dr["CategoryName"].ToString() : null;

                        categories.Add(category);
                    }

                    dr.Close();
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Hata");
            }

            finally
            {
                _cnn.Close();
            }

            lstKategoriListele.DisplayMember = "CategoryName";
            lstKategoriListele.ValueMember = "CategoryID";
            lstKategoriListele.DataSource = categories;
        }
    }
}
