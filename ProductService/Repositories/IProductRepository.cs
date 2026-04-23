using ProductService.Models;
using ProductService.DTOs;

namespace ProductService.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();

        Task<Product> GetProductById(Guid id);

        Task<Product> AddProduct(Product product);
        Task UpdateProduct(Product product);

        Task DeleteProduct(Guid id);
    }
}