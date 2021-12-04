namespace Mango.Services.ShoppingCartAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Repository;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    protected ResponseDto _response;

    public CartController(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
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
    
    [HttpPost("AddCart/{userId}")]
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
    
    [HttpPost("UpdateCart/{userId}")]
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
    
    [HttpPost("RemoveCart/{userId}")]
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
}