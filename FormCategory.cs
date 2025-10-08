using StockManager.Context;
using StockManager.DataAccess.Repositories;
using StockManager.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockManager
{
    public partial class FormCategory : Form
    {
        private readonly UnitOfWork _uow;

        public FormCategory()
        {
            InitializeComponent();
            _uow = new UnitOfWork(new StockContext());
        }

        private void FormCategory_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            var list = _uow.Categories.Queryable().Select(c => new
            {
                c.Id,
                c.Name,
                Status = c.IsDeleted ? "Silinmiş" : "Aktif"
            }).ToList();

            dgvCategories.AutoGenerateColumns = true;
            dgvCategories.DataSource = list;
        }

        private void Temizle()
        {
            txtId.Clear();
            txtName.Clear();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();

            if(string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Kategori adı boş olamaz.");
                return;
            }

            var entity = new Category
            {
                Name = name
            };

            _uow.Categories.Add(entity);
            _uow.SaveChanges();

            Listele();
            Temizle();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if(!int.TryParse(txtId.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Geçersiz kategori id");
                return;
            }

            var entity = _uow.Categories.GetById(id);
            if(entity == null)
            {
                MessageBox.Show("Kategori bulunamadı.");
                return;
            }

            var name = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Kategori adı boş olamaz.");
                return;
            }

            entity.Name = name;
            _uow.Categories.Update(entity);
            _uow.SaveChanges();

            Listele();
            Temizle();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Geçersiz kategori id");
                return;
            }

            var entity = _uow.Categories.GetById(id);
            if (entity == null)
            {
                MessageBox.Show("Kategori bulunamadı.");
                return;
            }

            _uow.Categories.Remove(entity);
            _uow.SaveChanges();

            Listele();
            Temizle();
        }

        private void btnGeriAl_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Geçersiz kategori id");
                return;
            }

            var entity = _uow.Categories.GetById(id);
            if (entity == null)
            {
                MessageBox.Show("Kategori bulunamadı.");
                return;
            }

            entity.IsDeleted = false;
            _uow.Categories.Update(entity);
            _uow.SaveChanges();

            Listele();
            Temizle();
        }

        private void dgvCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0) return;
            var row = dgvCategories.Rows[e.RowIndex];

            txtId.Text = row.Cells["Id"].Value?.ToString();
            txtName.Text = row.Cells["Name"].Value?.ToString();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _uow?.Dispose();
            base.OnFormClosed(e);
        }

    }
}
