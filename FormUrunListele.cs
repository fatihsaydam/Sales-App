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
    public partial class FormUrunListele : Form
    {
        SqlConnection _cnn;

        public FormUrunListele()
        {
            InitializeComponent();
            _cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["BaglantiCumlecigi"].ConnectionString);

        }

        private void KategoriYukle()
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

            cmbCategories.DisplayMember = "CategoryName";
            cmbCategories.ValueMember = "CategoryID";
            cmbCategories.DataSource = categories;
        }

        private void FormUrunListele_Load(object sender, EventArgs e)
        {
            KategoriYukle();                        
        }

        private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            int categoryId = (int)cmbCategories.SelectedValue;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select distinct ProductID,ProductName from Products where CategoryID="+categoryId;
            cmd.Connection = _cnn;

            if (_cnn.State != ConnectionState.Open)
            {
                _cnn.Open();
            }

            List<Product> products = new List<Product>();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Product product = new Product();
                        product.ProductID = (int)dr["ProductID"];
                        product.ProductName = dr["ProductName"] != null ? dr["ProductName"].ToString() : null;

                        products.Add(product);
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

            lstUrunListele.DisplayMember = "ProductName";
            lstUrunListele.ValueMember = "ProductID";
            lstUrunListele.DataSource = products;
        }
    }
}
