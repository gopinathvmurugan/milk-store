using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using OrderService.Hubs;   // 👈 your hub namespace

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IHubContext<OrderHub> _hub; // 👈 ADD
        private readonly HttpClient _httpClient;
        public OrderController(OrderDbContext context, IHubContext<OrderHub> hub, IHttpClientFactory factory)
        {
            _context = context;
            _hub = hub; // 👈 ADD
            _httpClient = factory.CreateClient();
        }
    


        [Authorize]
        [HttpPost]
       
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (order == null || order.Items == null || !order.Items.Any())
                return BadRequest("Invalid order");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId missing");

            order.UserId = userId;
            order.Id = Guid.NewGuid();
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";

            // ✅ CHECK + REDUCE STOCK
            foreach (var item in order.Items)
            {
                // 👉 GET PRODUCT
                var response = await _httpClient.GetAsync(
                    $"https://localhost:7000/api/Product/{item.ProductId}"
                );

                if (!response.IsSuccessStatusCode)
                    return BadRequest("Product not found");

                var product = await response.Content.ReadFromJsonAsync<dynamic>();

                int stock = product.stock;

                // ❌ OUT OF STOCK
                if (stock < item.Quantity)
                    return BadRequest($"Not enough stock for product");

                // ✅ REDUCE STOCK
                var updateData = new
                {
                    name = product.name,
                    price = product.price,
                    category = product.category,
                    stock = stock - item.Quantity
                };

                var form = new MultipartFormDataContent();
                form.Add(new StringContent(updateData.name), "name");
                form.Add(new StringContent(updateData.price.ToString()), "price");
                form.Add(new StringContent(updateData.category), "category");
                form.Add(new StringContent(updateData.stock.ToString()), "stock");

                await _httpClient.PutAsync(
                    $"https://localhost:7000/api/Product/{item.ProductId}",
                    form
                );
            }

            // ✅ TOTAL
            order.TotalAmount = order.Items.Sum(i => i.Price * i.Quantity);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // ✅ REAL-TIME NOTIFY
            await _hub.Clients
                .Group(userId)
                .SendAsync("ReceiveOrderUpdate", order);

            return Ok(order);
        }

        // ✅ GET USER ORDERS (FIXED TYPE)
        [HttpGet("user/{userId}")]
        public IActionResult GetOrders(string userId)
        {
            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .ToList();

            return Ok(orders);
        }
        [Authorize(Roles = "Admin")]
        // ✅ ADMIN - GET ALL
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _context.Orders.ToList();
            return Ok(orders);
        }

        // ✅ UPDATE STATUS (SAFE DTO)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            order.Status = dto.Status;

            await _context.SaveChangesAsync();
            
            // ✅ SIGNALR SEND            
            await _hub.Clients
    .Group(order.UserId.ToString())
    .SendAsync("ReceiveOrderUpdate", order);
            return Ok(order);
        }

        // ✅ GET SINGLE ORDER BY ID (ADD THIS)
        [HttpGet("details/{id}")]
        public IActionResult GetOrderById(Guid id)
        {
            var order = _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    o.CustomerName,
                    o.TotalAmount,
                    o.Status,
                    o.OrderDate,
                    Items = o.Items.Select(i => new
                    {
                        i.ProductId,
                        i.Quantity,
                        i.Price
                    })
                })
                .FirstOrDefault();

            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }

    // ✅ DTO CLASS (IMPORTANT)
    public class UpdateStatusDto
    {
        public string Status { get; set; }
    }

}