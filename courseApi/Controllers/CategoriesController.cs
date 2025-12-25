using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using courseApi.Data;

namespace courseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CourseStoreContext _db;
        public CategoriesController(CourseStoreContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cats = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
            return Ok(cats.Select(c => new { id = c.Id, name = c.Name }));
        }
    }
}
