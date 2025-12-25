using ClosedXML.Excel;
using courseApi.Data;
using courseApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace courseApi.Controllers
{
    [ApiController]
    [Route("api/admin/importexport/export")]
    [Authorize(Roles = "Администратор")]
    public class ExportController : ControllerBase
    {
        private readonly CourseStoreContext _db;
        private readonly JwtService _jwt;

        public ExportController(CourseStoreContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpGet("orders")]
        public IActionResult ExportOrders()
        {
            var orders = _db.Orders.Include(o => o.Items).ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Orders");

            ws.Cell(1, 1).Value = "Order Id";
            ws.Cell(1, 2).Value = "User Id";
            ws.Cell(1, 3).Value = "Total";
            ws.Cell(1, 4).Value = "Status";
            ws.Cell(1, 5).Value = "Created At";
            ws.Cell(1, 6).Value = "Product Id";
            ws.Cell(1, 7).Value = "Quantity";
            ws.Cell(1, 8).Value = "Unit Price";

            int row = 2;
            foreach (var order in orders)
            {
                foreach (var item in order.Items)
                {
                    ws.Cell(row, 1).Value = order.Id;
                    ws.Cell(row, 2).Value = order.UserId;
                    ws.Cell(row, 3).Value = order.Total;
                    ws.Cell(row, 4).Value = order.Status;
                    ws.Cell(row, 5).Value = order.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                    ws.Cell(row, 6).Value = item.ProductId;
                    ws.Cell(row, 7).Value = item.Quantity;
                    ws.Cell(row, 8).Value = item.UnitPrice;
                    row++;
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "orders.xlsx"
            );
        }
    }

}
