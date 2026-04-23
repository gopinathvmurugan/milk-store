using Microsoft.AspNetCore.Mvc;
using CartService.Data;
using CartService.Models;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartDbContext _context;

        public CartController(CartDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(CartItem item)
        {
            item.Id = Guid.NewGuid();

            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        [HttpGet("{userId}")]
        public IActionResult GetCart(string userId)
        {
            var cart = _context.CartItems.Where(x => x.UserId == userId).ToList();
            return Ok(cart);
        }
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            var items = _context.CartItems.Where(x => x.UserId == userId);

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}