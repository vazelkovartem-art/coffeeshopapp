using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Кофейня «Аромат» — главное окно";
            ClientSize = new Size(420, 420);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            Theme.StyleForm(this);

            var lblTitle = new Label
            {
                Text = "Кофейня «Аромат»",
                Font = Theme.TitleFont,
                ForeColor = Theme.PrimaryDark,
                AutoSize = true,
                Left = 60, Top = 20
            };
            var lblSub = new Label { Text = "Главное окно — выберите раздел", AutoSize = true, Left = 60, Top = 60 };

            var btnClients = new Button { Text = "Клиенты", Left = 60, Top = 100, Width = 300, Height = 38 };
            var btnOrders = new Button { Text = "Заказы", Left = 60, Top = 150, Width = 300, Height = 38 };
            var btnProducts = new Button { Text = "Товары", Left = 60, Top = 200, Width = 300, Height = 38 };
            var btnSuppliers = new Button { Text = "Поставщики", Left = 60, Top = 250, Width = 300, Height = 38 };
            var btnDeliveries = new Button { Text = "Поставки", Left = 60, Top = 300, Width = 300, Height = 38 };
            var btnExit = new Button { Text = "Выход", Left = 60, Top = 355, Width = 300, Height = 38 };

            Theme.StyleButton(btnClients);
            Theme.StyleButton(btnOrders);
            Theme.StyleButton(btnProducts);
            Theme.StyleButton(btnSuppliers);
            Theme.StyleButton(btnDeliveries);
            Theme.StyleButton(btnExit, true);

            btnClients.Click += (s, e) => { new ClientsForm().ShowDialog(this); };
            btnOrders.Click += (s, e) => { new OrdersForm().ShowDialog(this); };
            btnProducts.Click += (s, e) => { new ProductsForm().ShowDialog(this); };
            btnSuppliers.Click += (s, e) => { new SuppliersForm().ShowDialog(this); };
            btnDeliveries.Click += (s, e) => { new DeliveriesForm().ShowDialog(this); };
            btnExit.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { lblTitle, lblSub, btnClients, btnOrders, btnProducts,
                btnSuppliers, btnDeliveries, btnExit });
        }
    }
}
