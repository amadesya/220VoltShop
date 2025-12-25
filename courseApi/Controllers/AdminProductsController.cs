using Microsoft.AspNetCore.Mvc;
using courseApi.Models;
using courseApi.Data; // Укажите правильное пространство имен для вашего контекста
using System.Linq;

namespace courseApi.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    public class AdminProductsController : ControllerBase
    {
        private readonly CourseStoreContext _db;

        // Внедрение контекста через конструктор
        public AdminProductsController(CourseStoreContext db)
        {
            _db = db;
        }

        // GET api/admin/products
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_db.Products.ToList());
        }

        // GET api/admin/products/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProductDto model)
        {
            if (model == null) return BadRequest();

            var product = new Product
            {
                Sku = model.Sku,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CategoryId = model.CategoryId
            };

            _db.Products.Add(product);
            _db.SaveChanges();

            return Ok(new { success = true, message = "Товар успешно создан" });
        }

        public class ProductDto
        {
            public string Sku { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public int? CategoryId { get; set; }
            public string ImageUrl { get; set; }
        }

        // PUT api/admin/products/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProductDto model)
        {
            var existingProduct = _db.Products.Find(id);
            if (existingProduct == null)
                return NotFound(new { success = false, message = "Продукт не найден" });

            existingProduct.Sku = model.Sku;
            existingProduct.Name = model.Name;
            existingProduct.Description = model.Description;
            existingProduct.Price = model.Price;
            existingProduct.Stock = model.Stock;
            existingProduct.CategoryId = model.CategoryId;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            try
            {
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ошибка при сохранении данных" });
            }

            return Ok(new { success = true, message = "Товар успешно обновлён" });
        }


        // DELETE api/admin/products/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            _db.Products.Remove(product);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
