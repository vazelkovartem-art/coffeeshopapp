using System.Windows.Forms;

namespace CoffeeShopApp
{
    static class Program
    {
        [System.STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CoffeeShopApp.Forms.LoginForm());
        }
    }
}
