using System.Collections.Generic;
using System.Data.SqlClient;
using CoffeeShopApp.Models;

namespace CoffeeShopApp.Data
{
    public class SupplierRepository
    {
        public List<Supplier> GetAll()
        {
            var list = new List<Supplier>();
            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand("SELECT supplier_id, name, phone, city FROM suppliers ORDER BY name", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Supplier
                        {
                            Id = r.GetInt32(0),
                            Name = r.GetString(1),
                            Phone = r.IsDBNull(2) ? "" : r.GetString(2),
                            City = r.IsDBNull(3) ? "" : r.GetString(3)
                        });
                    }
                }
            }
            return list;
        }

        public string Insert(Supplier s)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO suppliers (name, phone, city) VALUES (@n, @p, @c)", conn))
                {
                    cmd.Parameters.AddWithValue("@n", s.Name);
                    cmd.Parameters.AddWithValue("@p", (object)s.Phone ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@c", (object)s.City ?? System.DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return null;
                }
            }
            catch (SqlException ex) { return Database.HumanMessage(ex); }
        }

        public string Update(Supplier s)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                using (var cmd = new SqlCommand(
                    "UPDATE suppliers SET name = @n, phone = @p, city = @c WHERE supplier_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@n", s.Name);
                    cmd.Parameters.AddWithValue("@p", (object)s.Phone ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@c", (object)s.City ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", s.Id);
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
                using (var cmd = new SqlCommand("DELETE FROM suppliers WHERE supplier_id = @id", conn))
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
