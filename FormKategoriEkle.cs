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
    public partial class FormKategoriEkle : Form
    {
        SqlConnection _cnn;

        public FormKategoriEkle()
        {
            InitializeComponent();
            _cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["BaglantiCumlecigi"].ConnectionString);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Kategori adı boş geçilemez.");
                return;
            }

            DialogResult result = MessageBox.Show("Kategori eklenecektir. Emin misiniz?", "Uyarı", MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
            {
                return;
            }


            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "insert into Categories(CategoryName,Description) values(@categoryName,@description)";
            cmd.Connection = _cnn;

            if (_cnn.State!=ConnectionState.Open)
            {
                _cnn.Open();
            }

            cmd.Parameters.AddWithValue("@categoryName", txtCategoryName.Text);
            cmd.Parameters.AddWithValue("@description", txtDescription.Text);
                        
            try
            {
                int ess = cmd.ExecuteNonQuery();

                if (ess==1)
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
    }
}
