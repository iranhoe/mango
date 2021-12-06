namespace Mango.Services.CouponAPI.Repository;

using Microsoft.EntityFrameworkCore;
using Models.Dto;

public interface ICouponRepository
{
    Task<CouponDto> GetCouponByCode(string couponCode);
}