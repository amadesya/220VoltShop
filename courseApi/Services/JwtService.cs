using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using courseApi.Models;

namespace courseApi.Services
{
	public class JwtService
	{
		private readonly byte[] _key;
		private readonly IConfiguration _config;

		public JwtService(IConfiguration config)
		{
			_config = config;
			var secret = _config["Jwt:Secret"] ?? "please_replace_with_a_long_secure_random_secret_in_production";
			_key = Encoding.ASCII.GetBytes(secret);
		}

		public string GenerateToken(User user, int? expireMinutes = null)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email ?? ""),
				new Claim(ClaimTypes.Name, user.Name ?? ""),
				new Claim(ClaimTypes.Role, user.Role?.Name ?? "")
			};
			var descriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(expireMinutes ?? int.Parse(_config["Jwt:ExpireMinutes"] ?? "60")),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(descriptor);
			return tokenHandler.WriteToken(token);
		}

		public ClaimsPrincipal ValidateToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				var parameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(_key),
					ClockSkew = TimeSpan.Zero
				};
				var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
				return principal;
			}
			catch
			{
				return null;
			}
		}
	}
}