using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SatisUygulamasi
{
    public partial class FormAnaSayfa : Form
    {
        public FormAnaSayfa()
        {
            InitializeComponent();
        }

        private void satisYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSatisYap frm = new FormSatisYap();
            frm.MdiParent = this;
            frm.Show();
        }

        private void ürünListeleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUrunListele frm = new FormUrunListele();
            frm.MdiParent = this;
            frm.Show();
        }

        private void kategoriListeleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormKategoriListele frm = new FormKategoriListele();
            frm.MdiParent = this;
            frm.Show();
        }

        private void kategoriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormKategoriEkle frm = new FormKategoriEkle();
            frm.MdiParent = this;
            frm.Show();
        }

        private void ürünToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUrunEkle frm = new FormUrunEkle();
            frm.MdiParent = this;
            frm.Show();
        }

        private void yazıRengiDegiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            DialogResult result = cd.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            FormUrunEkle frm = new FormUrunEkle();
            frm.ForeColor = cd.Color;
        }
    }
}
