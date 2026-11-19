using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    public class ProductEditForm : Form
    {
        private TextBox txtName;
        private ComboBox cmbCategory;
        private NumericUpDown numPrice;
        private CheckBox chkAvailable;
        private Product _product;

        public ProductEditForm(Product product)
        {
            _product = product;
            Text = product == null ? "Новый товар" : "Изменение товара";
            ClientSize = new Size(420, 300);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var l1 = new Label { Text = "Название:", AutoSize = true, Left = 20, Top = 30 };
            txtName = new TextBox { Left = 130, Top = 26, Width = 260 };
            Theme.StyleTextBox(txtName);

            var l2 = new Label { Text = "Категория:", AutoSize = true, Left = 20, Top = 70 };
            cmbCategory = new ComboBox { Left = 130, Top = 66, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in new ProductRepository().GetCategories())
                cmbCategory.Items.Add(c);

            var l3 = new Label { Text = "Цена:", AutoSize = true, Left = 20, Top = 110 };
            numPrice = new NumericUpDown
            {
                Left = 130, Top = 106, Width = 140,
                Minimum = 1, Maximum = 100000, DecimalPlaces = 2, ThousandsSeparator = true
            };

            chkAvailable = new CheckBox { Text = "Товар в наличии", AutoSize = true, Left = 130, Top = 145, Checked = true };

            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;

            if (product != null)
            {
                txtName.Text = product.Name;
                numPrice.Value = product.Price;
                chkAvailable.Checked = product.IsAvailable;
                foreach (Category c in cmbCategory.Items)
                    if (c.Id == product.CategoryId) { cmbCategory.SelectedItem = c; break; }
            }

            var btnSave = new Button { Text = "Сохранить", Left = 130, Top = 195, Width = 130 };
            Theme.StyleButton(btnSave);
            btnSave.Click += Save;

            var btnCancel = new Button { Text = "Отмена", Left = 280, Top = 195, Width = 110 };
            Theme.StyleButton(btnCancel);
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { l1, txtName, l2, cmbCategory, l3, numPrice, chkAvailable, btnSave, btnCancel });
        }

        private void Save(object sender, EventArgs e)
        {
            if (txtName.Text.Trim().Length < 2)
            {
                MessageBox.Show("Укажите название товара", "Проверка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var cat = cmbCategory.SelectedItem as Category;
            if (cat == null)
            {
                MessageBox.Show("Выберите категорию", "Проверка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repo = new ProductRepository();
            var p = _product ?? new Product();
            p.Name = txtName.Text.Trim();
            p.CategoryId = cat.Id;
            p.Price = numPrice.Value;
            p.IsAvailable = chkAvailable.Checked;

            string err = _product == null ? repo.Insert(p) : repo.Update(p);
            if (err != null)
            {
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
