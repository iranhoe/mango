namespace Mango.Web.Services;

using IServices;
using Models;

public class CouponService : BaseService, ICouponService
{

    private readonly IHttpClientFactory _clientFactory;

    public CouponService(IHttpClientFactory clientFactory) : base(clientFactory) 
    {
        _clientFactory = clientFactory;
    }

    public async Task<T> GetCouponAsync<T>(string couponCode, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Url = SD.CouponAPIBase + "api/coupon/" + couponCode,
            AccessToken = token
        });
    }
}