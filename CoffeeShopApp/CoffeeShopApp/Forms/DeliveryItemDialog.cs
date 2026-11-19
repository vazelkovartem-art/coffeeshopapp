using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Models;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    // Диалог добавления позиции в поставку: товар, количество, закупочная цена
    public class DeliveryItemDialog : Form
    {
        public DeliveryItemInfo Result;
        private ComboBox cmbProduct;
        private NumericUpDown numQty, numPrice;
        private Label lblInfo;

        public DeliveryItemDialog(List<Product> products)
        {
            Text = "Позиция поставки";
            ClientSize = new Size(430, 240);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var l1 = new Label { Text = "Товар:", AutoSize = true, Left = 20, Top = 25 };
            cmbProduct = new ComboBox { Left = 110, Top = 21, Width = 290, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var p in products) cmbProduct.Items.Add(p);

            var l2 = new Label { Text = "Количество:", AutoSize = true, Left = 20, Top = 65 };
            numQty = new NumericUpDown { Left = 110, Top = 61, Width = 120, Minimum = 1, Maximum = 9999, Value = 1 };

            var l3 = new Label { Text = "Цена закупки:", AutoSize = true, Left = 20, Top = 105 };
            numPrice = new NumericUpDown { Left = 110, Top = 101, Width = 120, Minimum = 0, Maximum = 100000, DecimalPlaces = 2 };

            lblInfo = new Label { AutoSize = true, Left = 110, Top = 138, ForeColor = Theme.PrimaryDark };

            var btnOk = new Button { Text = "Добавить", Left = 110, Top = 180, Width = 130 };
            Theme.StyleButton(btnOk);
            var btnCancel = new Button { Text = "Отмена", Left = 260, Top = 180, Width = 130 };
            Theme.StyleButton(btnCancel);

            cmbProduct.SelectedIndexChanged += UpdateInfo;
            numQty.ValueChanged += UpdateInfo;
            numPrice.ValueChanged += UpdateInfo;

            btnOk.Click += (s, e) =>
            {
                var p = cmbProduct.SelectedItem as Product;
                if (p == null)
                {
                    MessageBox.Show("Выберите товар", "Проверка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Result = new DeliveryItemInfo
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Quantity = (int)numQty.Value,
                    UnitPrice = numPrice.Value
                };
                DialogResult = DialogResult.OK;
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            if (cmbProduct.Items.Count > 0) cmbProduct.SelectedIndex = 0;
            UpdateInfo(null, null);

            Controls.AddRange(new Control[] { l1, cmbProduct, l2, numQty, l3, numPrice, lblInfo, btnOk, btnCancel });
        }

        private void UpdateInfo(object sender, EventArgs e)
        {
            decimal sum = numPrice.Value * numQty.Value;
            lblInfo.Text = "Сумма: " + sum.ToString("0.##") + " р.";
        }
    }
}
