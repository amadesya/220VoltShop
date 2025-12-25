-- Optional: set your database name here, e.g. `USE course_store;`
-- USE your_database_here;

-- If you already have a `Products` table and want to replace it, uncomment the next line.
-- DROP TABLE IF EXISTS `Products`;

CREATE TABLE IF NOT EXISTS `Products` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Sku` varchar(64) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Description` text,
  `Price` decimal(10,2) NOT NULL DEFAULT 0.00,
  `Stock` int NOT NULL DEFAULT 0,
  `CategoryId` int DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  INDEX `IX_Products_Sku` (`Sku`)
);

-- Insert sample products (images expected at /images/1.jpeg, /images/2.jpeg, /images/3.jpeg)
INSERT INTO `Products` (`Sku`, `Name`, `Description`, `Price`, `Stock`, `CategoryId`, `CreatedAt`, `UpdatedAt`) VALUES
('1', 'Отвертка аккумуляторная DORKEL DRS-3,6', 'Отвертка аккумуляторная DORKEL DRS-3,6', 549.00, 15, NULL, NOW(), NOW()),
('2', 'Отвертка ARCHIMEDES 90358', 'Отвертка ARCHIMEDES 90358', 19.00, 30, NULL, NOW(), NOW()),
('3', 'Уровень лазерный ADA LaserMarker', 'Уровень лазерный ADA LaserMarker', 890.00, 10, NULL, NOW(), NOW());

-- Additional popular products (4.jpeg,5.jpeg,6.jpeg)
INSERT INTO `Products` (`Sku`, `Name`, `Description`, `Price`, `Stock`, `CategoryId`, `CreatedAt`, `UpdatedAt`) VALUES
('4', 'Краскопульт HAMMER PRZ500B', 'Краскопульт HAMMER PRZ500B', 4690.00, 8, NULL, NOW(), NOW()),
('5', 'Ключ GRIFF 31111', 'Ключ GRIFF 31111', 3.00, 120, NULL, NOW(), NOW()),
('6', 'Лазерный дальномер ADA Cosmo 70', 'Лазерный дальномер ADA Cosmo 70', 590.00, 12, NULL, NOW(), NOW());

-- You can later update CategoryId values to match your Categories table.

-- Optional: if your existing EF-generated table uses a different precision (e.g. decimal(65,30)), run this to change it to two decimals:
-- ALTER TABLE `Products` MODIFY COLUMN `Price` DECIMAL(10,2) NOT NULL DEFAULT 0.00;
