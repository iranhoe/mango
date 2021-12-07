namespace Mango.Web.Services.IServices;

using Models;

public interface ICartService
{
    Task<T> GetCartByUserIdAsync<T>(string userId, string token = null);
    Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null);
    Task<T> UpdatreCartAsync<T>(CartDto cartDto, string token = null);
    Task<T> RemoveFromCartAsync<T>(int cartId, string token = null);
    Task<T> ApplyCouponAsync<T>(CartDto cartDto, string token = null);
    Task<T> RemoveCouponAsync<T>(string userId, string token = null);
    
}