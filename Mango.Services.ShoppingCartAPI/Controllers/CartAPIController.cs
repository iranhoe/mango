namespace Mango.Services.ShoppingCartAPI.Controllers;

using MessageBus;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Repository;

[ApiController]
[Route("api/cart")]
public class CartAPIController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IMessageBus _messageBus;
    protected ResponseDto _response;

    public CartAPIController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository)
    {
        _cartRepository = cartRepository;
        _messageBus = messageBus;
        _couponRepository = couponRepository;
        this._response = new ResponseDto();
    }

    [HttpGet("GetCart/{userId}")]
    public async Task<object> GetCart(string userId)
    {
        try
        {
            CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
            _response.Result = cartDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() {ex.ToString()};
        }

        return _response;
    }
    
    [HttpPost("AddCart")]
    public async Task<object> AddCart(CartDto cartDto)
    {
        try
        {
            CartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
            _response.Result = cartDt;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() {ex.ToString()};
        }

        return _response;
    }
    
    [HttpPost("UpdateCart")]
    public async Task<object> UpdateCart(CartDto cartDto)
    {
        try
        {
            CartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
            _response.Result = cartDt;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() {ex.ToString()};
        }

        return _response;
    }
    
    [HttpPost("RemoveCart")]
    public async Task<object> RemoveCart([FromBody]int cartId)
    {
        try
        {
            bool isSuccess = await _cartRepository.RemoveFromCart(cartId);
            _response.Result = isSuccess;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() {ex.ToString()};
        }

        return _response;
    }
    
    [HttpPost("ApplyCoupon")]
    public async Task<object> ApplyCoupon([FromBody]CartDto cartDto)
    {
        try
        {
            bool isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId,
                cartDto.CartHeader.CouponCode);
            _response.Result = isSuccess;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() {ex.ToString()};
        }

        return _response;
    }
    
    [HttpPost("RemoveCoupon")]
    public async Task<object> RemoveCoupon([FromBody]string userId)
    {
        try
        {
            bool isSuccess = await _cartRepository.RemoveCoupon(userId);
            _response.Result = isSuccess;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() {ex.ToString()};
        }

        return _response;
    }
    
    [HttpPost("checkout")]
    public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
    {
        try
        {
            CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
            if (cartDto == null)
            {
                return BadRequest();
            }

            if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
            {
                CouponDto couponDto = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                if (checkoutHeader.DiscountTotal != couponDto.DiscountAmount)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() {"Coupon Price has changed, please confirm"};
                    _response.DisplayMessage = "Coupon Price has changed, please confirm";
                    return _response;
                }
            }

            checkoutHeader.CartDetails = cartDto.CartDetails;
            // logic to add message to process order
            await _messageBus.PublishMessage(checkoutHeader, "checkoutmessagetopic");
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() {ex.ToString()};
        }

        return _response;
    }
}