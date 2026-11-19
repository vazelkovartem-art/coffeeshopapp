using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    public class SupplierEditForm : Form
    {
        private TextBox txtName, txtPhone, txtCity;

        public SupplierEditForm(Supplier supplier)
        {
            Text = supplier == null ? "Новый поставщик" : "Изменение поставщика";
            ClientSize = new Size(420, 260);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var l1 = new Label { Text = "Название:", AutoSize = true, Left = 20, Top = 30 };
            txtName = new TextBox { Left = 130, Top = 26, Width = 260 };
            Theme.StyleTextBox(txtName);

            var l2 = new Label { Text = "Телефон:", AutoSize = true, Left = 20, Top = 70 };
            txtPhone = new TextBox { Left = 130, Top = 66, Width = 260 };
            Theme.StyleTextBox(txtPhone);

            var l3 = new Label { Text = "Город:", AutoSize = true, Left = 20, Top = 110 };
            txtCity = new TextBox { Left = 130, Top = 106, Width = 260 };
            Theme.StyleTextBox(txtCity);

            if (supplier != null)
            {
                txtName.Text = supplier.Name;
                txtPhone.Text = supplier.Phone;
                txtCity.Text = supplier.City;
            }

            var btnSave = new Button { Text = "Сохранить", Left = 130, Top = 160, Width = 130 };
            Theme.StyleButton(btnSave);
            btnSave.Click += Save;

            var btnCancel = new Button { Text = "Отмена", Left = 280, Top = 160, Width = 110 };
            Theme.StyleButton(btnCancel);
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { l1, txtName, l2, txtPhone, l3, txtCity, btnSave, btnCancel });
        }

        private void Save(object sender, EventArgs e)
        {
            if (txtName.Text.Trim().Length < 3)
            {
                MessageBox.Show("Укажите название поставщика", "Проверка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repo = new SupplierRepository();
            var s = new Supplier();
            s.Name = txtName.Text.Trim();
            s.Phone = txtPhone.Text.Trim();
            s.City = txtCity.Text.Trim();
            s.Id = 0;

            string err = repo.Insert(s);
            if (err != null)
            {
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
