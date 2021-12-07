namespace Mango.Web.Services.IServices;

using Models;

public interface ICouponService
{
    Task<T> GetCouponAsync<T>(string couponCode, string token = null);
}