using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    // Одно окно и для добавления, и для редактирования:
    // если client == null - это новый клиент
    public class ClientEditForm : Form
    {
        private TextBox txtName, txtPhone, txtEmail;
        private NumericUpDown numDiscount;

        public ClientEditForm(Client client)
        {
            Text = client == null ? "Новый клиент" : "Изменение клиента";
            ClientSize = new Size(420, 300);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var l1 = new Label { Text = "ФИО:", AutoSize = true, Left = 20, Top = 30 };
            txtName = new TextBox { Left = 110, Top = 26, Width = 280 };
            Theme.StyleTextBox(txtName);

            var l2 = new Label { Text = "Телефон:", AutoSize = true, Left = 20, Top = 70 };
            txtPhone = new TextBox { Left = 110, Top = 66, Width = 280 };
            Theme.StyleTextBox(txtPhone);

            var l3 = new Label { Text = "E-mail:", AutoSize = true, Left = 20, Top = 110 };
            txtEmail = new TextBox { Left = 110, Top = 106, Width = 280 };
            Theme.StyleTextBox(txtEmail);

            var l4 = new Label { Text = "Скидка, %:", AutoSize = true, Left = 20, Top = 150 };
            numDiscount = new NumericUpDown { Left = 110, Top = 146, Width = 120, Minimum = 0, Maximum = 50 };
            if (client != null)
            {
                txtName.Text = client.FullName;
                txtPhone.Text = client.Phone;
                txtEmail.Text = client.Email;
                numDiscount.Value = client.Discount;
            }

            var btnSave = new Button { Text = "Сохранить", Left = 110, Top = 200, Width = 130 };
            Theme.StyleButton(btnSave);
            btnSave.Click += Save;

            var btnCancel = new Button { Text = "Отмена", Left = 260, Top = 200, Width = 130 };
            Theme.StyleButton(btnCancel);
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { l1, txtName, l2, txtPhone, l3, txtEmail, l4, numDiscount, btnSave, btnCancel });
        }

        private void Save(object sender, EventArgs e)
        {
            // Простая валидация перед сохранением
            if (txtName.Text.Trim().Length < 3)
            {
                MessageBox.Show("Укажите ФИО (минимум 3 символа)", "Проверка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtPhone.Text.Trim().Length < 6)
            {
                MessageBox.Show("Укажите телефон", "Проверка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repo = new ClientRepository();
            // NB: у модели публичное поле Id используется как флаг "новый/редактируем"
            var c = new Client();
            c.FullName = txtName.Text.Trim();
            c.Phone = txtPhone.Text.Trim();
            c.Email = txtEmail.Text.Trim();
            c.Discount = numDiscount.Value;

            // Передаю через public поле, чтобы не плодить конструкторы
            c.Id = 0;
            string err = repo.Insert(c);
            if (err != null)
            {
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
