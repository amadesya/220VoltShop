using Microsoft.AspNetCore.Mvc;
using courseApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace courseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly JwtService _jwt;
        private readonly Data.CourseStoreContext _db;
        public UsersController(JwtService jwt, Data.CourseStoreContext db) { _jwt = jwt; _db = db; }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var auth = Request.Headers["Authorization"].ToString();
            string token = null;
            if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer ")) token = auth.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrWhiteSpace(token) && Request.Query.ContainsKey("token")) token = Request.Query["token"].ToString();
            if (string.IsNullOrWhiteSpace(token)) return Unauthorized();

            var principal = _jwt.ValidateToken(token);
            if (principal == null) return Unauthorized();
            var idc = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idc, out var id)) return Unauthorized();

            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            return Ok(new { id = user.Id, email = user.Email, name = user.Name, role = user.Role?.Name, phone = user.Phone, photo = user.PhotoUrl });
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateMeDto dto)
        {
            var auth = Request.Headers["Authorization"].ToString();
            string token = null;
            if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer ")) token = auth.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrWhiteSpace(token) && Request.Query.ContainsKey("token")) token = Request.Query["token"].ToString();
            if (string.IsNullOrWhiteSpace(token)) return Unauthorized();

            var principal = _jwt.ValidateToken(token);
            if (principal == null) return Unauthorized();
            var idc = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idc, out var id)) return Unauthorized();

            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.PhotoUrl)) user.PhotoUrl = dto.PhotoUrl;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return Ok(new { status = "updated" });
        }
    }

    public class UpdateMeDto { public string Name { get; set; } public string Phone { get; set; } public string PhotoUrl { get; set; } }
}
