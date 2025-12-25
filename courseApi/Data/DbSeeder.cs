using System;
using System.Linq;
using courseApi.Models;

namespace courseApi.Data
{
    public static class DbSeeder
    {
        public static void Seed(CourseStoreContext db)
        {
            // seed minimal domain data
            if (db.Categories.Any() || db.Products.Any())
            {
                // ensure roles/users seeded if missing
                if (!db.Roles.Any())
                {
                    SeedRolesAndAdmin(db);
                }
                return;
            }

            var cats = new[] {
                new Category { Name = "Электроинструмент" },
                new Category { Name = "Ручной инструмент" },
                new Category { Name = "Измерительный инструмент" },
                new Category { Name = "Садовая техника" }
            };
            db.Categories.AddRange(cats);
            db.SaveChanges();

            var rnd = new Random(1234);
            var products = new Product[] {
                new Product { Sku = "1", Name = "Отвертка аккумуляторная DORKEL DRS-3,6", Description = "Отвертка аккумуляторная DORKEL DRS-3,6", Price = 549m, Stock = 15, CategoryId = cats[1].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { Sku = "2", Name = "Отвертка ARCHIMEDES 90358", Description = "Отвертка ARCHIMEDES 90358", Price = 19m, Stock = 30, CategoryId = cats[1].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { Sku = "3", Name = "Уровень лазерный ADA LaserMarker", Description = "Уровень лазерный ADA LaserMarker", Price = 890m, Stock = 10, CategoryId = cats[2].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                ,new Product { Sku = "4", Name = "Краскопульт HAMMER PRZ500B", Description = "Краскопульт HAMMER PRZ500B", Price = 4690m, Stock = 8, CategoryId = cats[0].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { Sku = "5", Name = "Ключ GRIFF 31111", Description = "Ключ GRIFF 31111", Price = 3m, Stock = 120, CategoryId = cats[1].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { Sku = "6", Name = "Лазерный дальномер ADA Cosmo 70", Description = "Лазерный дальномер ADA Cosmo 70", Price = 590m, Stock = 12, CategoryId = cats[2].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            // create many demo products to test pagination
            for (int i = 1; i <= 40; i++)
            {
                var baseProd = products[(i - 1) % products.Length];
                db.Products.Add(new Product {
                    Sku = baseProd.Sku + i,
                    Name = baseProd.Name + " " + i,
                    Description = baseProd.Description,
                    Price = baseProd.Price + (i % 5) * 100,
                    Stock = rnd.Next(0, 50),
                    CategoryId = baseProd.CategoryId,
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            db.SaveChanges();

            // roles + admin user
            SeedRolesAndAdmin(db);
        }

        private static void SeedRolesAndAdmin(CourseStoreContext db)
        {
            if (!db.Roles.Any())
            {
                var userRole = new Models.Role { Name = "Пользователь", Description = "Обычный пользователь" };
                var adminRole = new Models.Role { Name = "Администратор", Description = "Админ системы" };
                db.Roles.AddRange(userRole, adminRole);
                db.SaveChanges();

                // create admin user if missing
                if (!db.Users.Any(u => u.Email == "admin@local"))
                {
                    var pwd = BCrypt.Net.BCrypt.HashPassword("admin123");
                    db.Users.Add(new Models.User
                    {
                        Email = "admin@local",
                        PasswordHash = pwd,
                        Name = "Admin",
                        Phone = "+7 (900) 000-00-01",
                        PhotoUrl = "/images/avatars/admin.jpg",
                        RoleId = adminRole.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                if (!db.Users.Any(u => u.Email == "user@local"))
                {
                    var pwd = BCrypt.Net.BCrypt.HashPassword("user123");
                    db.Users.Add(new Models.User
                    {
                        Email = "user@local",
                        PasswordHash = pwd,
                            Name = "Demo User",
                            Phone = "+7 (900) 000-00-02",
                            PhotoUrl = "/images/avatars/user.jpg",
                        RoleId = db.Roles.First(r => r.Name == "Пользователь").Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                db.SaveChanges();
            }

            // В конец метода Seed(CourseStoreContext db)
            if (!db.Orders.Any())
            {
                var demoUser = db.Users.FirstOrDefault(u => u.Email == "user@local");
                var product = db.Products.FirstOrDefault();

                if (demoUser != null && product != null)
                {
                    var testOrder = new Order
                    {
                        UserId = demoUser.Id,
                        Total = product.Price * 2,
                        Status = "Новый",
                        CreatedAt = DateTime.UtcNow,
                        Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = 2,
                    UnitPrice = product.Price
                }
            }
                    };
                    db.Orders.Add(testOrder);
                    db.SaveChanges();
                }
            }
        }
    }
}
