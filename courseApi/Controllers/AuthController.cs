using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using courseApi.Data;
using courseApi.Services;
using courseApi.Models;

namespace courseApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly CourseStoreContext _db;
		private readonly JwtService _jwt;
		public AuthController(CourseStoreContext db, JwtService jwt) { _db = db; _jwt = jwt; }

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == dto.Email);
			if (user == null) return Unauthorized(new { error = "invalid_credentials" });

			// В проде: храните user.PasswordHash как BCrypt-хеш и используйте BCrypt.Verify.
			// Установите пакет: dotnet add package BCrypt.Net-Next
			bool passwordOk = false;
			try
			{
				// если хранится хеш
				passwordOk = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
			}
			catch
			{
				// fallback (для старых данных) — НЕ оставляйте в проде
				passwordOk = user.PasswordHash == dto.Password;
			}

			if (!passwordOk) return Unauthorized(new { error = "invalid_credentials" });

			var token = _jwt.GenerateToken(user, 60 * 24);
			return Ok(new { token, user = new { user.Id, user.Email, user.Name, role = user.Role?.Name } });
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
				return BadRequest(new { error = "email_password_required" });

			var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
			if (exists) return BadRequest(new { error = "email_exists" });

			// find user role
			var userRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Пользователь");
			if (userRole == null)
			{
				userRole = new Role { Name = "Пользователь", Description = "Обычный пользователь" };
				_db.Roles.Add(userRole);
				await _db.SaveChangesAsync();
			}

			var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
			var user = new User { Email = dto.Email, PasswordHash = hash, Name = dto.Name ?? dto.Email, RoleId = userRole.Id, CreatedAt = DateTime.UtcNow };
			_db.Users.Add(user);
			await _db.SaveChangesAsync();

			// include role
			await _db.Entry(user).Reference(u => u.Role).LoadAsync();
			var token = _jwt.GenerateToken(user, 60 * 24);
			return Ok(new { token, user = new { user.Id, user.Email, user.Name, role = user.Role?.Name } });
		}
	}

	public class LoginDto { public string Email { get; set; } public string Password { get; set; } }

	public class RegisterDto { public string Email { get; set; } public string Password { get; set; } public string Name { get; set; } }
}
