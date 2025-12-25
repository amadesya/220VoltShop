using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using courseApi.Data;

namespace courseApi.Controllers
{
    [ApiController]
    [Route("api/dev")]
    public class DevController : ControllerBase
    {
        private readonly CourseStoreContext _db;
        private readonly IHostEnvironment _env;
        public DevController(CourseStoreContext db, IHostEnvironment env) { _db = db; _env = env; }

        [HttpPost("seed")]
        public IActionResult Seed()
        {
            if (!_env.IsDevelopment()) return Forbid();
            try
            {
                DbSeeder.Seed(_db);
                return Ok(new { status = "seeded" });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }
    }
}
