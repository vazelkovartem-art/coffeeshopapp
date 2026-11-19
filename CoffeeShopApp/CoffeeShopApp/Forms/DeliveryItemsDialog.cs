using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    // Просмотр позиций конкретной поставки (только чтение)
    public class DeliveryItemsDialog : Form
    {
        public DeliveryItemsDialog(int deliveryId, string supplier)
        {
            Text = string.Format("Поставка № {0} — {1}", deliveryId, supplier);
            ClientSize = new Size(560, 380);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var grid = new DataGridView { Left = 20, Top = 20, Width = 520, Height = 290 };
            Theme.StyleGrid(grid);
            grid.Columns.Add("name", "Товар");
            grid.Columns.Add("qty", "Кол-во");
            grid.Columns.Add("price", "Цена закупки");
            grid.Columns.Add("sum", "Сумма");
            grid.Columns["name"].FillWeight = 40;
            grid.Columns["qty"].FillWeight = 15;
            grid.Columns["price"].FillWeight = 20;
            grid.Columns["sum"].FillWeight = 20;
            foreach (DataGridViewColumn col in grid.Columns) col.ReadOnly = true;

            var btnClose = new Button { Text = "Закрыть", Left = 430, Top = 320, Width = 110 };
            Theme.StyleButton(btnClose);
            btnClose.Click += (s, e) => Close();

            decimal total = 0;
            foreach (var it in new DeliveryRepository().GetDeliveryItems(deliveryId))
            {
                decimal sum = it.Quantity * it.UnitPrice;
                grid.Rows.Add(it.ProductName, it.Quantity, it.UnitPrice.ToString("0.##"), sum.ToString("0.##"));
                total += sum;
            }

            var lblTotal = new Label
            {
                Text = "Итого: " + total.ToString("0.##") + " р.",
                Font = Theme.BoldFont, AutoSize = true, Left = 20, Top = 322
            };

            Controls.AddRange(new Control[] { grid, btnClose, lblTotal });
        }
    }
}
