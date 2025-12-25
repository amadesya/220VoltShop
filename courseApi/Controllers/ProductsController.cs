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
		public ProductsController(CourseStoreContext db) { _db = db; }

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
				Console.WriteLine($"Products.Get invoked. ModelState.IsValid={ModelState.IsValid}");
				if (!ModelState.IsValid)
				{
					foreach (var kv in ModelState)
					{
						Console.WriteLine($"ModelState[{kv.Key}] = {kv.Value?.Errors?.Count}");
						foreach (var err in kv.Value?.Errors ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError>())
						{
							Console.WriteLine($" - {err.ErrorMessage} | Exception: {err.Exception}");
						}
					}
				}

				// parse paging manually to avoid automatic model-binding 400 responses
				if (!int.TryParse(page, out var pageNum)) pageNum = 1;
				if (!int.TryParse(pageSize, out var pageSizeNum)) pageSizeNum = 20;

				var query = _db.Products.AsQueryable();

				if (!string.IsNullOrWhiteSpace(q))
					query = query.Where(p => p.Name.Contains(q) || p.Description.Contains(q) || p.Sku.Contains(q));

				if (categoryId.HasValue)
					query = query.Where(p => p.CategoryId == categoryId.Value);

				query = sort switch
				{
					"price_asc" => query.OrderBy(p => p.Price),
					"price_desc" => query.OrderByDescending(p => p.Price),
					"newest" => query.OrderByDescending(p => p.CreatedAt),
					_ => query.OrderBy(p => p.Name)
				};

				var total = await query.CountAsync();
				var pageProducts = await query.Skip((pageNum - 1) * pageSizeNum).Take(pageSizeNum)
					.Select(p => new {
						id = p.Id,
						sku = p.Sku,
						name = p.Name,
						price = p.Price,
						stock = p.Stock,
						category = p.CategoryId,
						description = p.Description
					})
					.ToListAsync();

				// resolve image file existence (look for common extensions in courseProd/Images)
				string solutionRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(Directory.GetCurrentDirectory(), ".."));
				string imagesDir = System.IO.Path.Combine(solutionRoot, "courseProd", "Images");

				var items = pageProducts.Select(p => {
					string[] exts = new[] { ".jpeg", ".jpg", ".png", ".svg" };
					string found = null;
					foreach(var ext in exts)
					{
						var path = System.IO.Path.Combine(imagesDir, p.sku + ext);
						if (System.IO.File.Exists(path)) { found = "/images/" + p.sku + ext; break; }
					}
					if (found == null) found = "/images/placeholder.svg";

					// small special-offer support: mark sku 4 as discounted (example)
					int discountPercent = 0;
					if (p.sku == "4") discountPercent = 15;

					return new {
						id = p.id,
						sku = p.sku,
						name = p.name,
						price = p.price,
						stock = p.stock,
						category = p.category,
						description = p.description,
						image = found,
						discountPercent = discountPercent
					};
				}).ToList();

				return Ok(new { total, page = pageNum, pageSize = pageSizeNum, items });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Products.Get exception: {ex}");
				return Problem(detail: ex.ToString());
			}
		}

		// ...existing code...
	}
}