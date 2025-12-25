using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using courseApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace courseApi.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var req = context.HttpContext.Request;

			// try header
			var authHeader = req.Headers["Authorization"].FirstOrDefault();
			string token = null;
			if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
			{
				token = authHeader.Substring("Bearer ".Length).Trim();
			}

			// fallback: allow token via query string for simple admin downloads in dev
			if (string.IsNullOrEmpty(token) && req.Query.ContainsKey("token"))
			{
				token = req.Query["token"].FirstOrDefault();
			}

			if (string.IsNullOrWhiteSpace(token))
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			var jwt = context.HttpContext.RequestServices.GetService<JwtService>();
			if (jwt == null)
			{
				context.Result = new ForbidResult();
				return;
			}
			var principal = jwt.ValidateToken(token);
			if (principal == null)
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
			if (string.IsNullOrWhiteSpace(roleClaim))
			{
				context.Result = new ForbidResult();
				return;
			}

			// accept several common admin role names (russian and english)
			var allowed = new[] { "Администратор", "Aдминистратор", "Administrator", "Admin" };
			if (!allowed.Any(a => roleClaim.Contains(a)))
			{
				context.Result = new ForbidResult();
				return;
			}

			// ok
		}
	}
}