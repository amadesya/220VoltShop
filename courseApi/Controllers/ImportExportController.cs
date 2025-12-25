using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using courseApi.Data;
using courseApi.Attributes;
using System.IO;
using ClosedXML.Excel; // require: dotnet add package ClosedXML
using System.Linq;

namespace courseApi.Controllers
{
	[ApiController]
	[Route("api/admin/[controller]")]
	[AdminAuthorize]
	public class ImportExportController : ControllerBase
	{
		private readonly CourseStoreContext _db;
		public ImportExportController(CourseStoreContext db) { _db = db; }

		[HttpGet("export/products")]
		public async Task<IActionResult> ExportProducts()
		{
			var products = await _db.Products.Include(p => p.Category).ToListAsync();
			using var wb = new XLWorkbook();
			var ws = wb.Worksheets.Add("Products");
			ws.Cell(1,1).Value = "Id";
			ws.Cell(1,2).Value = "Sku";
			ws.Cell(1,3).Value = "Name";
			ws.Cell(1,4).Value = "Description";
			ws.Cell(1,5).Value = "Price";
			ws.Cell(1,6).Value = "Stock";
			ws.Cell(1,7).Value = "CategoryId";

			for (int i=0;i<products.Count;i++)
			{
				var r = i+2;
				var p = products[i];
				ws.Cell(r,1).Value = p.Id;
				ws.Cell(r,2).Value = p.Sku;
				ws.Cell(r,3).Value = p.Name;
				ws.Cell(r,4).Value = p.Description;
				ws.Cell(r,5).Value = p.Price;
				ws.Cell(r,6).Value = p.Stock;
				ws.Cell(r,7).Value = p.CategoryId;
			}

			using var ms = new MemoryStream();
			wb.SaveAs(ms);
			ms.Position = 0;
			return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
		}

		[HttpGet("export/orders")]
		public async Task<IActionResult> ExportOrders()
		{
			var orders = await _db.Orders
					.Include(o => o.User)
					.Include(o => o.Items)
					.ToListAsync();

			using var wb = new XLWorkbook();
			var ws = wb.Worksheets.Add("Orders");
			ws.Cell(1,1).Value = "OrderId";
			ws.Cell(1,2).Value = "UserEmail";
			ws.Cell(1,3).Value = "Total";
			ws.Cell(1,4).Value = "Status";
			ws.Cell(1,5).Value = "CreatedAt";
			ws.Cell(1,6).Value = "Items"; // json

			for (int i=0;i<orders.Count;i++)
			{
				var r = i+2;
				var o = orders[i];
				ws.Cell(r,1).Value = o.Id;
				ws.Cell(r,2).Value = o.User?.Email;
				ws.Cell(r,3).Value = o.Total;
				ws.Cell(r,4).Value = o.Status;
				ws.Cell(r,5).Value = o.CreatedAt;

				// simple items serialization
				var itemsText = string.Join("; ", o.Items.Select(it => $"{it.ProductId}x{it.Quantity}@{it.UnitPrice}"));
				ws.Cell(r,6).Value = itemsText;
			}

			using var ms = new MemoryStream();
			wb.SaveAs(ms);
			ms.Position = 0;
			return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "orders.xlsx");
		}

		[HttpPost("import/products")]
		public async Task<IActionResult> ImportProducts([FromForm] Microsoft.AspNetCore.Http.IFormFile file)
		{
			if (file == null) return BadRequest("file required");
			using var ms = new MemoryStream();
			await file.CopyToAsync(ms);
			using var wb = new XLWorkbook(ms);
			var ws = wb.Worksheets.First();
			var rows = ws.RowsUsed().Skip(1);
			foreach (var row in rows)
			{
				var idCell = row.Cell(1).GetValue<int?>();
				var sku = row.Cell(2).GetString();
				var name = row.Cell(3).GetString();
				var desc = row.Cell(4).GetString();
				var price = row.Cell(5).GetValue<decimal>();
				var stock = row.Cell(6).GetValue<int>();
				var catId = row.Cell(7).GetValue<int?>();

				if (idCell.HasValue)
				{
					var existing = await _db.Products.FindAsync(idCell.Value);
					if (existing != null)
					{
						existing.Sku = sku;
						existing.Name = name;
						existing.Description = desc;
						existing.Price = price;
						existing.Stock = stock;
						existing.CategoryId = catId;
					}
					else
					{
						_db.Products.Add(new Models.Product { Sku = sku, Name = name, Description = desc, Price = price, Stock = stock, CategoryId = catId });
					}
				}
				else
				{
					_db.Products.Add(new Models.Product { Sku = sku, Name = name, Description = desc, Price = price, Stock = stock, CategoryId = catId });
				}
			}
			await _db.SaveChangesAsync();
			return Ok(new { status = "imported" });
		}
	}
}
