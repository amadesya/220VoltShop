CREATE DATABASE  IF NOT EXISTS `220volt` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `220volt`;
-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: localhost    Database: 220volt
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20251213094229_Initial','8.0.0');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categories`
--

DROP TABLE IF EXISTS `categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ParentId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Categories_ParentId` (`ParentId`),
  CONSTRAINT `FK_Categories_Categories_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `categories` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categories`
--

LOCK TABLES `categories` WRITE;
/*!40000 ALTER TABLE `categories` DISABLE KEYS */;
INSERT INTO `categories` VALUES (1,'Электроинструмент',NULL),(2,'Ручной инструмент',NULL),(3,'Измерительный инструмент',NULL),(4,'Садовая техника',NULL);
/*!40000 ALTER TABLE `categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `importexports`
--

DROP TABLE IF EXISTS `importexports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `importexports` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Filename` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Action` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PerformedBy` int DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Details` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `importexports`
--

LOCK TABLES `importexports` WRITE;
/*!40000 ALTER TABLE `importexports` DISABLE KEYS */;
/*!40000 ALTER TABLE `importexports` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orderitems`
--

DROP TABLE IF EXISTS `orderitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orderitems` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `OrderId` int NOT NULL,
  `ProductId` int NOT NULL,
  `Quantity` int NOT NULL,
  `UnitPrice` decimal(65,30) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OrderItems_OrderId` (`OrderId`),
  CONSTRAINT `FK_OrderItems_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orderitems`
--

LOCK TABLES `orderitems` WRITE;
/*!40000 ALTER TABLE `orderitems` DISABLE KEYS */;
INSERT INTO `orderitems` VALUES (1,1,1,2,649.000000000000000000000000000000),(2,2,7,1,749.000000000000000000000000000000),(3,2,23,2,303.000000000000000000000000000000),(4,3,27,1,1090.000000000000000000000000000000),(5,3,39,2,1290.000000000000000000000000000000),(6,4,31,1,649.000000000000000000000000000000),(7,4,37,2,749.000000000000000000000000000000),(8,4,25,3,549.000000000000000000000000000000),(9,5,37,1,749.000000000000000000000000000000),(10,6,21,1,990.000000000000000000000000000000),(11,6,19,2,949.000000000000000000000000000000),(12,7,15,1,890.000000000000000000000000000000),(13,8,10,1,4690.000000000000000000000000000000),(14,9,22,1,4890.000000000000000000000000000000),(15,10,14,1,419.000000000000000000000000000000),(16,10,28,2,4990.000000000000000000000000000000),(17,10,9,3,1290.000000000000000000000000000000),(18,11,18,1,890.000000000000000000000000000000),(19,11,34,2,5090.000000000000000000000000000000),(20,11,14,3,419.000000000000000000000000000000),(21,12,4,1,5090.000000000000000000000000000000),(22,12,10,2,4690.000000000000000000000000000000),(23,12,14,3,419.000000000000000000000000000000),(24,13,36,1,690.000000000000000000000000000000),(25,13,29,2,403.000000000000000000000000000000),(26,14,10,1,4690.000000000000000000000000000000),(27,14,29,2,403.000000000000000000000000000000),(28,14,27,3,1090.000000000000000000000000000000),(29,15,34,1,5090.000000000000000000000000000000),(30,15,33,2,1190.000000000000000000000000000000),(31,16,4,1,5090.000000000000000000000000000000),(32,16,8,2,319.000000000000000000000000000000);
/*!40000 ALTER TABLE `orderitems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Total` decimal(65,30) NOT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Orders_UserId` (`UserId`),
  CONSTRAINT `FK_Orders_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,2,1298.000000000000000000000000000000,'Новый','2025-12-25 22:15:25.150549'),(2,2,1355.000000000000000000000000000000,'В обработке','2025-12-25 21:16:11.000000'),(3,2,3670.000000000000000000000000000000,'Отправлен','2025-12-25 20:16:11.000000'),(4,2,3794.000000000000000000000000000000,'Новый','2025-12-25 19:16:12.000000'),(5,2,749.000000000000000000000000000000,'Отменён','2025-12-25 18:16:12.000000'),(6,2,2888.000000000000000000000000000000,'Отправлен','2025-12-25 17:16:12.000000'),(7,2,890.000000000000000000000000000000,'Новый','2025-12-25 16:16:12.000000'),(8,2,4690.000000000000000000000000000000,'Новый','2025-12-25 15:16:12.000000'),(9,2,4890.000000000000000000000000000000,'Доставлен','2025-12-25 14:16:12.000000'),(10,2,14269.000000000000000000000000000000,'Новый','2025-12-25 13:16:12.000000'),(11,2,12327.000000000000000000000000000000,'Доставлен','2025-12-25 12:16:12.000000'),(12,2,15727.000000000000000000000000000000,'В обработке','2025-12-25 11:16:12.000000'),(13,2,1496.000000000000000000000000000000,'Новый','2025-12-25 10:16:12.000000'),(14,2,8766.000000000000000000000000000000,'Доставлен','2025-12-25 09:16:12.000000'),(15,2,7470.000000000000000000000000000000,'Новый','2025-12-25 08:16:12.000000'),(16,2,5728.000000000000000000000000000000,'Новый','2025-12-25 07:16:12.000000');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Sku` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Price` decimal(65,30) NOT NULL,
  `Stock` int NOT NULL,
  `CategoryId` int DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) NOT NULL,
  `ImageUrl` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_Products_CategoryId` (`CategoryId`),
  CONSTRAINT `FK_Products_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'11','аккумуляторная DORKEL DRS-3,6 1','Отвертка аккумуляторная DORKEL DRS-3,6',649.000000000000000000000000000000,19,2,'2025-12-24 19:11:04.363827','2025-12-25 20:52:36.799459','Images/4.jpeg'),(2,'22','Отвертка ARCHIMEDES 90358 2','Отвертка ARCHIMEDES 90358',219.000000000000000000000000000000,44,2,'2025-12-23 19:11:04.372232','2025-12-25 19:11:04.372232','Images/6.jpeg'),(3,'33','Уровень лазерный ADA LaserMarker 3','Уровень лазерный ADA LaserMarker',1190.000000000000000000000000000000,15,3,'2025-12-22 19:11:04.372275','2025-12-25 19:11:04.372275','Images/5.jpeg'),(4,'44','Краскопульт HAMMER PRZ500B 4','Краскопульт HAMMER PRZ500B',5090.000000000000000000000000000000,47,1,'2025-12-21 19:11:04.372282','2025-12-25 19:11:04.372282','Images/6.jpeg'),(5,'55','Ключ GRIFF 31111 5','Ключ GRIFF 31111',3.000000000000000000000000000000,16,2,'2025-12-20 19:11:04.372292','2025-12-25 19:11:04.372292','Images/1.jpeg'),(6,'66','Лазерный дальномер ADA Cosmo 70 6','Лазерный дальномер ADA Cosmo 70',690.000000000000000000000000000000,47,3,'2025-12-19 19:11:04.372297','2025-12-25 19:11:04.372297','Images/1.jpeg'),(7,'17','Отвертка аккумуляторная DORKEL DRS-3,6 7','Отвертка аккумуляторная DORKEL DRS-3,6',749.000000000000000000000000000000,40,2,'2025-12-18 19:11:04.372301','2025-12-25 19:11:04.372301','Images/1.jpeg'),(8,'28','Отвертка ARCHIMEDES 90358 8','Отвертка ARCHIMEDES 90358',319.000000000000000000000000000000,26,2,'2025-12-17 19:11:04.372306','2025-12-25 19:11:04.372306','Images/1.jpeg'),(9,'39','Уровень лазерный ADA LaserMarker 9','Уровень лазерный ADA LaserMarker',1290.000000000000000000000000000000,32,3,'2025-12-16 19:11:04.372312','2025-12-25 19:11:04.372312','Images/6.jpeg'),(10,'410','Краскопульт HAMMER PRZ500B 10','Краскопульт HAMMER PRZ500B',4690.000000000000000000000000000000,15,1,'2025-12-15 19:11:04.372317','2025-12-25 19:11:04.372317','Images/6.jpeg'),(11,'511','Ключ GRIFF 31111 11','Ключ GRIFF 31111',103.000000000000000000000000000000,20,2,'2025-12-14 19:11:04.372325','2025-12-25 19:11:04.372325','Images/3.jpeg'),(12,'612','Лазерный дальномер ADA Cosmo 70 12','Лазерный дальномер ADA Cosmo 70',790.000000000000000000000000000000,43,3,'2025-12-13 19:11:04.372331','2025-12-25 19:11:04.372331','Images/1.jpeg'),(13,'113','Отвертка аккумуляторная DORKEL DRS-3,6 13','Отвертка аккумуляторная DORKEL DRS-3,6',849.000000000000000000000000000000,47,2,'2025-12-12 19:11:04.372336','2025-12-25 19:11:04.372336','Images/5.jpeg'),(14,'214','Отвертка ARCHIMEDES 90358 14','Отвертка ARCHIMEDES 90358',419.000000000000000000000000000000,23,2,'2025-12-11 19:11:04.372342','2025-12-25 19:11:04.372342','Images/6.jpeg'),(15,'315','Уровень лазерный ADA LaserMarker 15','Уровень лазерный ADA LaserMarker',890.000000000000000000000000000000,37,3,'2025-12-10 19:11:04.372346','2025-12-25 19:11:04.372347','Images/5.jpeg'),(16,'416','Краскопульт HAMMER PRZ500B 16','Краскопульт HAMMER PRZ500B',4790.000000000000000000000000000000,34,1,'2025-12-09 19:11:04.372351','2025-12-25 19:11:04.372351','Images/6.jpeg'),(17,'517','Ключ GRIFF 31111 17','Ключ GRIFF 31111',203.000000000000000000000000000000,29,2,'2025-12-08 19:11:04.372356','2025-12-25 19:11:04.372356','Images/1.jpeg'),(18,'618','Лазерный дальномер ADA Cosmo 70 18','Лазерный дальномер ADA Cosmo 70',890.000000000000000000000000000000,3,3,'2025-12-07 19:11:04.372361','2025-12-25 19:11:04.372361','Images/4.jpeg'),(19,'119','Отвертка аккумуляторная DORKEL DRS-3,6 19','Отвертка аккумуляторная DORKEL DRS-3,6',949.000000000000000000000000000000,4,2,'2025-12-06 19:11:04.372369','2025-12-25 19:11:04.372369','Images/6.jpeg'),(20,'220','Отвертка ARCHIMEDES 90358 20','Отвертка ARCHIMEDES 90358',19.000000000000000000000000000000,6,2,'2025-12-05 19:11:04.372373','2025-12-25 19:11:04.372373','Images/4.jpeg'),(21,'321','Уровень лазерный ADA LaserMarker 21','Уровень лазерный ADA LaserMarker',990.000000000000000000000000000000,38,3,'2025-12-04 19:11:04.372377','2025-12-25 19:11:04.372377','Images/2.jpeg'),(22,'422','Краскопульт HAMMER PRZ500B 22','Краскопульт HAMMER PRZ500B',4890.000000000000000000000000000000,29,1,'2025-12-03 19:11:04.372383','2025-12-25 19:11:04.372383','Images/3.jpeg'),(23,'523','Ключ GRIFF 31111 23','Ключ GRIFF 31111',303.000000000000000000000000000000,27,2,'2025-12-02 19:11:04.372388','2025-12-25 19:11:04.372389','Images/1.jpeg'),(24,'624','Лазерный дальномер ADA Cosmo 70 24','Лазерный дальномер ADA Cosmo 70',990.000000000000000000000000000000,6,3,'2025-12-01 19:11:04.372393','2025-12-25 19:11:04.372393','Images/2.jpeg'),(25,'125','Отвертка аккумуляторная DORKEL DRS-3,6 25','Отвертка аккумуляторная DORKEL DRS-3,6',549.000000000000000000000000000000,31,2,'2025-11-30 19:11:04.372397','2025-12-25 19:11:04.372397','Images/2.jpeg'),(26,'226','Отвертка ARCHIMEDES 90358 26','Отвертка ARCHIMEDES 90358',119.000000000000000000000000000000,10,2,'2025-11-29 19:11:04.372402','2025-12-25 19:11:04.372402','Images/3.jpeg'),(27,'327','Уровень лазерный ADA LaserMarker 27','Уровень лазерный ADA LaserMarker',1090.000000000000000000000000000000,44,3,'2025-11-28 19:11:04.372407','2025-12-25 19:11:04.372407','Images/4.jpeg'),(28,'428','Краскопульт HAMMER PRZ500B 28','Краскопульт HAMMER PRZ500B',4990.000000000000000000000000000000,14,1,'2025-11-27 19:11:04.372415','2025-12-25 19:11:04.372415','Images/2.jpeg'),(29,'529','Ключ GRIFF 31111 29','Ключ GRIFF 31111',403.000000000000000000000000000000,19,2,'2025-11-26 19:11:04.372420','2025-12-25 19:11:04.372420','Images/5.jpeg'),(30,'630','Лазерный дальномер ADA Cosmo 70 30','Лазерный дальномер ADA Cosmo 70',590.000000000000000000000000000000,44,3,'2025-11-25 19:11:04.372426','2025-12-25 19:11:04.372426','Images/4.jpeg'),(31,'131','Отвертка аккумуляторная DORKEL DRS-3,6 31','Отвертка аккумуляторная DORKEL DRS-3,6',649.000000000000000000000000000000,37,2,'2025-11-24 19:11:04.372431','2025-12-25 19:11:04.372431','Images/1.jpeg'),(32,'232','Отвертка ARCHIMEDES 90358 32','Отвертка ARCHIMEDES 90358',219.000000000000000000000000000000,24,2,'2025-11-23 19:11:04.372436','2025-12-25 19:11:04.372436','Images/4.jpeg'),(33,'333','Уровень лазерный ADA LaserMarker 33','Уровень лазерный ADA LaserMarker',1190.000000000000000000000000000000,25,3,'2025-11-22 19:11:04.372443','2025-12-25 19:11:04.372443','Images/3.jpeg'),(34,'434','Краскопульт HAMMER PRZ500B 34','Краскопульт HAMMER PRZ500B',5090.000000000000000000000000000000,46,1,'2025-11-21 19:11:04.372448','2025-12-25 19:11:04.372448','Images/5.jpeg'),(35,'535','Ключ GRIFF 31111 35','Ключ GRIFF 31111',3.000000000000000000000000000000,26,2,'2025-11-20 19:11:04.372457','2025-12-25 19:11:04.372457','Images/2.jpeg'),(36,'636','Лазерный дальномер ADA Cosmo 70 36','Лазерный дальномер ADA Cosmo 70',690.000000000000000000000000000000,12,3,'2025-11-19 19:11:04.372461','2025-12-25 19:11:04.372461','Images/1.jpeg'),(37,'137','Отвертка аккумуляторная DORKEL DRS-3,6 37','Отвертка аккумуляторная DORKEL DRS-3,6',749.000000000000000000000000000000,10,2,'2025-11-18 19:11:04.372466','2025-12-25 19:11:04.372466','Images/3.jpeg'),(38,'238','Отвертка ARCHIMEDES 90358 38','Отвертка ARCHIMEDES 90358',319.000000000000000000000000000000,17,2,'2025-11-17 19:11:04.372473','2025-12-25 19:11:04.372473','Images/1.jpeg'),(39,'339','Уровень лазерный ADA LaserMarker 39','Уровень лазерный ADA LaserMarker',1290.000000000000000000000000000000,33,3,'2025-11-16 19:11:04.372478','2025-12-25 19:11:04.372478','Images/3.jpeg'),(40,'440','Краскопульт HAMMER PRZ500B 40','Краскопульт HAMMER PRZ500B',4690.000000000000000000000000000000,31,1,'2025-11-15 19:11:04.372484','2025-12-25 19:11:04.372484','Images/5.jpeg');
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reports`
--

DROP TABLE IF EXISTS `reports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reports` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedBy` int DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `Payload` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reports`
--

LOCK TABLES `reports` WRITE;
/*!40000 ALTER TABLE `reports` DISABLE KEYS */;
/*!40000 ALTER TABLE `reports` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'Администратор','Админ системы'),(2,'Пользователь','Обычный пользователь');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` int NOT NULL,
  `Phone` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhotoUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Users_RoleId` (`RoleId`),
  CONSTRAINT `FK_Users_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin@local','$2a$11$Q.3SasKbR.RCdHXFV1z7F.KdUfjdqCxAfsbYZpQIfI47xib9yzbPm','Админ',1,NULL,NULL,'2025-12-25 19:12:46.325949'),(2,'user@local','$2a$11$4gw4j7cPr2pW/wXPSdRVReRnssP4QUEEb8SeahQDwKEDreWpXsefW','Тестовый пользователь',2,NULL,NULL,'2025-12-25 19:14:06.998518');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database '220volt'
--

--
-- Dumping routines for database '220volt'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-12-26  1:23:43
