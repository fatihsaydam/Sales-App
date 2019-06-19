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
    public partial class FormUrunEkle : Form
    {
        SqlConnection _cnn;

        public FormUrunEkle()
        {
            InitializeComponent();
            _cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["BaglantiCumlecigi"].ConnectionString);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Ürün adı boş geçilemez.");
                return;
            }

            DialogResult result = MessageBox.Show("Ürün eklenecektir. Emin misiniz?", "Uyarı", MessageBoxButtons.YesNo);

            if (result!=DialogResult.Yes)
            {
                return;
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "insert into Products(ProductName,CategoryID,UnitsInStock,UnitPrice,Discontinued) values(@ProductName,@CategoryID,@UnitsInStock,@UnitPrice,@Discontinued)";
            cmd.Connection = _cnn;

            if (_cnn.State != ConnectionState.Open)
            {
                _cnn.Open();
            }

            cmd.Parameters.AddWithValue("@ProductName", txtProductName.Text);
            cmd.Parameters.AddWithValue("@CategoryID", (int)cmbCategory.SelectedValue);
            cmd.Parameters.AddWithValue("@Discontinued", chbDiscontinued.Checked);
            cmd.Parameters.AddWithValue("@UnitsInStock", nudUnitsInStock.Value);
            cmd.Parameters.AddWithValue("@UnitPrice", txtUnitPrice.Text);

            try
            {
                int ess = cmd.ExecuteNonQuery();

                if (ess == 1)
                {
                    MessageBox.Show("Kayıt başarılıdır.");
                }

                else
                {
                    MessageBox.Show("Kayıt başarısız olmuştur. Lütfen gerekli kontrolleri yaparak tekrar deneyiniz.");
                }

            }
            catch (Exception)
            {

                MessageBox.Show("Bir hatadan dolayı kayıt yapılamadı.");
            }

            finally
            {
                _cnn.Close();
            }
        }

        private void FormUrunEkle_Load(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("select CategoryID,CategoryName from Categories", _cnn);

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

            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryID";
            cmbCategory.DataSource = categories;

        }
    }
}
