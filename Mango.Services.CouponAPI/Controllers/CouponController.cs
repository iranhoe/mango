namespace Mango.Services.CouponAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Repository;

[ApiController]
[Route("api/[controller]")]
public class CouponController : ControllerBase
{
    private readonly ICouponRepository _couponRepository;
    protected ResponseDto _response;

    public CouponController(ICouponRepository couponRepository, ResponseDto response)
    {
        _couponRepository = couponRepository;
        _response = new ResponseDto();
    }
    
    [HttpGet("{code}")]
    public async Task<object> GetDiscountForCode(string code)
    {
        try
        {
            var coupon = await _couponRepository.GetCouponByCode(code);
            _response.Result = coupon;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() {ex.ToString()};
        }

        return _response;
    }
}