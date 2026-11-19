using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    // Диалог добавления одной позиции в заказ
    public class OrderItemDialog : Form
    {
        public OrderItemInfo Result;
        private ComboBox cmbProduct;
        private NumericUpDown numQty;
        private Label lblInfo;

        public OrderItemDialog(List<Product> products)
        {
            Text = "Позиция заказа";
            ClientSize = new Size(430, 200);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var l1 = new Label { Text = "Товар:", AutoSize = true, Left = 20, Top = 25 };
            cmbProduct = new ComboBox { Left = 110, Top = 21, Width = 290, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var p in products) cmbProduct.Items.Add(p);

            var l2 = new Label { Text = "Количество:", AutoSize = true, Left = 20, Top = 65 };
            numQty = new NumericUpDown { Left = 110, Top = 61, Width = 120, Minimum = 1, Maximum = 99, Value = 1 };

            lblInfo = new Label { AutoSize = true, Left = 110, Top = 98, ForeColor = Theme.PrimaryDark };

            var btnOk = new Button { Text = "Добавить", Left = 110, Top = 140, Width = 130 };
            Theme.StyleButton(btnOk);
            var btnCancel = new Button { Text = "Отмена", Left = 260, Top = 140, Width = 130 };
            Theme.StyleButton(btnCancel);

            // Подписываюсь на события ПОСЛЕ того, как всё создано, иначе будет null
            cmbProduct.SelectedIndexChanged += UpdateInfo;
            numQty.ValueChanged += UpdateInfo;

            btnOk.Click += (s, e) =>
            {
                var p = cmbProduct.SelectedItem as Product;
                if (p == null)
                {
                    MessageBox.Show("Выберите товар", "Проверка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Result = new OrderItemInfo
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Quantity = (int)numQty.Value,
                    Price = p.Price
                };
                DialogResult = DialogResult.OK;
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            if (cmbProduct.Items.Count > 0) cmbProduct.SelectedIndex = 0;
            UpdateInfo(null, null);

            Controls.AddRange(new Control[] { l1, cmbProduct, l2, numQty, lblInfo, btnOk, btnCancel });
        }

        private void UpdateInfo(object sender, EventArgs e)
        {
            var p = cmbProduct.SelectedItem as Product;
            if (p == null) { lblInfo.Text = ""; return; }
            decimal sum = p.Price * numQty.Value;
            lblInfo.Text = string.Format("Цена: {0} р.   Сумма: {1} р.", p.Price.ToString("0.##"), sum.ToString("0.##"));
        }
    }
}
