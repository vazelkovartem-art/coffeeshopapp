using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    public class ProductsForm : Form
    {
        private DataGridView grid;
        private TextBox txtSearch;
        private ComboBox cmbCategory;
        private Label lblStatus;
        private bool _loading; // чтобы программное заполнение таблицы не дёргало базу лишними UPDATE

        public ProductsForm()
        {
            Text = "Справочник товаров";
            ClientSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            Theme.StyleForm(this);

            var lblTitle = new Label
            {
                Text = "Товары", Font = Theme.TitleFont,
                ForeColor = Theme.PrimaryDark, AutoSize = true, Left = 20, Top = 15
            };

            var lblSearch = new Label { Text = "Поиск:", AutoSize = true, Left = 20, Top = 62 };
            txtSearch = new TextBox { Left = 80, Top = 58, Width = 180 };
            Theme.StyleTextBox(txtSearch);
            txtSearch.TextChanged += (s, e) => LoadData();

            var lblCat = new Label { Text = "Категория:", AutoSize = true, Left = 280, Top = 62 };
            cmbCategory = new ComboBox { Left = 365, Top = 57, Width = 190, DropDownStyle = ComboBoxStyle.DropDownList };
            FillCategories();
            cmbCategory.SelectedIndexChanged += (s, e) => { if (grid != null) LoadData(); };

            var btnAdd = new Button { Text = "Добавить", Left = 20, Top = 95, Width = 110 };
            Theme.StyleButton(btnAdd);
            btnAdd.Click += (s, e) =>
            {
                var f = new ProductEditForm(null);
                if (f.ShowDialog(this) == DialogResult.OK) { LoadData(); lblStatus.Text = "Товар добавлен"; }
            };

            var btnEdit = new Button { Text = "Изменить", Left = 140, Top = 95, Width = 110 };
            Theme.StyleButton(btnEdit);
            btnEdit.Click += (s, e) => EditSelected();

            var btnDelete = new Button { Text = "Удалить", Left = 260, Top = 95, Width = 110 };
            Theme.StyleButton(btnDelete, true);
            btnDelete.Click += (s, e) => DeleteSelected();

            var btnClose = new Button { Text = "Закрыть", Left = 770, Top = 515, Width = 110 };
            Theme.StyleButton(btnClose);
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Click += (s, e) => Close();

            grid = new DataGridView { Left = 20, Top = 140, Width = 860, Height = 360 };
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Theme.StyleGrid(grid);
            grid.Columns.Add("name", "Название");
            grid.Columns.Add("cat", "Категория");
            grid.Columns.Add("price", "Цена");
            grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "avail", HeaderText = "В наличии" });
            grid.Columns["name"].FillWeight = 40;
            grid.Columns["cat"].FillWeight = 25;
            grid.Columns["price"].FillWeight = 15;
            grid.Columns["avail"].FillWeight = 12;
            grid.Columns["price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["name"].ReadOnly = true;
            grid.Columns["cat"].ReadOnly = true;
            grid.Columns["price"].ReadOnly = true;
            // столбец "В наличии" НЕ readonly - галочка кликабельная

            grid.CellContentClick += Grid_CellContentClick;
            grid.CellValueChanged += Grid_CellValueChanged;

            lblStatus = new Label { Text = "", AutoSize = true, Left = 20, Top = 510, ForeColor = Theme.Primary };

            Controls.AddRange(new Control[] { lblTitle, lblSearch, txtSearch, lblCat, cmbCategory,
                btnAdd, btnEdit, btnDelete, btnClose, grid, lblStatus });

            LoadData();
        }

        private void FillCategories()
        {
            cmbCategory.Items.Add(new Category { Id = 0, Name = "Все категории" });
            foreach (var c in new ProductRepository().GetCategories())
                cmbCategory.Items.Add(c);
        }

        private int CurrentCategoryId()
        {
            var c = cmbCategory.SelectedItem as Category;
            return c == null ? 0 : c.Id;
        }

        private void LoadData()
        {
            _loading = true;
            var repo = new ProductRepository();
            var list = repo.GetAll(txtSearch.Text, CurrentCategoryId());
            grid.Rows.Clear();
            foreach (var p in list)
            {
                int i = grid.Rows.Add(p.Name, p.CategoryName, p.Price, p.IsAvailable);
                grid.Rows[i].Tag = p.Id;
            }
            lblStatus.Text = "Товаров: " + list.Count;
            _loading = false;
        }

        // Клик по галочке - сразу меняем значение ячейки, дальше отработает CellValueChanged
        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != grid.Columns["avail"].Index) return;

            bool cur = Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["avail"].Value ?? false);
            grid.Rows[e.RowIndex].Cells["avail"].Value = !cur;
        }

        private void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loading) return;
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != grid.Columns["avail"].Index) return;

            int id = (int)grid.Rows[e.RowIndex].Tag;
            bool val = Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["avail"].Value ?? false);

            string err = new ProductRepository().SetAvailable(id, val);
            if (err != null)
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            lblStatus.Text = string.Format("Товар «{0}»: {1}",
                grid.Rows[e.RowIndex].Cells["name"].Value,
                val ? "есть в наличии" : "нет в наличии");
        }

        private void EditSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
            {
                MessageBox.Show("Выберите товар в таблице", "Подсказка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = (int)grid.CurrentRow.Tag;

            Product found = null;
            foreach (var p in new ProductRepository().GetAll("", 0))
                if (p.Id == id) found = p;
            if (found == null) return;

            var f = new ProductEditForm(found);
            if (f.ShowDialog(this) == DialogResult.OK) { LoadData(); lblStatus.Text = "Товар обновлён"; }
        }

        private void DeleteSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
            {
                MessageBox.Show("Выберите товар в таблице", "Подсказка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = (int)grid.CurrentRow.Tag;

            if (MessageBox.Show("Удалить товар?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            string err = new ProductRepository().Delete(id);
            if (err != null)
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                lblStatus.Text = "Товар удалён";
                LoadData();
            }
        }
    }
}
