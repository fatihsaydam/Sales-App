using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SatisUygulamasi
{
    public partial class FormSatisYap : Form
    {
        SqlConnection _cnn;

        Random rnd,rndd;

        decimal toplamTutar = 0;

        decimal toplamFiyatlar = 0;
        decimal _toplamFiyat = 0;
        decimal _stokSayisi=0;

        public FormSatisYap()
        {
            InitializeComponent();
            _cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["BaglantiCumlecigi"].ConnectionString);
            rnd = new Random();
            rndd = new Random();
        }


        private void ProductListYukle()
        {
            int categoryId = (int)cmbCategory.SelectedValue;
            SqlCommand cmd = new SqlCommand("select * from Products where CategoryID=" + categoryId, _cnn);

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
                        product.Discontinued = (bool)dr["Discontinued"];
                        product.CategoryID = (int?)dr["CategoryID"];
                        product.UnitPrice = (decimal?)dr["UnitPrice"];
                        product.UnitsInStock = (short?)dr["UnitsInStock"];

                        products.Add(product);
                    }
                }

                dr.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("hata");
            }
            finally
            {
                _cnn.Close();
            }

            cmbProduct.DisplayMember = "ProductName";
            cmbProduct.ValueMember = "ProductID";
            cmbProduct.DataSource = products;
        }


        private void UnitPriceYukle()
        {
            int productId = (int)cmbProduct.SelectedValue;
            SqlCommand cmd = new SqlCommand("select cast(UnitPrice as decimal(18,2)) from Products where ProductID=" + productId, _cnn);

            if (_cnn.State != ConnectionState.Open)
            {
                _cnn.Open();
            }

            try
            {
                lblUnitPriceValue.Text = cmd.ExecuteScalar().ToString();
            }
            catch (Exception)
            {

                MessageBox.Show("hata");
            }

            finally
            {
                _cnn.Close();
            }
        }

        private void FormSatisYap_Load(object sender, EventArgs e)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select * from Categories";
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
                        category.CategoryName = dr["CategoryName"].ToString();
                        categories.Add(category);
                    }
                }

                dr.Close();
            }
            catch (Exception)
            {

                throw;
            }

            finally
            {
                _cnn.Close();
            }

            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryID";
            cmbCategory.DataSource = categories;
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProductListYukle();
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnitPriceYukle();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            int productId = (int)cmbProduct.SelectedValue;
            SqlCommand cmd = new SqlCommand("select UnitsInStock from Products where ProductID=" + productId, _cnn);

            if (_cnn.State != ConnectionState.Open)
            {
                _cnn.Open();
            }

            decimal stokSayisi = Convert.ToInt32(cmd.ExecuteScalar());
            _stokSayisi = stokSayisi;

            try
            {
                if (nudQuantity.Value == 0)
                {
                    MessageBox.Show("Adet sayısı 0 olamaz. Lütfen 0'dan farklı bir sayı giriniz.");
                    return;
                }

                if (stokSayisi < nudQuantity.Value)
                {
                    MessageBox.Show("Seçtiğiniz " + cmbProduct.Text + " ürününden " + stokSayisi + " adet stoklarımızda bulunmaktadır. Lütfen " + stokSayisi + " adet veya daha az adet giriniz.");
                    return;
                }

                else
                {
                    DialogResult result = MessageBox.Show("Seçtiğiniz ürün listeye eklenecektir. Emin misiniz?", "Uyarı", MessageBoxButtons.YesNo);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }

                    foreach (ListViewItem itm in lstSatisListele.Items)
                    {
                        for (int i = 0; i < lstSatisListele.Items.Count; i++)
                        {
                            if (itm.SubItems[i].Text == cmbProduct.Text)
                            {
                                int value = int.Parse(this.lstSatisListele.Items[i].SubItems[1].Text);
                                lstSatisListele.Items[i].SubItems[1].Text = ((int)nudQuantity.Value + (value)).ToString();
                                lstSatisListele.Items[i].SubItems[3].Text = ((Convert.ToDecimal(lstSatisListele.Items[i].SubItems[1].Text)) * (Convert.ToDecimal(lstSatisListele.Items[i].SubItems[2].Text))).ToString();
                                toplamFiyatlar = decimal.Parse(lstSatisListele.Items[i].SubItems[3].Text);
                                toplamTutar = toplamFiyatlar + _toplamFiyat;
                                lblToplamTutarValue.Text = toplamTutar.ToString();
                                return;
                            }
                        }

                    }

                    decimal birimFiyat = Convert.ToDecimal(lblUnitPriceValue.Text);
                    decimal toplamFiyat = (nudQuantity.Value) * birimFiyat;
                    _toplamFiyat = toplamFiyat;

                    string[] urunDetaylarim =
                    {
                        cmbProduct.Text.ToString(),
                        nudQuantity.Value.ToString(),
                        lblUnitPriceValue.Text,
                        toplamFiyat.ToString()
                    };

                    ListViewItem item = new ListViewItem(urunDetaylarim);
                    item.Tag = cmbProduct;
                    lstSatisListele.Items.Add(item);

                    toplamFiyatlar = decimal.Parse(item.SubItems[3].Text);
                    toplamTutar += toplamFiyatlar;
                    lblToplamTutarValue.Text = toplamTutar.ToString();
                }

                SqlCommand cmdd = new SqlCommand();
                cmd.Connection = _cnn;
                cmd.CommandText = "update Products set " +
                    "UnitsInStock=@UnitsInStock" +
                    " where ProductID=@ProductID";

                cmd.Parameters.AddWithValue("@UnitsInStock", (stokSayisi - nudQuantity.Value));
                cmd.Parameters.AddWithValue("@ProductID", productId);

                int ess = cmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {

                MessageBox.Show("hata");
            }

            finally
            {
                _cnn.Close();
            }
        }

        private void btnCıkar_Click(object sender, EventArgs e)
        {
            if (lstSatisListele.SelectedIndices.Count == 0)
            {
                return;
            }

            DialogResult _result = MessageBox.Show("Seçtiğiniz ürün(ler) çıkarılacaktır. Emin misiniz?", "Uyarı", MessageBoxButtons.YesNo);


            if (_result != DialogResult.Yes)
            {
                return;
            }

            foreach (ListViewItem item in lstSatisListele.Items)
            {
                if (item.Selected)
                {
                    lstSatisListele.Items.Remove(item);

                    toplamTutar -= decimal.Parse(item.SubItems[3].Text);

                    lblToplamTutarValue.Text = toplamTutar.ToString();

                    string productName = "'"+item.SubItems[0].Text+"'";

                    _cnn.Open();

                    SqlCommand cmdd = new SqlCommand();
                    cmdd.Connection = _cnn;
                    cmdd.CommandText = "select UnitsInStock from Products where ProductName=" + productName;                    
                    decimal guncellenmisStok=Convert.ToDecimal(cmdd.ExecuteScalar());

                    _cnn.Close();

                    _cnn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = _cnn;
                    cmd.CommandText = "update Products set " +
                    "UnitsInStock=@UnitsInStock" +
                    " where ProductName=@ProductName";
                    cmd.Parameters.AddWithValue("@UnitsInStock", guncellenmisStok+Convert.ToDecimal(item.SubItems[1].Text));
                    cmd.Parameters.AddWithValue("@ProductName", item.SubItems[0].Text);
                    int ess = cmd.ExecuteNonQuery();

                    _cnn.Close();                    
                }
            }
        }

        private void btnTumunuTemizle_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Tüm ürünler temizlenecektir. Emin misiniz?", "Uyarı", MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
            {
                return;
            }

            foreach (ListViewItem item in lstSatisListele.Items)
            {
                lstSatisListele.Items.Remove(item);

                toplamTutar -= decimal.Parse(item.SubItems[3].Text);

                lblToplamTutarValue.Text = toplamTutar.ToString();

                string productName = "'" + item.SubItems[0].Text + "'";

                _cnn.Open();

                SqlCommand cmdd = new SqlCommand();
                cmdd.Connection = _cnn;
                cmdd.CommandText = "select UnitsInStock from Products where ProductName=" + productName;
                decimal guncellenmisStok = Convert.ToDecimal(cmdd.ExecuteScalar());

                _cnn.Close();

                _cnn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _cnn;
                cmd.CommandText = "update Products set " +
                "UnitsInStock=@UnitsInStock" +
                " where ProductName=@ProductName";
                cmd.Parameters.AddWithValue("@UnitsInStock", guncellenmisStok + Convert.ToDecimal(item.SubItems[1].Text));
                cmd.Parameters.AddWithValue("@ProductName", item.SubItems[0].Text);
                int ess = cmd.ExecuteNonQuery();

                _cnn.Close();                
            }

        }

        private void btnTamamla_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Ürün satın alma işlemi tamamlanacaktır.Emin misiniz?", "Uyarı", MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
            {
                return;
            }

            if (_cnn.State != ConnectionState.Open)
            {
                _cnn.Open();
            }

            foreach (ListViewItem item in lstSatisListele.Items)
            {
                string productName = "'" + item.SubItems[0].Text + "'";                

                SqlCommand cmddd = new SqlCommand();
                cmddd.Connection = _cnn;
                cmddd.CommandText = "select ProductID from Products where ProductName=" + productName;
                int productId = Convert.ToInt32(cmddd.ExecuteScalar());
                cmddd.CommandText= "insert into [Order Details](OrderID,ProductID,UnitPrice,Quantity,Discount) values(@OrderID,@ProductID,@UnitPrice,@Quantity,@Discount)";
                int rndOrder = rnd.Next(10248, 11078);
                double rndDiscount = rndd.NextDouble();
                rndDiscount = Math.Round(rndDiscount, 2);
                cmddd.Parameters.AddWithValue("@OrderID",rndOrder);
                cmddd.Parameters.AddWithValue("@UnitPrice", Convert.ToDecimal(item.SubItems[2].Text));
                cmddd.Parameters.AddWithValue("@Quantity", Convert.ToInt32(item.SubItems[1].Text));
                cmddd.Parameters.AddWithValue("@ProductID", productId);
                cmddd.Parameters.AddWithValue("@Discount", rndDiscount);

                try
                {
                    int ess = cmddd.ExecuteNonQuery();

                    if (ess==1)
                    {
                        MessageBox.Show("Satış(lar) başarıyla tamamlandı.");
                    }

                    else
                    {
                        MessageBox.Show("Satış tamamlama işlemi başarısız.");
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("Bir nedenden dolayı satış tamamlanamadı.");
                }

                finally
                {
                    _cnn.Close();
                }
            }            
        }
    }
}
