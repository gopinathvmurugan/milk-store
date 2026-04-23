namespace ProductService.DTOs
{
    public class UpdateProductDTO
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public IFormFile Image { get; set; }
        public int Stock { get; set; }
    }
}
