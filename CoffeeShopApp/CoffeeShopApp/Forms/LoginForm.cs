using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLogin;

        public LoginForm()
        {
            Text = "Авторизация — Кофейня «Аромат»";
            ClientSize = new Size(400, 250);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var lblTitle = new Label
            {
                Text = "Кофейня «Аромат»",
                Font = Theme.TitleFont,
                ForeColor = Theme.PrimaryDark,
                AutoSize = true,
                Left = 40, Top = 18
            };
            var lblSub = new Label
            {
                Text = "Вход для сотрудников",
                AutoSize = true,
                Left = 40, Top = 58
            };

            var lblLogin = new Label { Text = "Логин:", AutoSize = true, Left = 40, Top = 97 };
            txtLogin = new TextBox { Left = 120, Top = 93, Width = 240 };
            Theme.StyleTextBox(txtLogin);

            var lblPass = new Label { Text = "Пароль:", AutoSize = true, Left = 40, Top = 137 };
            txtPassword = new TextBox { Left = 120, Top = 133, Width = 240, PasswordChar = '*' };
            Theme.StyleTextBox(txtPassword);

            btnLogin = new Button { Text = "Войти", Left = 120, Top = 180, Width = 240, Height = 38 };
            Theme.StyleButton(btnLogin);
            btnLogin.Click += OnLogin;

            // Enter в окне логина = нажать "Войти"
            AcceptButton = btnLogin;

            Controls.AddRange(new Control[] { lblTitle, lblSub, lblLogin, txtLogin, lblPass, txtPassword, btnLogin });

            // Проверяем подключение к БД сразу, а не когда пользователь нажмёт "Войти"
            this.Load += (s, e) =>
            {
                string err;
                if (!Database.TestConnection(out err))
                {
                    MessageBox.Show(err, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            };
        }

        private void OnLogin(object sender, EventArgs e)
        {
            // TODO: перенести пользователей в БД (п.18 отчёта)
            if (txtLogin.Text == "dev" && txtPassword.Text == "123123")
            {
                Hide();
                var main = new MainForm();
                main.ShowDialog();
                Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}
