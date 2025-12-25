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

        // POST api/admin/products
        [HttpPost]
        public IActionResult Create([FromBody] Product model)
        {
            if (model == null) return BadRequest();

            _db.Products.Add(model);
            _db.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        // PUT api/admin/products/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product model)
        {
            var existingProduct = _db.Products.Find(id);
            if (existingProduct == null)
                return NotFound();

            // Обновляем свойства (пример)
            existingProduct.Name = model.Name;
            existingProduct.Price = model.Price;
            // добавьте остальные поля...

            _db.SaveChanges();
            return NoContent();
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
