namespace Mango.Web.Controllers;

using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.IServices;

public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;

    public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
    {
        _productService = productService;
        _cartService = cartService;
        _couponService = couponService;
    }

    public async Task<ActionResult> CartIndex()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }
    
    [HttpPost]
    [ActionName("ApplyCoupon")]
    public async Task<ActionResult> ApplyCoupon(CartDto cartDto)
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        
        var response = await _cartService.ApplyCouponAsync<ResponseDto>(cartDto, accessToken);

        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }
    
    [HttpPost]
    [ActionName("RemoveCoupon")]
    public async Task<ActionResult> RemoveCoupon(CartDto cartDto)
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        
        var response = await _cartService.RemoveCouponAsync<ResponseDto>(userId);

        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }
    
    public async Task<ActionResult> Remove(int cartDetailsId)
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailsId, accessToken);

        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

        CartDto cartDto = new();
        if (response != null && response.IsSuccess)
        {
            cartDto = JsonSerializer.Deserialize<CartDto>(Convert.ToString(response.Result) ?? string.Empty, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        if (cartDto.CartHeader != null)
        {
            if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
            {
                var coupon =
                    await _couponService.GetCouponAsync<ResponseDto>(cartDto.CartHeader.CouponCode, accessToken);
                if (coupon != null && coupon.IsSuccess)
                {
                    var couponObj = JsonSerializer.Deserialize<CouponDto>(Convert.ToString(coupon.Result) ?? string.Empty, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    cartDto.CartHeader.DiscountTotal = couponObj.DiscountAmount;
                }
            }
            
            foreach (var detail in cartDto.CartDetails)
            {
                cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
            }

            cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
        }

        return cartDto;
    }
}