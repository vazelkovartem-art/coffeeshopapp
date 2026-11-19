using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CoffeeShopApp.Models;

namespace CoffeeShopApp.Data
{
    public class OrderRepository
    {
        // Список заказов из представления v_orders_full
        public List<OrderInfo> GetOrders()
        {
            var list = new List<OrderInfo>();
            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand(
                "SELECT order_id, client, employee, order_date, status, total FROM v_orders_full ORDER BY order_id DESC", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var o = new OrderInfo();
                        o.OrderId = r.GetInt32(0);
                        o.Client = r.GetString(1);
                        o.Employee = r.GetString(2);
                        o.OrderDate = r.GetDateTime(3);
                        o.Status = r.GetString(4);
                        o.Total = r.GetDecimal(5);
                        list.Add(o);
                    }
                }
            }
            return list;
        }

        public List<Client> GetClientsForCombo()
        {
            var list = new List<Client>();
            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand("SELECT client_id, full_name FROM clients ORDER BY full_name", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(new Client { Id = r.GetInt32(0), FullName = r.GetString(1) });
                }
            }
            return list;
        }

        public List<Employee> GetEmployeesForCombo()
        {
            var list = new List<Employee>();
            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand("SELECT employee_id, full_name FROM employees ORDER BY full_name", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(new Employee { Id = r.GetInt32(0), FullName = r.GetString(1) });
                }
            }
            return list;
        }

        // В выпадающий список нового заказа попадают только товары в наличии
        public List<Product> GetAvailableProducts()
        {
            var list = new List<Product>();
            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand(
                "SELECT product_id, name, price FROM products WHERE is_available = 1 ORDER BY name", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(new Product { Id = r.GetInt32(0), Name = r.GetString(1), Price = r.GetDecimal(2) });
                }
            }
            return list;
        }

        // Новый заказ + позиции одним блоком: либо всё сохранится, либо ничего (транзакция)
        public string AddOrder(int clientId, int employeeId, DateTime date, List<OrderItemInfo> items)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                {
                    conn.Open();
                    using (var tr = conn.BeginTransaction())
                    {
                        try
                        {
                            int orderId;
                            using (var cmd = new SqlCommand(
                                "INSERT INTO orders (client_id, employee_id, order_date, status) " +
                                "VALUES (@c, @e, @d, N'новый'); SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tr))
                            {
                                cmd.Parameters.Add("@c", SqlDbType.Int).Value = clientId;
                                cmd.Parameters.Add("@e", SqlDbType.Int).Value = employeeId;
                                cmd.Parameters.Add("@d", SqlDbType.Date).Value = date.Date;
                                orderId = (int)cmd.ExecuteScalar();
                            }

                            foreach (var it in items)
                            {
                                using (var cmd = new SqlCommand(
                                    "INSERT INTO order_items (order_id, product_id, quantity, price) VALUES (@o, @p, @q, @pr)", conn, tr))
                                {
                                    cmd.Parameters.Add("@o", SqlDbType.Int).Value = orderId;
                                    cmd.Parameters.Add("@p", SqlDbType.Int).Value = it.ProductId;
                                    cmd.Parameters.Add("@q", SqlDbType.Int).Value = it.Quantity;
                                    cmd.Parameters.Add("@pr", SqlDbType.Decimal).Value = it.Price;
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            tr.Commit();
                            return null;
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                            return "Не удалось сохранить заказ: " + ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex) { return "Ошибка базы данных: " + ex.Message; }
        }

        public string ChangeStatus(int orderId, string status)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                using (var cmd = new SqlCommand("UPDATE orders SET status = @s WHERE order_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@s", status);
                    cmd.Parameters.AddWithValue("@id", orderId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return null;
                }
            }
            catch (SqlException ex) { return Database.HumanMessage(ex); }
        }

        public string DeleteOrder(int orderId)
        {
            try
            {
                using (var conn = new SqlConnection(Database.ConnectionString))
                {
                    conn.Open();
                    using (var tr = conn.BeginTransaction())
                    {
                        try
                        {
                            // Сначала позиции, потом сам заказ - иначе внешнему ключу не понравится
                            using (var cmd = new SqlCommand("DELETE FROM order_items WHERE order_id = @id", conn, tr))
                            {
                                cmd.Parameters.Add("@id", SqlDbType.Int).Value = orderId;
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new SqlCommand("DELETE FROM orders WHERE order_id = @id", conn, tr))
                            {
                                cmd.Parameters.Add("@id", SqlDbType.Int).Value = orderId;
                                cmd.ExecuteNonQuery();
                            }
                            tr.Commit();
                            return null;
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                            return "Не удалось удалить заказ: " + ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex) { return "Ошибка базы данных: " + ex.Message; }
        }
    }
}
