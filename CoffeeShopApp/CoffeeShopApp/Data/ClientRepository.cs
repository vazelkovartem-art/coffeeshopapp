using System.Collections.Generic;
using System.Data.SqlClient;
using CoffeeShopApp.Models;

namespace CoffeeShopApp.Data
{
    public class ClientRepository
    {
        // Белый список сортировок: в ORDER BY подставляем только значения отсюда,
        // пользовательский ввод напрямую в SQL не попадает (п.19 отчёта)
        private static readonly string[] SortMap =
        {
            "full_name ASC",     // 0
            "full_name DESC",    // 1
            "discount DESC",     // 2
            "registered_on ASC"  // 3
        };

        public List<Client> GetAll(string search, bool onlyDiscount, int sortIndex)
        {
            var list = new List<Client>();
            string sort = SortMap[sortIndex];

            string sql = "SELECT client_id, full_name, phone, email, discount FROM clients " +
                         "WHERE (@name = '' OR full_name LIKE '%' + @name + '%') " +
                         "AND (@only = 0 OR discount > 0) " +
                         "ORDER BY " + sort;

            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", search == null ? "" : search.Trim());
                cmd.Parameters.AddWithValue("@only", onlyDiscount ? 1 : 0);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var c = new Client();
                        c.Id = r.GetInt32(0);
                        c.FullName = r.GetString(1);
                        c.Phone = r.GetString(2);
                        c.Email = r.IsDBNull(3) ? "" : r.GetString(3);
                        c.Discount = r.GetDecimal(4);
                        list.Add(c);
                    }
                }
            }
            return list;
        }

        // Возвращает null, если всё ок, иначе текст ошибки для пользователя
        public string Insert(Client c)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO clients (full_name, phone, email, discount) VALUES (@f, @p, @e, @d)", conn))
                {
                    cmd.Parameters.AddWithValue("@f", c.FullName);
                    cmd.Parameters.AddWithValue("@p", c.Phone);
                    cmd.Parameters.AddWithValue("@e", (object)c.Email ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@d", c.Discount);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return null;
                }
            }
            catch (SqlException ex) { return Database.HumanMessage(ex); }
        }

        public string Update(Client c)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                using (var cmd = new SqlCommand(
                    "UPDATE clients SET full_name = @f, phone = @p, email = @e, discount = @d WHERE client_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@f", c.FullName);
                    cmd.Parameters.AddWithValue("@p", c.Phone);
                    cmd.Parameters.AddWithValue("@e", (object)c.Email ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@d", c.Discount);
                    cmd.Parameters.AddWithValue("@id", c.Id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return null;
                }
            }
            catch (SqlException ex) { return Database.HumanMessage(ex); }
        }

        public string Delete(int id)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                using (var cmd = new SqlCommand("DELETE FROM clients WHERE client_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return null;
                }
            }
            catch (SqlException ex) { return Database.HumanMessage(ex); }
        }
    }
}
