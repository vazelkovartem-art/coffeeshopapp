using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeShopApp.Data;
using CoffeeShopApp.Ui;

namespace CoffeeShopApp.Forms
{
    public class OrdersForm : Form
    {
        private DataGridView grid;
        private Label lblStatus;

        public OrdersForm()
        {
            Text = "Заказы";
            ClientSize = new Size(960, 560);
            StartPosition = FormStartPosition.CenterParent;
            Theme.StyleForm(this);

            var lblTitle = new Label
            {
                Text = "Заказы", Font = Theme.TitleFont,
                ForeColor = Theme.PrimaryDark, AutoSize = true, Left = 20, Top = 15
            };

            var btnNew = new Button { Text = "Новый заказ", Left = 20, Top = 95, Width = 130 };
            Theme.StyleButton(btnNew);
            btnNew.Click += (s, e) =>
            {
                var f = new OrderEditForm();
                if (f.ShowDialog(this) == DialogResult.OK) { LoadData(); lblStatus.Text = "Заказ создан"; }
            };

            var btnStatus = new Button { Text = "Изменить статус", Left = 160, Top = 95, Width = 140 };
            Theme.StyleButton(btnStatus);
            btnStatus.Click += (s, e) => ChangeStatus();

            var btnDelete = new Button { Text = "Удалить", Left = 310, Top = 95, Width = 110 };
            Theme.StyleButton(btnDelete, true);
            btnDelete.Click += (s, e) => DeleteSelected();

            var btnRefresh = new Button { Text = "Обновить", Left = 430, Top = 95, Width = 110 };
            Theme.StyleButton(btnRefresh);
            btnRefresh.Click += (s, e) => { LoadData(); lblStatus.Text = "Обновлено"; };

            var btnClose = new Button { Text = "Закрыть", Left = 830, Top = 515, Width = 110 };
            Theme.StyleButton(btnClose);
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Click += (s, e) => Close();

            grid = new DataGridView { Left = 20, Top = 140, Width = 920, Height = 360 };
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Theme.StyleGrid(grid);
            grid.Columns.Add("num", "№");
            grid.Columns.Add("client", "Клиент");
            grid.Columns.Add("employee", "Сотрудник");
            grid.Columns.Add("date", "Дата");
            grid.Columns.Add("status", "Статус");
            grid.Columns.Add("total", "Сумма");
            grid.Columns["num"].FillWeight = 8;
            grid.Columns["client"].FillWeight = 25;
            grid.Columns["employee"].FillWeight = 20;
            grid.Columns["date"].FillWeight = 14;
            grid.Columns["status"].FillWeight = 13;
            grid.Columns["total"].FillWeight = 15;
            foreach (DataGridViewColumn col in grid.Columns) col.ReadOnly = true;

            lblStatus = new Label { Text = "", AutoSize = true, Left = 20, Top = 510, ForeColor = Theme.Primary };

            Controls.AddRange(new Control[] { lblTitle, btnNew, btnStatus, btnDelete, btnRefresh, btnClose, grid, lblStatus });

            LoadData();
        }

        private void LoadData()
        {
            var repo = new OrderRepository();
            var list = repo.GetOrders();
            grid.Rows.Clear();
            foreach (var o in list)
            {
                int i = grid.Rows.Add(o.OrderId, o.Client, o.Employee,
                    o.OrderDate.ToString("dd.MM.yyyy"), o.Status, o.Total.ToString("0.##"));
                grid.Rows[i].Tag = o.OrderId;
            }
            lblStatus.Text = "Заказов: " + list.Count;
        }

        private void ChangeStatus()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
            {
                MessageBox.Show("Выберите заказ в таблице", "Подсказка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = (int)grid.CurrentRow.Tag;
            string current = Convert.ToString(grid.CurrentRow.Cells["status"].Value);

            var dlg = new StatusDialog(current);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            string err = new OrderRepository().ChangeStatus(id, dlg.Status);
            if (err != null)
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                lblStatus.Text = string.Format("Статус заказа № {0}: {1}", id, dlg.Status);
                LoadData();
            }
        }

        private void DeleteSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Tag == null)
            {
                MessageBox.Show("Выберите заказ в таблице", "Подсказка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int id = (int)grid.CurrentRow.Tag;

            if (MessageBox.Show(string.Format("Удалить заказ № {0} вместе с позициями?", id),
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            string err = new OrderRepository().DeleteOrder(id);
            if (err != null)
                MessageBox.Show(err, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                lblStatus.Text = "Заказ удалён";
                LoadData();
            }
        }
    }
}
