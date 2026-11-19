/* ============================================================
   Кофейня «Аромат» — база данных CoffeeDB (SQL Server / SSMS)
   Скрипт ПЕРЕСОЗДАЁТ базу с нуля: старые данные будут заменены
   тестовыми. Для учебной практики это нормально.
   ============================================================ */

IF DB_ID(N'CoffeeDB') IS NOT NULL
BEGIN
    ALTER DATABASE CoffeeDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CoffeeDB;
END
GO

CREATE DATABASE CoffeeDB;
GO
USE CoffeeDB;
GO

CREATE TABLE clients (
    client_id      INT IDENTITY(1,1) PRIMARY KEY,
    full_name      NVARCHAR(100) NOT NULL,
    phone          NVARCHAR(20)  NOT NULL UNIQUE,
    email          NVARCHAR(100) NULL,
    discount       DECIMAL(5,2)  NOT NULL DEFAULT 0 CHECK (discount BETWEEN 0 AND 50),
    registered_on  DATE          NOT NULL DEFAULT CAST(GETDATE() AS DATE)
);

CREATE TABLE employees (
    employee_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name   NVARCHAR(100) NOT NULL,
    [position]  NVARCHAR(30)  NOT NULL CHECK ([position] IN (N'бариста', N'кассир', N'менеджер')),
    hire_date   DATE          NOT NULL,
    salary      DECIMAL(10,2) NOT NULL CHECK (salary > 0)
);

CREATE TABLE categories (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    name        NVARCHAR(50)  NOT NULL UNIQUE
);

CREATE TABLE products (
    product_id   INT IDENTITY(1,1) PRIMARY KEY,
    name         NVARCHAR(100) NOT NULL UNIQUE,
    category_id  INT           NOT NULL REFERENCES categories(category_id),
    price        DECIMAL(10,2) NOT NULL CHECK (price > 0),
    is_available BIT           NOT NULL DEFAULT 1
);

CREATE TABLE suppliers (
    supplier_id INT IDENTITY(1,1) PRIMARY KEY,
    name        NVARCHAR(100) NOT NULL,
    phone       NVARCHAR(20)  NULL,
    city        NVARCHAR(50)  NULL
);

CREATE TABLE orders (
    order_id    INT IDENTITY(1,1) PRIMARY KEY,
    client_id   INT  NOT NULL REFERENCES clients(client_id),
    employee_id INT  NOT NULL REFERENCES employees(employee_id),
    order_date  DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    status      NVARCHAR(20) NOT NULL DEFAULT N'новый'
                CHECK (status IN (N'новый', N'выполнен', N'отменён'))
);

CREATE TABLE order_items (
    order_item_id INT IDENTITY(1,1) PRIMARY KEY,
    order_id      INT NOT NULL REFERENCES orders(order_id),
    product_id    INT NOT NULL REFERENCES products(product_id),
    quantity      INT NOT NULL CHECK (quantity > 0),
    price         DECIMAL(10,2) NOT NULL CHECK (price >= 0),
    UNIQUE (order_id, product_id)
);

CREATE TABLE deliveries (
    delivery_id   INT IDENTITY(1,1) PRIMARY KEY,
    supplier_id   INT  NOT NULL REFERENCES suppliers(supplier_id),
    delivery_date DATE NOT NULL,
    total_amount  DECIMAL(12,2) NULL
);

CREATE TABLE delivery_items (
    delivery_item_id INT IDENTITY(1,1) PRIMARY KEY,
    delivery_id      INT NOT NULL REFERENCES deliveries(delivery_id),
    product_id       INT NOT NULL REFERENCES products(product_id),
    quantity         INT NOT NULL CHECK (quantity > 0),
    unit_price       DECIMAL(10,2) NOT NULL CHECK (unit_price >= 0)
);
GO

-- ============================================================
-- ПРЕДСТАВЛЕНИЯ
-- ============================================================
CREATE VIEW v_orders_full AS
SELECT o.order_id,
       c.full_name AS client,
       e.full_name AS employee,
       o.order_date,
       o.status,
       COALESCE(SUM(oi.quantity * oi.price), 0) AS total
FROM orders o
JOIN clients   c  ON c.client_id = o.client_id
JOIN employees e  ON e.employee_id = o.employee_id
LEFT JOIN order_items oi ON oi.order_id = o.order_id
GROUP BY o.order_id, c.full_name, e.full_name, o.order_date, o.status;
GO

CREATE VIEW v_popular_products AS
SELECT p.name AS product,
       COUNT(oi.order_item_id) AS times_ordered,
       SUM(oi.quantity)        AS total_qty
FROM products p
LEFT JOIN order_items oi ON oi.product_id = p.product_id
GROUP BY p.product_id, p.name;
GO

-- ============================================================
-- ПАРАМЕТРИЗОВАННЫЕ ПРОЦЕДУРЫ (значение через @param, без конкатенации)
-- ============================================================
CREATE PROCEDURE usp_FindProduct
    @q NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM products
    WHERE name LIKE N'%' + @q + N'%';
END;
GO

CREATE PROCEDURE usp_OrdersByClientPeriod
    @client_id INT,
    @from DATE,
    @to DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.order_id, o.order_date, o.status
    FROM orders o
    WHERE o.client_id = @client_id
      AND o.order_date BETWEEN @from AND @to;
END;
GO

-- ============================================================
-- ТЕСТОВЫЕ ДАННЫЕ
-- ============================================================
INSERT INTO clients (full_name, phone, email, discount, registered_on) VALUES
 (N'Иванов Пётр Сергеевич',      N'+7-910-711-22-33', N'ivanov@mail.ru',    5,  '2024-02-10'),
 (N'Смирнова Анна Львовна',      N'+7-910-711-44-55', N'smirnova@mail.ru',  0,  '2024-03-14'),
 (N'Кузнецов Дмитрий Олегович',  N'+7-910-711-66-77', N'kuznetsov@mail.ru', 10, '2024-05-02'),
 (N'Орлова Марина Викторовна',   N'+7-910-711-88-99', N'orlova@mail.ru',    0,  '2024-06-21'),
 (N'Фёдоров Артём Игоревич',     N'+7-910-712-00-11', NULL,                 15, '2024-09-30'),
 (N'Логинова Вера Павловна',     N'+7-910-712-22-33', N'loginova@mail.ru',  5,  '2025-01-18'),
 (N'Титов Павел Романович',      N'+7-910-712-44-55', N'titov@mail.ru',     0,  '2025-04-07'),
 (N'Белова Ольга Дмитриевна',    N'+7-910-712-66-77', N'belova@mail.ru',    10, '2025-08-25');

INSERT INTO employees (full_name, [position], hire_date, salary) VALUES
 (N'Петрова Ольга Ивановна',   N'бариста',  '2023-03-01', 45000),
 (N'Сидоров Никита Андреевич', N'кассир',   '2024-01-15', 38000),
 (N'Гаврилова Елена Юрьевна',  N'менеджер', '2022-09-01', 60000),
 (N'Морозов Илья Павлович',    N'бариста',  '2024-06-10', 45000),
 (N'Лебедева Дарья Сергеевна', N'кассир',   '2025-02-01', 38000),
 (N'Козлов Артём Викторович',  N'кассир',   '2025-05-12', 38000);

INSERT INTO categories (name) VALUES
 (N'Кофе'), (N'Чай'), (N'Десерты'), (N'Выпечка'), (N'Напитки');

INSERT INTO products (name, category_id, price, is_available) VALUES
 (N'Капучино',      1, 180, 1),
 (N'Латте',         1, 200, 1),
 (N'Эспрессо',      1, 120, 1),
 (N'Раф',           1, 260, 1),
 (N'Чёрный чай',    2,  90, 1),
 (N'Зелёный чай',   2, 100, 1),
 (N'Чизкейк',       3, 250, 1),
 (N'Тирамису',      3, 280, 1),
 (N'Круассан',      4, 150, 0),
 (N'Синнабон',      4, 220, 1),
 (N'Американо',     1, 130, 1),
 (N'Какао',         5, 160, 1),
 (N'Штрудель',      4, 190, 1),
 (N'Имбирный чай',  2, 110, 1);

INSERT INTO suppliers (name, phone, city) VALUES
 (N'ООО КофеТрейд',     N'+7-4812-55-01-01', N'Смоленск'),
 (N'ИП Пекарня у Луны', N'+7-4812-55-02-02', N'Смоленск'),
 (N'ООО Чайный дом',    N'+7-499-100-20-30', N'Москва'),
 (N'ТД Аромат кофе',    N'+7-4812-55-03-03', N'Смоленск'),
 (N'ООО ФрешСити',      N'+7-499-200-40-50', N'Москва');

INSERT INTO orders (client_id, employee_id, order_date, status) VALUES
 (1, 1, '2026-11-10', N'выполнен'),
 (2, 2, '2026-11-11', N'выполнен'),
 (3, 1, '2026-11-12', N'новый'),
 (1, 4, '2026-11-12', N'отменён'),
 (5, 2, '2026-11-13', N'новый'),
 (4, 1, '2026-11-13', N'выполнен'),
 (6, 3, '2026-11-14', N'выполнен'),
 (2, 5, '2026-11-15', N'выполнен'),
 (7, 1, '2026-11-15', N'новый'),
 (8, 6, '2026-11-16', N'выполнен'),
 (6, 2, '2026-11-17', N'новый'),
 (3, 1, '2026-11-17', N'выполнен');

INSERT INTO order_items (order_id, product_id, quantity, price) VALUES
 (1, 1, 2, 180), (1, 7, 1, 250),
 (2, 3, 1, 120), (2, 5, 2, 90),
 (3, 2, 1, 200), (3, 7, 2, 250),
 (4, 9, 2, 150),
 (5, 10, 3, 220), (5, 6, 1, 100),
 (6, 1, 4, 180), (6, 8, 1, 280),
 (7, 4, 2, 260), (7, 13, 1, 190),
 (8, 12, 2, 160), (8, 14, 1, 110),
 (9, 2, 3, 200), (9, 8, 2, 280),
 (10, 11, 2, 130), (10, 10, 1, 220),
 (11, 1, 1, 180), (11, 5, 1, 90),
 (12, 4, 1, 260), (12, 12, 1, 160), (12, 7, 1, 250);

INSERT INTO deliveries (supplier_id, delivery_date, total_amount) VALUES
 (1, '2026-11-01', 39000),
 (1, '2026-11-05', 16300),
 (2, '2026-11-07', 17800),
 (2, '2026-11-12', 13000),
 (3, '2026-11-14',  5300),
 (4, '2026-11-16', 12350);

INSERT INTO delivery_items (delivery_id, product_id, quantity, unit_price) VALUES
 (1, 1, 50, 300), (1, 2, 50, 320), (1, 3, 40, 200),
 (2, 11, 40, 220), (2, 12, 30, 250),
 (3, 9, 40, 120), (3, 10, 25, 280), (3, 13, 20, 300),
 (4, 7, 20, 300), (4, 8, 20, 350),
 (5, 5, 30, 90), (5, 6, 20, 130),
 (6, 4, 25, 350), (6, 14, 30, 120);
GO

PRINT 'База CoffeeDB создана и наполнена тестовыми данными';
