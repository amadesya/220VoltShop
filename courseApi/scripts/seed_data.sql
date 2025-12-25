-- SQL seed script for courseApi database (MySQL / MySQL Workbench friendly)
-- Usage:
-- 1) Run on the target database (after migrations applied).
-- 2) Replace the PASSWORD_HASH_* placeholders with real bcrypt hashes
--    (you can get them by calling the app register endpoint or generating via BCrypt).
-- The script is idempotent: it checks for existing records and will not insert duplicates.

SET FOREIGN_KEY_CHECKS=0;

-- Roles (insert only if not exists)
INSERT INTO Roles (Name, Description)
SELECT 'Пользователь', 'Обычный пользователь' FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Пользователь');

INSERT INTO Roles (Name, Description)
SELECT 'Администратор', 'Админ системы' FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Администратор');

-- Categories
INSERT INTO Categories (Name, ParentId)
SELECT 'Электроинструмент', NULL FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Электроинструмент');

INSERT INTO Categories (Name, ParentId)
SELECT 'Ручной инструмент', NULL FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Ручной инструмент');

INSERT INTO Categories (Name, ParentId)
SELECT 'Измерительный инструмент', NULL FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Измерительный инструмент');

INSERT INTO Categories (Name, ParentId)
SELECT 'Садовая техника', NULL FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Садовая техника');

-- Insert core product templates (only if SKU missing)
INSERT INTO Products (Sku, Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt)
SELECT 'DW2000', 'Дрель ударная DW-2000', 'Мощная ударная дрель 850 Вт', 8990.00, 12, c.Id, NOW(), NOW()
FROM Categories c
WHERE c.Name = 'Электроинструмент' AND NOT EXISTS (SELECT 1 FROM Products p WHERE p.Sku = 'DW2000');

INSERT INTO Products (Sku, Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt)
SELECT 'UG125', 'Болгарка угловая 125мм', 'УШМ с защитой от перегрузки', 4667.00, 20, c.Id, NOW(), NOW()
FROM Categories c
WHERE c.Name = 'Электроинструмент' AND NOT EXISTS (SELECT 1 FROM Products p WHERE p.Sku = 'UG125');

INSERT INTO Products (Sku, Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt)
SELECT 'KT120', 'Набор инструментов универсальный', 'Профессиональный набор из 120 предметов', 12792.00, 5, c.Id, NOW(), NOW()
FROM Categories c
WHERE c.Name = 'Ручной инструмент' AND NOT EXISTS (SELECT 1 FROM Products p WHERE p.Sku = 'KT120');

INSERT INTO Products (Sku, Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt)
SELECT 'IW18', 'Гайковёрт ударный 18В', 'Аккумуляторный гайковёрт', 8991.00, 8, c.Id, NOW(), NOW()
FROM Categories c
WHERE c.Name = 'Электроинструмент' AND NOT EXISTS (SELECT 1 FROM Products p WHERE p.Sku = 'IW18');

INSERT INTO Products (Sku, Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt)
SELECT 'MH500', 'Молоток строительный 500г', 'Профессиональный молоток', 1290.00, 30, c.Id, NOW(), NOW()
FROM Categories c
WHERE c.Name = 'Ручной инструмент' AND NOT EXISTS (SELECT 1 FROM Products p WHERE p.Sku = 'MH500');

INSERT INTO Products (Sku, Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt)
SELECT 'CS1800', 'Пила циркулярная 1800 Вт', 'Мощная дисковая пила', 12990.00, 6, c.Id, NOW(), NOW()
FROM Categories c
WHERE c.Name = 'Электроинструмент' AND NOT EXISTS (SELECT 1 FROM Products p WHERE p.Sku = 'CS1800');

-- Generate additional demo products (40) based on templates; only insert SKUs that don't exist yet
INSERT INTO Products (Sku, Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt)
SELECT CONCAT(t.SkuBase, s.seq) AS Sku,
       CONCAT(t.NameBase, ' ', s.seq) AS Name,
       t.DescriptionBase AS Description,
       t.PriceBase + (s.seq % 5) * 100 AS Price,
       FLOOR(RAND() * 50) AS Stock,
       c.Id AS CategoryId,
       DATE_SUB(NOW(), INTERVAL s.seq DAY) AS CreatedAt,
       NOW() AS UpdatedAt
FROM (
  SELECT 1 AS seq UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5
  UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9 UNION ALL SELECT 10
  UNION ALL SELECT 11 UNION ALL SELECT 12 UNION ALL SELECT 13 UNION ALL SELECT 14 UNION ALL SELECT 15
  UNION ALL SELECT 16 UNION ALL SELECT 17 UNION ALL SELECT 18 UNION ALL SELECT 19 UNION ALL SELECT 20
  UNION ALL SELECT 21 UNION ALL SELECT 22 UNION ALL SELECT 23 UNION ALL SELECT 24 UNION ALL SELECT 25
  UNION ALL SELECT 26 UNION ALL SELECT 27 UNION ALL SELECT 28 UNION ALL SELECT 29 UNION ALL SELECT 30
  UNION ALL SELECT 31 UNION ALL SELECT 32 UNION ALL SELECT 33 UNION ALL SELECT 34 UNION ALL SELECT 35
  UNION ALL SELECT 36 UNION ALL SELECT 37 UNION ALL SELECT 38 UNION ALL SELECT 39 UNION ALL SELECT 40
) AS s
CROSS JOIN (
  SELECT 'DW2000' AS SkuBase, 'Дрель ударная DW-2000' AS NameBase, 'Мощная ударная дрель 850 Вт' AS DescriptionBase, 8990.00 AS PriceBase, 'Электроинструмент' AS CategoryName
  UNION ALL SELECT 'UG125','Болгарка угловая 125мм','УШМ с защитой от перегрузки',4667.00,'Электроинструмент'
  UNION ALL SELECT 'KT120','Набор инструментов универсальный','Профессиональный набор из 120 предметов',12792.00,'Ручной инструмент'
  UNION ALL SELECT 'IW18','Гайковёрт ударный 18В','Аккумуляторный гайковёрт',8991.00,'Электроинструмент'
  UNION ALL SELECT 'MH500','Молоток строительный 500г','Профессиональный молоток',1290.00,'Ручной инструмент'
  UNION ALL SELECT 'CS1800','Пила циркулярная 1800 Вт','Мощная дисковая пила',12990.00,'Электроинструмент'
) AS t
JOIN Categories c ON c.Name = t.CategoryName
LEFT JOIN Products p ON p.Sku = CONCAT(t.SkuBase, s.seq)
WHERE p.Id IS NULL;

-- Users (insert only if email missing). Replace password placeholders with real bcrypt hashes.
INSERT INTO Users (Email, PasswordHash, Name, RoleId, CreatedAt)
SELECT 'admin@local', 'REPLACE_WITH_BCRYPT_HASH_FOR_admin123', 'Admin', r.Id, NOW()
FROM Roles r
WHERE r.Name = 'Администратор' AND NOT EXISTS (SELECT 1 FROM Users u WHERE u.Email = 'admin@local');

INSERT INTO Users (Email, PasswordHash, Name, RoleId, CreatedAt)
SELECT 'user@local', 'REPLACE_WITH_BCRYPT_HASH_FOR_user123', 'Demo User', r.Id, NOW()
FROM Roles r
WHERE r.Name = 'Пользователь' AND NOT EXISTS (SELECT 1 FROM Users u WHERE u.Email = 'user@local');

SET FOREIGN_KEY_CHECKS=1;

-- Notes:
-- - This version is safe to run multiple times: it checks for existing records and will not insert duplicates.
-- - Replace the password placeholders with bcrypt hashes if you want seeded users to be able to login.
--   You can generate hashes via the API register endpoint or by a small helper using BCrypt.Net.
-- - If you need to re-seed from scratch in development, truncate the tables first (be careful in non-dev environments).
