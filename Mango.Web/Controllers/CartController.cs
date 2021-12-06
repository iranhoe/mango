namespace Mango.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services.IServices;

public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    public CartController(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        
    }
}