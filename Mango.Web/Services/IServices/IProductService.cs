namespace Mango.Web.Services.IServices;

using Models;

public interface IProductService : IBaseService
{
    Task<T> GetAllProducts<T>();
    Task<T> GetProductsAsync<T>(int id);
    Task<T> CreateProductAsync<T>(ProductDto productDto);
    Task<T> UpdateProductAsync<T>(ProductDto productDto);
    Task<T> DeleteProductAsync<T>(int id);
}