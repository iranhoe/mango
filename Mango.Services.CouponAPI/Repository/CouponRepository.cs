namespace Mango.Services.CouponAPI.Repository;

using AutoMapper;
using DbContexts;
using Models.Dto;

public class CouponRepository : ICouponRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CouponRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CouponDto> GetCouponByCode(string couponCode)
    {
        var couponFromDb = _db.Coupons.FirstOrDefault(u => u.CouponCode == couponCode);
        return _mapper.Map<CouponDto>(couponFromDb);
    }
}