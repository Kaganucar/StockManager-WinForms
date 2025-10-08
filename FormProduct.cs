using StockManager.Context;
using StockManager.DataAccess.Interfaces;
using StockManager.DataAccess.Repositories;
using StockManager.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockManager
{
    public partial class FormProduct : Form
    {
        private IUnitOfWork _uow;

        public FormProduct()
        {
            InitializeComponent();
            _uow = new UnitOfWork(new StockContext());
        }

        private void FormProduct_Load(object sender, EventArgs e)
        {
            cmbKategori.Items.Clear();
            cmbDurum.Items.Add(new { Text = "Tümü", Value = (int?)null });
            cmbDurum.Items.Add(new { Text = "Aktif", Value = (int?)0 });
            cmbDurum.Items.Add(new { Text = "Silinmiş", Value = (int?)1 });
            cmbDurum.DisplayMember = "Text";
            cmbDurum.ValueMember = "Value";
            cmbDurum.SelectedIndex = 0;

            KategoriYukle();
            UrunleriListele();
        }

        private void KategoriYukle()
        {
            var categories = _uow.Categories.GetAll().ToList();
            cmbKategori.DisplayMember = "Name";
            cmbKategori.ValueMember = "Id";
            cmbKategori.DataSource = categories;

            cmbKategoriFiltrele.DisplayMember = "Name";
            cmbKategoriFiltrele.ValueMember = "Id";
            cmbKategoriFiltrele.DataSource = categories.Prepend(new Category { Id = 0, Name = "Tümü" }).ToList();
        }

        private void UrunleriListele(int? durum = null)
        {
            var q = _uow.Products.Queryable().Include(p => p.Category);

            if (durum.HasValue)
            {
                bool deleted = durum.Value == 1;
                q = q.Where(p => p.IsDeleted == deleted);
            }

            var list = q.Select(p => new
            {
                p.Id,
                p.Name,
                p.UnitPrice,
                Category = p.Category.Name,
                CategoryId = p.CategoryId,
                Status = p.IsDeleted ? "Silinmiş" : "Aktif"
            }).ToList();

            dgvProducts.DataSource = list;

            if (dgvProducts.Columns["CategoryId"] != null)
                dgvProducts.Columns["CategoryId"].Visible = false;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            var name = txtUrunAdi.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Ürün adı boş olamaz.");
                return;
            }

            if (!decimal.TryParse(txtFiyat.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Fiyat pozitif sayı olmalı");
                return;
            }

            if (!(cmbKategori.SelectedValue is int catId) || catId <= 0)
            {
                MessageBox.Show("Kategori seçmelisiniz.");
                return;
            }

            var entity = new Product
            {
                Name = name,
                UnitPrice = price,
                CategoryId = catId
            };

            _uow.Products.Add(entity);
            _uow.SaveChanges();
            UrunleriListele();
            Temizle();

        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtID.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Güncellenecek ürünü seçmelisiniz.");
                return;
            }

            var entity = _uow.Products.GetById(id);
            if (entity == null)
            {
                MessageBox.Show("Güncellenecek ürün bulunamadı.");
                return;
            }

            var name = txtUrunAdi.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Ürün adı boş olamaz.");
                return;
            }

            if (!decimal.TryParse(txtFiyat.Text, out var price) || price <= 0)
            {
                MessageBox.Show("Fiyat pozitif sayı olmalı");
                return;
            }

            if (!(cmbKategori.SelectedValue is int catId) || catId <= 0)
            {
                MessageBox.Show("Kategori seçmelisiniz.");
                return;
            }

            entity.Name = name;
            entity.UnitPrice = price;
            entity.CategoryId = catId;

            _uow.Products.Update(entity);
            _uow.SaveChanges();

            var durum = GetSelectedDurum();
            UrunleriListele();
            Temizle();

        }

        private int? GetSelectedDurum()
        {
            var sel = cmbDurum.SelectedItem;
            if (sel == null) return null;
            var prop = sel.GetType().GetProperty("Value");
            return (int?)prop.GetValue(sel, null);
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtID.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Silinecek ürünü seçmelisiniz.");
                return;
            }

            var entity = _uow.Products.GetById(id);
            if (entity == null)
            {
                MessageBox.Show("Silinecek ürün bulunamadı.");
                return;
            }

            _uow.Products.Remove(entity);
            _uow.SaveChanges();

            var durum = GetSelectedDurum();
            UrunleriListele();
            Temizle();
        }

        private void btnFiltrele_Click(object sender, EventArgs e)
        {
            int? kategoriId = null;

            if (cmbKategoriFiltrele.SelectedValue != null && int.TryParse(cmbKategoriFiltrele.SelectedValue.ToString(), out int id) && id > 0)
                kategoriId = id;

            decimal? minFiyat = null, maxFiyat = null;

            if (decimal.TryParse(txtMinPrice.Text, out var f)) minFiyat = f;
            if (decimal.TryParse(txtMaxPrice.Text, out f)) maxFiyat = f;

            var q = _uow.Products.Queryable().Include(p => p.Category);

            if (!string.IsNullOrEmpty(txtName.Text))
            {
                var ara = txtName.Text.Trim();
                q = q.Where(p => p.Name.Contains(ara));
            }

            if (kategoriId.HasValue) q = q.Where(p => p.CategoryId == kategoriId.Value);
            if (minFiyat.HasValue) q = q.Where(p => p.UnitPrice >= minFiyat.Value);
            if (maxFiyat.HasValue) q = q.Where(p => p.UnitPrice <= maxFiyat.Value);

            q = q.Where(p => !p.IsDeleted);

            dgvProducts.DataSource = q.Select(p => new
            {
                p.Id,
                p.Name,
                p.UnitPrice,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            }).ToList();

        }

        private void Temizle()
        {
            txtUrunAdi.Clear();
            txtFiyat.Clear();
            if (cmbKategori.Items.Count > 0) cmbKategori.SelectedIndex = 0;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _uow?.Dispose();
            base.OnFormClosed(e);
        }

        private void cmbDurum_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sel = cmbDurum.SelectedItem;
            var prop = sel.GetType().GetProperty("Value");
            int? durum = (int?)prop.GetValue(sel, null);
            UrunleriListele(durum);
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvProducts.Rows[e.RowIndex];

            txtID.Text = row.Cells["Id"].Value?.ToString();
            txtUrunAdi.Text = row.Cells["Name"].Value?.ToString();
            txtFiyat.Text = row.Cells["UnitPrice"].Value?.ToString();

            if (row.Cells["CategoryId"]?.Value is int selectedCatId)
            {
                cmbKategori.SelectedValue = selectedCatId;
            }
            else
            {
                var catName = row.Cells["Category"]?.Value?.ToString();
                if (!string.IsNullOrEmpty(catName))
                {
                    int idx = cmbKategori.FindStringExact(catName);
                    if (idx >= 0) cmbKategori.SelectedIndex = idx;
                }

            }
        }

        private void btnGeriAl_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtID.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Geri alınacak ürünü seçmelisiniz.");
                return;
            }

            var entity = _uow.Products.GetById(id);
            if (entity == null)
            {
                MessageBox.Show("Geri alınacak ürün bulunamadı.");
                return;
            }

            entity.IsDeleted = false;
            _uow.Products.Update(entity);
            _uow.SaveChanges();

            var durum = GetSelectedDurum();
            UrunleriListele(durum);
            Temizle();

        }

        private void btnKategoriYonet_Click(object sender, EventArgs e)
        {
            var frm = new FormCategory();
            frm.ShowDialog();
            KategoriYukle();
        }
    }
}
