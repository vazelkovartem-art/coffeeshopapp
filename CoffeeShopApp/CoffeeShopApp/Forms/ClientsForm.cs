using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    public class ClientsForm : Form
    {
        private DataGridView grid;
        private TextBox txtSearch;
        private CheckBox chkOnlyDiscount;
        private ComboBox cmbSort;
        private Label lblStatus;

        public ClientsForm()
        {
            Text = "Справочник клиентов";
            ClientSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            Theme.StyleForm(this);

            var lblTitle = new Label
            {
                Text = "Клиенты", Font = Theme.TitleFont,
                ForeColor = Theme.PrimaryDark, AutoSize = true, Left = 20, Top = 15
            };

            var lblSearch = new Label { Text = "Поиск по фамилии:", AutoSize = true, Left = 20, Top = 62 };
            txtSearch = new TextBox { Left = 145, Top = 58, Width = 200 };
            Theme.StyleTextBox(txtSearch);
            txtSearch.TextChanged += (s, e) => LoadData();

            chkOnlyDiscount = new CheckBox { Text = "Только со скидкой", AutoSize = true, Left = 365, Top = 60 };
            chkOnlyDiscount.CheckedChanged += (s, e) => { if (grid != null) LoadData(); };

            cmbSort = new ComboBox { Left = 520, Top = 57, Width = 210, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSort.Items.AddRange(new object[]
            {
                "По фамилии (А-Я)", "По фамилии (Я-А)", "По скидке (по убыв.)", "По дате регистрации"
            });
            cmbSort.SelectedIndexChanged += (s, e) => { if (grid != null) LoadData(); };

            var btnAdd = new Button { Text = "Добавить", Left = 20, Top = 95, Width = 110 };
            Theme.StyleButton(btnAdd);
            btnAdd.Click += (s, e) =>
            {
                var f = new ClientEditForm(null);
                if (f.ShowDialog(this) == DialogResult.OK) { LoadData(); lblStatus.Text = "Клиент добавлен"; }
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
            grid.Columns.Add("fio", "ФИО");
            grid.Columns.Add("phone", "Телефон");
            grid.Columns.Add("email", "E-mail");
            grid.Columns.Add("disc", "Скидка, %");
            grid.Columns["fio"].FillWeight = 45;
            grid.Columns["phone"].FillWeight = 25;
            grid.Columns["email"].FillWeight = 20;
            grid.Columns["disc"].FillWeight = 10;
            grid.Columns["disc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            foreach (DataGridViewColumn col in grid.Columns) col.ReadOnly = true;

            lblStatus = new Label { Text = "", AutoSize = true, Left = 20, Top = 510, ForeColor = Theme.Primary };

            Controls.AddRange(new Control[] { lblTitle, lblSearch, txtSearch, chkOnlyDiscount, cmbSort,
                btnAdd, btnEdit, btnDelete, btnClose, grid, lblStatus });

            cmbSort.SelectedIndex = 0; // последнее: вызовет LoadData, когда grid уже есть
        }

        private void LoadData()
        {
            var repo = new ClientRepository();
            var list = repo.GetAll(txtSearch.Text, chkOnlyDiscount.Checked, cmbSort.SelectedIndex);
            grid.Rows.Clear();
            foreach (var c in list)
            {
                int i = grid.Rows.Add(c.FullName, c.Phone, c.Email, c.Discount);
                grid.Rows[i].Tag = c.Id; // id держим в Tag строки
            }
            lblStatus.Text = "Записей: " + list.Count;
        }

        private void EditSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
            {
                MessageBox.Show("Выберите клиента в таблице", "Подсказка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = (int)grid.CurrentRow.Tag;

            // Ищем клиента по id (вдруг список уже отфильтрован)
            Client found = null;
            foreach (var c in new ClientRepository().GetAll("", false, 0))
                if (c.Id == id) found = c;
            if (found == null) return;

            var f = new ClientEditForm(found);
            if (f.ShowDialog(this) == DialogResult.OK) { LoadData(); lblStatus.Text = "Клиент обновлён"; }
        }

        private void DeleteSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
            {
                MessageBox.Show("Выберите клиента в таблице", "Подсказка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = (int)grid.CurrentRow.Tag;

            if (MessageBox.Show("Удалить клиента?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            string err = new ClientRepository().Delete(id);
            if (err != null)
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                lblStatus.Text = "Клиент удалён";
                LoadData();
            }
        }
    }
}
