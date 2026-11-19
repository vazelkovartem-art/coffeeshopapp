using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CoffeeShopApp.Models;

namespace CoffeeShopApp.Data
{
    public class DeliveryRepository
    {
        public List<DeliveryInfo> GetDeliveries()
        {
            var list = new List<DeliveryInfo>();
            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand(
                "SELECT d.delivery_id, s.name, d.delivery_date, d.total_amount " +
                "FROM deliveries d JOIN suppliers s ON s.supplier_id = d.supplier_id " +
                "ORDER BY d.delivery_id DESC", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new DeliveryInfo
                        {
                            DeliveryId = r.GetInt32(0),
                            Supplier = r.GetString(1),
                            DeliveryDate = r.GetDateTime(2),
                            Total = r.IsDBNull(3) ? 0 : r.GetDecimal(3)
                        });
                    }
                }
            }
            return list;
        }

        public List<DeliveryItemInfo> GetDeliveryItems(int deliveryId)
        {
            var list = new List<DeliveryItemInfo>();
            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand(
                "SELECT p.product_id, p.name, di.quantity, di.unit_price " +
                "FROM delivery_items di JOIN products p ON p.product_id = di.product_id " +
                "WHERE di.delivery_id = @id ORDER BY di.delivery_item_id", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = deliveryId;
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new DeliveryItemInfo
                        {
                            ProductId = r.GetInt32(0),
                            ProductName = r.GetString(1),
                            Quantity = r.GetInt32(2),
                            UnitPrice = r.GetDecimal(3)
                        });
                    }
                }
            }
            return list;
        }

        public List<Supplier> GetSuppliersForCombo()
        {
            var list = new List<Supplier>();
            using (var conn = new SqlConnection(Database.ConnectionString))
            using (var cmd = new SqlCommand("SELECT supplier_id, name FROM suppliers ORDER BY name", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(new Supplier { Id = r.GetInt32(0), Name = r.GetString(1) });
                }
            }
            return list;
        }

        public string AddDelivery(int supplierId, DateTime date, List<DeliveryItemInfo> items)
        {
            try
            {
                decimal total = 0;
                foreach (var it in items) total += it.Quantity * it.UnitPrice;

                using (var conn = new SqlConnection(Database.ConnectionString))
                {
                    conn.Open();
                    using (var tr = conn.BeginTransaction())
                    {
                        try
                        {
                            int deliveryId;
                            using (var cmd = new SqlCommand(
                                "INSERT INTO deliveries (supplier_id, delivery_date, total_amount) VALUES (@s, @d, @t); " +
                                "SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tr))
                            {
                                cmd.Parameters.Add("@s", SqlDbType.Int).Value = supplierId;
                                cmd.Parameters.Add("@d", SqlDbType.Date).Value = date.Date;
                                cmd.Parameters.Add("@t", SqlDbType.Decimal).Value = total;
                                deliveryId = (int)cmd.ExecuteScalar();
                            }

                            foreach (var it in items)
                            {
                                using (var cmd = new SqlCommand(
                                    "INSERT INTO delivery_items (delivery_id, product_id, quantity, unit_price) VALUES (@d, @p, @q, @u)", conn, tr))
                                {
                                    cmd.Parameters.Add("@d", SqlDbType.Int).Value = deliveryId;
                                    cmd.Parameters.Add("@p", SqlDbType.Int).Value = it.ProductId;
                                    cmd.Parameters.Add("@q", SqlDbType.Int).Value = it.Quantity;
                                    cmd.Parameters.Add("@u", SqlDbType.Decimal).Value = it.UnitPrice;
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            tr.Commit();
                            return null;
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                            return "Не удалось сохранить поставку: " + ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex) { return "Ошибка базы данных: " + ex.Message; }
        }

        public string DeleteDelivery(int deliveryId)
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
                            using (var cmd = new SqlCommand("DELETE FROM delivery_items WHERE delivery_id = @id", conn, tr))
                            {
                                cmd.Parameters.Add("@id", SqlDbType.Int).Value = deliveryId;
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new SqlCommand("DELETE FROM deliveries WHERE delivery_id = @id", conn, tr))
                            {
                                cmd.Parameters.Add("@id", SqlDbType.Int).Value = deliveryId;
                                cmd.ExecuteNonQuery();
                            }
                            tr.Commit();
                            return null;
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                            return "Не удалось удалить поставку: " + ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex) { return "Ошибка базы данных: " + ex.Message; }
        }
    }
}
