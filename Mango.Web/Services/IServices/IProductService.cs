namespace Mango.Web.Services.IServices;

using Models;

public interface IProductService : IBaseService
{
    Task<T> GetAllProductsAsync<T>();
    Task<T> GetProductsByIdAsync<T>(int id);
    Task<T> CreateProductAsync<T>(ProductDto productDto);
    Task<T> UpdateProductAsync<T>(ProductDto productDto);
    Task<T> DeleteProductAsync<T>(int id);
}