using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    public class DeliveriesForm : Form
    {
        private DataGridView grid;
        private Label lblStatus;

        public DeliveriesForm()
        {
            Text = "Поставки";
            ClientSize = new Size(960, 560);
            StartPosition = FormStartPosition.CenterParent;
            Theme.StyleForm(this);

            var lblTitle = new Label
            {
                Text = "Поставки", Font = Theme.TitleFont,
                ForeColor = Theme.PrimaryDark, AutoSize = true, Left = 20, Top = 15
            };

            var btnNew = new Button { Text = "Новая поставка", Left = 20, Top = 70, Width = 150 };
            Theme.StyleButton(btnNew);
            btnNew.Click += (s, e) =>
            {
                var f = new DeliveryEditForm();
                if (f.ShowDialog(this) == DialogResult.OK) { LoadData(); lblStatus.Text = "Поставка добавлена"; }
            };

            var btnItems = new Button { Text = "Позиции", Left = 180, Top = 70, Width = 110 };
            Theme.StyleButton(btnItems);
            btnItems.Click += (s, e) =>
            {
                if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
                {
                    MessageBox.Show("Выберите поставку в таблице", "Подсказка",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int id = (int)grid.CurrentRow.Tag;
                new DeliveryItemsDialog(id, Convert.ToString(grid.CurrentRow.Cells["supplier"].Value)).ShowDialog(this);
            };

            var btnDelete = new Button { Text = "Удалить", Left = 300, Top = 70, Width = 110 };
            Theme.StyleButton(btnDelete, true);
            btnDelete.Click += (s, e) => DeleteSelected();

            var btnRefresh = new Button { Text = "Обновить", Left = 420, Top = 70, Width = 110 };
            Theme.StyleButton(btnRefresh);
            btnRefresh.Click += (s, e) => LoadData();

            var btnClose = new Button { Text = "Закрыть", Left = 830, Top = 515, Width = 110 };
            Theme.StyleButton(btnClose);
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Click += (s, e) => Close();

            grid = new DataGridView { Left = 20, Top = 115, Width = 920, Height = 385 };
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Theme.StyleGrid(grid);
            grid.Columns.Add("num", "№");
            grid.Columns.Add("supplier", "Поставщик");
            grid.Columns.Add("date", "Дата");
            grid.Columns.Add("total", "Сумма");
            grid.Columns["num"].FillWeight = 10;
            grid.Columns["supplier"].FillWeight = 45;
            grid.Columns["date"].FillWeight = 20;
            grid.Columns["total"].FillWeight = 25;
            grid.Columns["total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            foreach (DataGridViewColumn col in grid.Columns) col.ReadOnly = true;

            lblStatus = new Label { Text = "", AutoSize = true, Left = 20, Top = 510, ForeColor = Theme.Primary };

            Controls.AddRange(new Control[] { lblTitle, btnNew, btnItems, btnDelete, btnRefresh, btnClose, grid, lblStatus });

            LoadData();
        }

        private void LoadData()
        {
            var list = new DeliveryRepository().GetDeliveries();
            grid.Rows.Clear();
            foreach (var d in list)
            {
                int i = grid.Rows.Add(d.DeliveryId, d.Supplier, d.DeliveryDate.ToString("dd.MM.yyyy"), d.Total.ToString("0.##"));
                grid.Rows[i].Tag = d.DeliveryId;
            }
            lblStatus.Text = "Поставок: " + list.Count;
        }

        private void DeleteSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
            {
                MessageBox.Show("Выберите поставку в таблице", "Подсказка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = (int)grid.CurrentRow.Tag;

            if (MessageBox.Show(string.Format("Удалить поставку № {0} вместе с позициями?", id),
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            string err = new DeliveryRepository().DeleteDelivery(id);
            if (err != null)
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                lblStatus.Text = "Поставка удалена";
                LoadData();
            }
        }
    }
}
