namespace Mango.Services.ShoppingCartAPI.Repository;

using Models.Dto;

public interface ICouponRepository
{
    Task<CouponDto> GetCoupon(string couponName);
}