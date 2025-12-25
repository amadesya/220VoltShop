using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using courseApi.Data;
using courseApi.Models;

namespace courseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly CourseStoreContext _db;

        public ProductsController(CourseStoreContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? q,
            [FromQuery] int? categoryId,
            [FromQuery] string? sort = "name",
            [FromQuery] string? page = "1",
            [FromQuery] string? pageSize = "20")
        {
            try
            {
                // 1. Парсинг пагинации
                if (!int.TryParse(page, out var pageNum)) pageNum = 1;
                if (!int.TryParse(pageSize, out var pageSizeNum)) pageSizeNum = 20;

                // 2. Базовый запрос
                var query = _db.Products.AsQueryable();

                // 3. Фильтрация (Поиск)
                if (!string.IsNullOrWhiteSpace(q))
                {
                    var lowerQ = q.ToLower();
                    query = query.Where(p => p.Name.ToLower().Contains(lowerQ) ||
                                             p.Description.ToLower().Contains(lowerQ) ||
                                             p.Sku.ToLower().Contains(lowerQ));
                }

                if (categoryId.HasValue)
                    query = query.Where(p => p.CategoryId == categoryId.Value);

                // 4. Сортировка
                query = sort switch
                {
                    "price_asc" => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "newest" => query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.Name)
                };

                var total = await query.CountAsync();
                var pageProducts = await query
                    .Skip((pageNum - 1) * pageSizeNum)
                    .Take(pageSizeNum)
                    .ToListAsync();

                // 5. Формирование ответа
                var items = pageProducts.Select(p => {

                    // Берем путь напрямую из БД (поле ImageUrl)
                    string imageUrl = p.ImageUrl;

                    // Если в БД пусто, ставим заглушку
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        imageUrl = "Images/placeholder.svg";
                    }

                    // Гарантируем, что путь начинается с / для корректной работы URL
                    if (!imageUrl.StartsWith("/"))
                    {
                        imageUrl = "/" + imageUrl;
                    }

                    return new
                    {
                        id = p.Id,
                        sku = p.Sku,
                        name = p.Name,
                        price = p.Price,
                        stock = p.Stock,
                        category = p.CategoryId,
                        description = p.Description,
                        image = imageUrl, // Это поле пойдет на фронтенд
                        discountPercent = (p.Sku == "612" ? 15 : 0) // Пример скидки для вашего SKU
                    };
                }).ToList();

                return Ok(new { total, page = pageNum, pageSize = pageSizeNum, items });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Products.Get: {ex.Message}");
                return Problem(detail: "Ошибка при загрузке товаров");
            }
        }
    }
}