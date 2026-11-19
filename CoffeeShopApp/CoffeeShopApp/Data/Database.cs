using System;
using System.Configuration;
using System.Data.SqlClient;

namespace CoffeeShopApp.Data
{
    public static class Database
    {
        // Строка подключения лежит в App.config (п.27 отчёта), в коде ничего не хардкодим
        public static string ConnectionString
        {
            get
            {
                var s = ConfigurationManager.ConnectionStrings["CoffeeDb"];
                if (s == null)
                    throw new InvalidOperationException("В App.config нет строки подключения CoffeeDb");
                return s.ConnectionString;
            }
        }

        public static bool TestConnection(out string error)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                }
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = "Нет подключения к базе данных. Проверьте App.config и запущен ли SQL Server.\n\n" + ex.Message;
                return false;
            }
        }

        // Перевод ошибок SQL Server на человеческий язык (для пункта про ссылочную целостность)
        public static string HumanMessage(SqlException ex)
        {
            // 547 - нарушение внешнего ключа, 2601/2627 - нарушение уникальности
            if (ex.Number == 547)
                return "Нельзя удалить запись: на неё ссылаются другие данные (например, у клиента есть заказы).";
            if (ex.Number == 2601 || ex.Number == 2627)
                return "Такая запись уже существует (нарушено условие уникальности).";
            return "Ошибка базы данных: " + ex.Message;
        }
    }
}
