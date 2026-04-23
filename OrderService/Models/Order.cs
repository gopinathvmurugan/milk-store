using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OrderService.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        [BindNever]
        public string? UserId { get; set; }   // ✅ STRING

        public string? CustomerName { get; set; }   // ✅ ADD

        public decimal TotalAmount { get; set; }   // ✅ ADD

        public DateTime OrderDate { get; set; }

        public string? Status { get; set; } = "Pending"; // ✅ VERY IMPORTANT

        public List<OrderItem> Items { get; set; }
    }
}