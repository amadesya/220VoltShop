using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using courseApi.Data;
using courseApi.Attributes;

namespace courseApi.Controllers
{
    [ApiController]
    [Route("api/admin/categories")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly CourseStoreContext _db;
        public AdminCategoriesController(CourseStoreContext db) { _db = db; }

        public class CategoryDto
        {
            public string Name { get; set; }
            public int? ParentId { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _db.Categories.OrderBy(c=>c.Name).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDto model)
        {
            var category = new Models.Category
            {
                Name = model.Name,
                ParentId = model.ParentId
            };
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = category.Id }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto model)
        {
            var existing = await _db.Categories.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = model.Name;
            existing.ParentId = model.ParentId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Categories.FindAsync(id);
            if (existing == null) return NotFound();
            _db.Categories.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
