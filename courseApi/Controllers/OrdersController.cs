using ClosedXML.Excel;
using courseApi.Data;
using courseApi.Models;
using courseApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace courseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly CourseStoreContext _db;
        private readonly JwtService _jwt;
        public OrdersController(CourseStoreContext db, JwtService jwt) { _db = db; _jwt = jwt; }

        public class OrderItemDto
        {
            public int? ProductId { get; set; }
            public string Sku { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class OrderCreateDto
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string PaymentMethod { get; set; }
            public decimal Total { get; set; }
            public List<OrderItemDto> Items { get; set; }
        }

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto model)
        {
            var token = GetTokenFromHeader();
            var principal = _jwt.ValidateToken(token);
            if (principal == null) return Unauthorized();
            var idClaim = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            if (model == null || model.Items == null || !model.Items.Any()) return BadRequest("items required");

            var order = new Order
            {
                UserId = userId,
                Total = model.Total,
                Status = "Новый",
                CreatedAt = DateTime.Now,
                Items = new List<OrderItem>()
            };

            foreach (var it in model.Items)
            {
                int pid = 0;
                if (it.ProductId.HasValue) pid = it.ProductId.Value;
                else if (!string.IsNullOrWhiteSpace(it.Sku))
                {
                    var prod = await _db.Products.FirstOrDefaultAsync(p => p.Sku == it.Sku);
                    if (prod != null) pid = prod.Id;
                }
                if (pid == 0) continue;
                order.Items.Add(new OrderItem { ProductId = pid, Quantity = it.Quantity, UnitPrice = it.UnitPrice });
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return Ok(new { id = order.Id });
        }

        // GET api/orders/my - returns orders of the authenticated user (token required)
        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var token = GetTokenFromHeader();
            var principal = _jwt.ValidateToken(token);
            if (principal == null) return Unauthorized();
            var idClaim = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            var orders = await _db.Orders.Where(o => o.UserId == userId).Include(o => o.Items).OrderByDescending(o => o.CreatedAt).ToListAsync();
            var list = orders.Select(o => new
            {
                id = o.Id,
                total = o.Total,
                status = o.Status,
                createdAt = o.CreatedAt,
                items = o.Items.Select(it => new { it.ProductId, it.Quantity, it.UnitPrice })
            });
            return Ok(list);
        }

        // GET api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var token = GetTokenFromHeader();
            var principal = _jwt.ValidateToken(token);
            if (principal == null) return Unauthorized();
            var idClaim = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            var o = await _db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (o == null) return NotFound();
            return Ok(new { id = o.Id, total = o.Total, status = o.Status, createdAt = o.CreatedAt, items = o.Items.Select(it => new { it.ProductId, it.Quantity, it.UnitPrice }) });
        }

        string GetTokenFromHeader()
        {
            var auth = Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer ")) return auth.Substring("Bearer ".Length).Trim();
            if (Request.Query.ContainsKey("token")) return Request.Query["token"].FirstOrDefault();
            return string.Empty;
        }

    }
}
