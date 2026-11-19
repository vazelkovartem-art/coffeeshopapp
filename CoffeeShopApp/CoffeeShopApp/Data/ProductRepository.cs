using System.Collections.Generic;
using System.Data.SqlClient;
using CoffeeShopApp.Models;

namespace CoffeeShopApp.Data
{
    public class ProductRepository
    {
        public List<Product> GetAll(string search, int categoryId)
        {
            var list = new List<Product>();
            string sql = "SELECT p.product_id, p.name, p.category_id, c.name, p.price, p.is_available " +
                         "FROM products p JOIN categories c ON c.category_id = p.category_id " +
                         "WHERE (@cat = 0 OR p.category_id = @cat) AND (@name = '' OR p.name LIKE '%' + @name + '%') " +
                         "ORDER BY p.name";

            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", search == null ? "" : search.Trim());
                cmd.Parameters.AddWithValue("@cat", categoryId);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var p = new Product();
                        p.Id = r.GetInt32(0);
                        p.Name = r.GetString(1);
                        p.CategoryId = r.GetInt32(2);
                        p.CategoryName = r.GetString(3);
                        p.Price = r.GetDecimal(4);
                        p.IsAvailable = r.GetBoolean(5);
                        list.Add(p);
                    }
                }
            }
            return list;
        }

        public List<Category> GetCategories()
        {
            var list = new List<Category>();
            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand("SELECT category_id, name FROM categories ORDER BY name", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Category { Id = r.GetInt32(0), Name = r.GetString(1) });
                    }
                }
            }
            return list;
        }

        // Переключение "в наличии / нет в наличии" одним запросом
        public string SetAvailable(int id, bool available)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                using (var cmd = new SqlCommand("UPDATE products SET is_available = @a WHERE product_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@a", available ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return null;
                }
            }
            catch (SqlException ex) { return Database.HumanMessage(ex); }
        }

        public string Insert(Product p)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO products (name, category_id, price, is_available) VALUES (@n, @c, @p, @a)", conn))
                {
                    cmd.Parameters.AddWithValue("@n", p.Name);
                    cmd.Parameters.AddWithValue("@c", p.CategoryId);
                    cmd.Parameters.AddWithValue("@p", p.Price);
                    cmd.Parameters.AddWithValue("@a", p.IsAvailable ? 1 : 0);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return null;
                }
            }
            catch (SqlException ex) { return Database.HumanMessage(ex); }
        }

        public string Update(Product p)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                using (var cmd = new SqlCommand(
                    "UPDATE products SET name = @n, category_id = @c, price = @p, is_available = @a WHERE product_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@n", p.Name);
                    cmd.Parameters.AddWithValue("@c", p.CategoryId);
                    cmd.Parameters.AddWithValue("@p", p.Price);
                    cmd.Parameters.AddWithValue("@a", p.IsAvailable ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", p.Id);
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
                using (var cmd = new SqlCommand("DELETE FROM products WHERE product_id = @id", conn))
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
