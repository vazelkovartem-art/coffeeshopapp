using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    // Окно создания нового заказа: клиент, сотрудник, дата и позиции.
    // В идеале логику позиций можно вынести в общий класс с поставками, но пока так.
    public class OrderEditForm : Form
    {
        private ComboBox cmbClient, cmbEmployee;
        private DateTimePicker dtpDate;
        private ListBox lstItems;
        private Label lblTotal;
        private List<OrderItemInfo> _items = new List<OrderItemInfo>();

        public OrderEditForm()
        {
            Text = "Новый заказ";
            ClientSize = new Size(580, 470);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var rep = new OrderRepository();

            var l1 = new Label { Text = "Клиент:", AutoSize = true, Left = 20, Top = 27 };
            cmbClient = new ComboBox { Left = 120, Top = 23, Width = 430, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in rep.GetClientsForCombo()) cmbClient.Items.Add(c);

            var l2 = new Label { Text = "Сотрудник:", AutoSize = true, Left = 20, Top = 67 };
            cmbEmployee = new ComboBox { Left = 120, Top = 63, Width = 430, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var emp in rep.GetEmployeesForCombo()) cmbEmployee.Items.Add(emp);

            var l3 = new Label { Text = "Дата:", AutoSize = true, Left = 20, Top = 107 };
            dtpDate = new DateTimePicker { Left = 120, Top = 103, Width = 150, Format = DateTimePickerFormat.Short };

            var l4 = new Label { Text = "Позиции заказа:", Font = Theme.BoldFont, AutoSize = true, Left = 20, Top = 142 };

            lstItems = new ListBox
            {
                Left = 20, Top = 168, Width = 400, Height = 210,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            lstItems.Font = Theme.LabelFont;
            lstItems.BorderStyle = BorderStyle.FixedSingle;

            var btnAddItem = new Button { Text = "Добавить позицию", Left = 435, Top = 168, Width = 125, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Theme.StyleButton(btnAddItem);
            var btnRemoveItem = new Button { Text = "Убрать позицию", Left = 435, Top = 213, Width = 125, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Theme.StyleButton(btnRemoveItem);
            var btnSave = new Button { Text = "Сохранить заказ", Left = 435, Top = 258, Width = 125, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Theme.StyleButton(btnSave);
            var btnCancel = new Button { Text = "Отмена", Left = 435, Top = 303, Width = 125, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Theme.StyleButton(btnCancel);

            lblTotal = new Label
            {
                Text = "Итого: 0 р.", Font = Theme.BoldFont, AutoSize = true,
                Left = 20, Top = 390, ForeColor = Theme.PrimaryDark
            };

            btnAddItem.Click += (s, e) =>
            {
                var dlg = new OrderItemDialog(new OrderRepository().GetAvailableProducts());
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

            if (cmbClient.Items.Count > 0) cmbClient.SelectedIndex = 0;
            if (cmbEmployee.Items.Count > 0) cmbEmployee.SelectedIndex = 0;

            Controls.AddRange(new Control[] { l1, cmbClient, l2, cmbEmployee, l3, dtpDate,
                l4, lstItems, btnAddItem, btnRemoveItem, btnSave, btnCancel, lblTotal });
        }

        private void RefreshItems()
        {
            lstItems.Items.Clear();
            decimal total = 0;
            foreach (var it in _items)
            {
                lstItems.Items.Add(string.Format("{0} × {1} — {2} р.",
                    it.Quantity, it.ProductName, (it.Quantity * it.Price).ToString("0.##")));
                total += it.Quantity * it.Price;
            }
            lblTotal.Text = "Итого: " + total.ToString("0.##") + " р.";
        }

        private void Save(object sender, EventArgs e)
        {
            var client = cmbClient.SelectedItem as Client;
            var emp = cmbEmployee.SelectedItem as Employee;
            if (client == null || emp == null)
            {
                MessageBox.Show("Выберите клиента и сотрудника", "Проверка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_items.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну позицию в заказ", "Проверка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string err = new OrderRepository().AddOrder(client.Id, emp.Id, dtpDate.Value.Date, _items);
            if (err != null)
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                DialogResult = DialogResult.OK;
        }
    }
}
