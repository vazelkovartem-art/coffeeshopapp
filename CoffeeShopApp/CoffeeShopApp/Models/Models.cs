using System;

namespace CoffeeShopApp.Models
{
    // Внимание: здесь публичные поля вместо свойств. Знаю, что «не по канону»,
    // но для учебного проекта так короче и понятнее.
    public class Client
    {
        public int Id;
        public string FullName;
        public string Phone;
        public string Email;
        public decimal Discount;

        public override string ToString() { return FullName; }
    }

    public class Employee
    {
        public int Id;
        public string FullName;
        public override string ToString() { return FullName; }
    }

    public class Category
    {
        public int Id;
        public string Name;
        public override string ToString() { return Name; }
    }

    public class Product
    {
        public int Id;
        public string Name;
        public int CategoryId;
        public string CategoryName;
        public decimal Price;
        public bool IsAvailable;
        public override string ToString() { return Name; }
    }

    public class Supplier
    {
        public int Id;
        public string Name;
        public string Phone;
        public string City;
        public override string ToString() { return Name; }
    }

    public class OrderInfo
    {
        public int OrderId;
        public string Client;
        public string Employee;
        public DateTime OrderDate;
        public string Status;
        public decimal Total;
    }

    public class OrderItemInfo
    {
        public int ProductId;
        public string ProductName;
        public int Quantity;
        public decimal Price;
    }

    public class DeliveryInfo
    {
        public int DeliveryId;
        public string Supplier;
        public DateTime DeliveryDate;
        public decimal Total;
    }

    public class DeliveryItemInfo
    {
        public int ProductId;
        public string ProductName;
        public int Quantity;
        public decimal UnitPrice;
    }
}
