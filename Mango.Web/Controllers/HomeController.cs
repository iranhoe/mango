using Mango.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    using System.Text.Json;
    using Microsoft.AspNetCore.Authorization;
    using Services.IServices;

    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            
            List<ProductDto>? list = new();
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");
            if (response is {IsSuccess: true})
            {
                list = JsonSerializer.Deserialize<List<ProductDto>>(Convert.ToString(response.Result) ?? string.Empty, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            } 
            
            return View(list);
        }
        
        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            
            ProductDto? model = new();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, "");
            if (response is {IsSuccess: true})
            {
                model = JsonSerializer.Deserialize<ProductDto>(Convert.ToString(response.Result) ?? string.Empty, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            } 
            
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}