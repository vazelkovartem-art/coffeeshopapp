using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    // Новая поставка: поставщик, дата, позиции с закупочной ценой.
    // Код почти повторяет OrderEditForm - в идеале вынести в общий класс, но не успел.
    public class DeliveryEditForm : Form
    {
        private ComboBox cmbSupplier;
        private DateTimePicker dtpDate;
        private ListBox lstItems;
        private Label lblTotal;
        private List<DeliveryItemInfo> _items = new List<DeliveryItemInfo>();

        public DeliveryEditForm()
        {
            Text = "Новая поставка";
            ClientSize = new Size(580, 470);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var rep = new DeliveryRepository();

            var l1 = new Label { Text = "Поставщик:", AutoSize = true, Left = 20, Top = 27 };
            cmbSupplier = new ComboBox { Left = 120, Top = 23, Width = 430, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var s in rep.GetSuppliersForCombo()) cmbSupplier.Items.Add(s);

            var l2 = new Label { Text = "Дата:", AutoSize = true, Left = 20, Top = 67 };
            dtpDate = new DateTimePicker { Left = 120, Top = 63, Width = 150, Format = DateTimePickerFormat.Short };

            var l3 = new Label { Text = "Позиции поставки:", Font = Theme.BoldFont, AutoSize = true, Left = 20, Top = 102 };

            lstItems = new ListBox
            {
                Left = 20, Top = 128, Width = 400, Height = 250,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            lstItems.Font = Theme.LabelFont;
            lstItems.BorderStyle = BorderStyle.FixedSingle;

            var btnAddItem = new Button { Text = "Добавить позицию", Left = 435, Top = 128, Width = 125, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Theme.StyleButton(btnAddItem);
            var btnRemoveItem = new Button { Text = "Убрать позицию", Left = 435, Top = 173, Width = 125, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Theme.StyleButton(btnRemoveItem);
            var btnSave = new Button { Text = "Сохранить", Left = 435, Top = 218, Width = 125, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Theme.StyleButton(btnSave);
            var btnCancel = new Button { Text = "Отмена", Left = 435, Top = 263, Width = 125, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Theme.StyleButton(btnCancel);

            lblTotal = new Label
            {
                Text = "Итого: 0 р.", Font = Theme.BoldFont, AutoSize = true,
                Left = 20, Top = 390, ForeColor = Theme.PrimaryDark
            };

            btnAddItem.Click += (s, e) =>
            {
                var dlg = new DeliveryItemDialog(new ProductRepository().GetAll("", 0));
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _items.Add(dlg.Result);
                    RefreshItems();
                }
            };
            btnRemoveItem.Click += (s, e) =>
            {
                if (lstItems.SelectedIndex < 0) return;
                _items.RemoveAt(lstItems.SelectedIndex);
                RefreshItems();
            };
            btnSave.Click += Save;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            if (cmbSupplier.Items.Count > 0) cmbSupplier.SelectedIndex = 0;

            Controls.AddRange(new Control[] { l1, cmbSupplier, l2, dtpDate, l3, lstItems,
                btnAddItem, btnRemoveItem, btnSave, btnCancel, lblTotal });
        }

        private void RefreshItems()
        {
            lstItems.Items.Clear();
            decimal total = 0;
            foreach (var it in _items)
            {
                lstItems.Items.Add(string.Format("{0} × {1} по {2} р. — {3} р.",
                    it.Quantity, it.ProductName, it.UnitPrice.ToString("0.##"),
                    (it.Quantity * it.UnitPrice).ToString("0.##")));
                total += it.Quantity * it.UnitPrice;
            }
            lblTotal.Text = "Итого: " + total.ToString("0.##") + " р.";
        }

        private void Save(object sender, EventArgs e)
        {
            var sup = cmbSupplier.SelectedItem as Supplier;
            if (sup == null)
            {
                MessageBox.Show("Выберите поставщика", "Проверка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_items.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну позицию в поставку", "Проверка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string err = new DeliveryRepository().AddDelivery(sup.Id, dtpDate.Value.Date, _items);
            if (err != null)
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                DialogResult = DialogResult.OK;
        }
    }
}
