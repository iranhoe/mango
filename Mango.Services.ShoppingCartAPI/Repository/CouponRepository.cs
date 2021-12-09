namespace Mango.Services.ShoppingCartAPI.Repository;

using System.Text.Json;
using Models.Dto;

public class CouponRepository : ICouponRepository
{
    private readonly HttpClient client;

    public CouponRepository(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CouponDto> GetCoupon(string couponName)
    {
        var options = new JsonSerializerOptions() {PropertyNameCaseInsensitive = true};
        var response = await client.GetAsync($"api/coupon/{couponName}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonSerializer.Deserialize<ResponseDto>(apiContent, options);

        if (resp.IsSuccess)
        {
            return JsonSerializer.Deserialize<CouponDto>(Convert.ToString(resp.Result), options);
        }

        return new CouponDto();
    }
}