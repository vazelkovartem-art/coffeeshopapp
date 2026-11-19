using System.Drawing;
using System.Windows.Forms;

namespace CoffeeShopApp.Ui
{
    // Общие цвета и оформление для всех окон приложения.
    // Всё в одном месте, чтобы не красить каждый контрол руками в каждой форме.
    public static class Theme
    {
        public static readonly Color Back = Color.FromArgb(240, 245, 250);      // фон окон
        public static readonly Color Panel = Color.White;
        public static readonly Color Primary = Color.FromArgb(41, 98, 158);     // основные кнопки, шапки таблиц
        public static readonly Color PrimaryDark = Color.FromArgb(28, 68, 112); // заголовки
        public static readonly Color PrimaryLight = Color.FromArgb(214, 230, 244);
        public static readonly Color Text = Color.FromArgb(30, 41, 59);
        public static readonly Color Danger = Color.FromArgb(198, 60, 60);      // удаление, выход

        public static readonly Font TitleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font LabelFont = new Font("Segoe UI", 9.75F);
        public static readonly Font BoldFont = new Font("Segoe UI", 9.75F, FontStyle.Bold);

        public static void StyleForm(Form f)
        {
            f.BackColor = Back;
            f.Font = LabelFont;
            f.ForeColor = Text;
        }

        public static void StyleButton(Button b, bool danger = false)
        {
            // Простые прямоугольные кнопки, без закруглений
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.UseVisualStyleBackColor = false;
            b.BackColor = danger ? Danger : Primary;
            b.ForeColor = Color.White;
            b.Font = LabelFont;
            b.Cursor = Cursors.Hand;
            b.Height = 36;
            b.FlatAppearance.MouseOverBackColor = danger ? Color.FromArgb(220, 90, 90) : Color.FromArgb(58, 118, 180);
            b.FlatAppearance.MouseDownBackColor = PrimaryDark;
        }

        public static void StyleTextBox(TextBox t)
        {
            t.BorderStyle = BorderStyle.FixedSingle;
            t.BackColor = Color.White;
        }

        public static void StyleGrid(DataGridView g)
        {
            g.BackgroundColor = Panel;
            g.BorderStyle = BorderStyle.None;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = BoldFont;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Primary;
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 8, 6, 8);
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 40;
            g.RowHeadersVisible = false;
            g.DefaultCellStyle.Font = LabelFont;
            g.DefaultCellStyle.SelectionBackColor = PrimaryLight;
            g.DefaultCellStyle.SelectionForeColor = Text;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 251, 254);
            g.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
            g.RowTemplate.Height = 32;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
        }
    }
}
