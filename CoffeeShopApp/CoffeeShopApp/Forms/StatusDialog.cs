using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    // Простой диалог смены статуса заказа
    public class StatusDialog : Form
    {
        public string Status;
        private ComboBox cmb;

        public StatusDialog(string current)
        {
            Text = "Изменить статус";
            ClientSize = new Size(360, 150);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Theme.StyleForm(this);

            var lbl = new Label { Text = "Новый статус:", AutoSize = true, Left = 20, Top = 28 };
            cmb = new ComboBox { Left = 130, Top = 24, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmb.Items.AddRange(new object[] { "новый", "выполнен", "отменён" });
            if (cmb.Items.Contains(current)) cmb.SelectedItem = current;

            var btnOk = new Button { Text = "Сохранить", Left = 130, Top = 70, Width = 200, Height = 30 };
            Theme.StyleButton(btnOk);
            btnOk.Click += (s, e) => { Status = (string)cmb.SelectedItem; DialogResult = DialogResult.OK; };

            var btnCancel = new Button { Text = "Отмена", Left = 130, Top = 105, Width = 200, Height = 30 };
            Theme.StyleButton(btnCancel);
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { lbl, cmb, btnOk, btnCancel });
        }
    }
}
