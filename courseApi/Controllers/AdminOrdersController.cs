using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using courseApi.Data;
using courseApi.Attributes;
using System.Linq;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.IO;

namespace courseApi.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly CourseStoreContext _db;

        public AdminOrdersController(CourseStoreContext db)
        {
            _db = db;
        }

        // --- DTO Модели ---

        public class OrderItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class OrderDto
        {
            public int Id { get; set; }
            public string UserEmail { get; set; } // Добавлено для админки
            public decimal Total { get; set; }
            public string Status { get; set; }
            public string CreatedAt { get; set; }
            public List<OrderItemDto> Items { get; set; }
        }

        public class UpdateStatusDto
        {
            public string Status { get; set; }
        }

        // --- Методы API ---

        // GET: api/admin/AdminOrders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Теперь этот метод доступен любому, кто знает URL
            var orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var list = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserEmail = o.User?.Email ?? "Гость",
                Total = o.Total,
                Status = o.Status,
                CreatedAt = o.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                Items = o.Items.Select(it => new OrderItemDto
                {
                    ProductId = it.ProductId,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice
                }).ToList()
            }).ToList();

            return Ok(list);
        }

        // PUT: api/admin/AdminOrders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] UpdateStatusDto body)
        {
            if (body == null || string.IsNullOrEmpty(body.Status))
                return BadRequest("Статус не указан");

            var existing = await _db.Orders.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Status = body.Status;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/admin/AdminOrders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Orders.FindAsync(id);
            if (existing == null) return NotFound();

            _db.Orders.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            var orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                var currentRow = 1;

                // Заголовки
                worksheet.Cell(currentRow, 1).Value = "ID Заказа";
                worksheet.Cell(currentRow, 2).Value = "Дата";
                worksheet.Cell(currentRow, 3).Value = "Email пользователя";
                worksheet.Cell(currentRow, 4).Value = "Статус";
                worksheet.Cell(currentRow, 5).Value = "Сумма (₽)";

                // Стилизация заголовков
                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Данные
                foreach (var order in orders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = order.Id;
                    worksheet.Cell(currentRow, 2).Value = order.CreatedAt.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(currentRow, 3).Value = order.User?.Email ?? "Гость";
                    worksheet.Cell(currentRow, 4).Value = order.Status;
                    worksheet.Cell(currentRow, 5).Value = order.Total;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Orders_Export_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
        }
    }
}