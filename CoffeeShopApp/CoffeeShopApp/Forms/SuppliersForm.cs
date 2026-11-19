using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    public class SuppliersForm : Form
    {
        private DataGridView grid;
        private Label lblStatus;

        public SuppliersForm()
        {
            Text = "Справочник поставщиков";
            ClientSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            Theme.StyleForm(this);

            var lblTitle = new Label
            {
                Text = "Поставщики", Font = Theme.TitleFont,
                ForeColor = Theme.PrimaryDark, AutoSize = true, Left = 20, Top = 15
            };

            var btnAdd = new Button { Text = "Добавить", Left = 20, Top = 70, Width = 110 };
            Theme.StyleButton(btnAdd);
            btnAdd.Click += (s, e) =>
            {
                var f = new SupplierEditForm(null);
                if (f.ShowDialog(this) == DialogResult.OK) { LoadData(); lblStatus.Text = "Поставщик добавлен"; }
            };

            var btnEdit = new Button { Text = "Изменить", Left = 140, Top = 70, Width = 110 };
            Theme.StyleButton(btnEdit);
            btnEdit.Click += (s, e) => EditSelected();

            var btnDelete = new Button { Text = "Удалить", Left = 260, Top = 70, Width = 110 };
            Theme.StyleButton(btnDelete, true);
            btnDelete.Click += (s, e) => DeleteSelected();

            var btnClose = new Button { Text = "Закрыть", Left = 770, Top = 515, Width = 110 };
            Theme.StyleButton(btnClose);
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Click += (s, e) => Close();

            grid = new DataGridView { Left = 20, Top = 115, Width = 860, Height = 385 };
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Theme.StyleGrid(grid);
            grid.Columns.Add("name", "Название");
            grid.Columns.Add("phone", "Телефон");
            grid.Columns.Add("city", "Город");
            grid.Columns["name"].FillWeight = 45;
            grid.Columns["phone"].FillWeight = 30;
            grid.Columns["city"].FillWeight = 25;
            foreach (DataGridViewColumn col in grid.Columns) col.ReadOnly = true;

            lblStatus = new Label { Text = "", AutoSize = true, Left = 20, Top = 510, ForeColor = Theme.Primary };

            Controls.AddRange(new Control[] { lblTitle, btnAdd, btnEdit, btnDelete, btnClose, grid, lblStatus });

            LoadData();
        }

        private void LoadData()
        {
            var repo = new SupplierRepository();
            var list = repo.GetAll();
            grid.Rows.Clear();
            foreach (var s in list)
            {
                int i = grid.Rows.Add(s.Name, s.Phone, s.City);
                grid.Rows[i].Tag = s.Id;
            }
            lblStatus.Text = "Поставщиков: " + list.Count;
        }

        private void EditSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
            {
                MessageBox.Show("Выберите поставщика в таблице", "Подсказка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = (int)grid.CurrentRow.Tag;

            Models.Supplier found = null;
            foreach (var s in new SupplierRepository().GetAll())
                if (s.Id == id) found = s;
            if (found == null) return;

            var f = new SupplierEditForm(found);
            if (f.ShowDialog(this) == DialogResult.OK) { LoadData(); lblStatus.Text = "Поставщик обновлён"; }
        }

        private void DeleteSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
            {
                MessageBox.Show("Выберите поставщика в таблице", "Подсказка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = (int)grid.CurrentRow.Tag;

            if (MessageBox.Show("Удалить поставщика?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            string err = new SupplierRepository().Delete(id);
            if (err != null)
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                lblStatus.Text = "Поставщик удалён";
                LoadData();
            }
        }
    }
}
