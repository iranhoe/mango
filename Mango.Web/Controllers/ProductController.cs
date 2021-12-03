namespace Mango.Web.Controllers;

using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.IServices;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> ProductIndex()
    {
        List<ProductDto>? list = new();
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _productService.GetAllProductsAsync<ResponseDto>(accessToken);
        if (response is {IsSuccess: true})
        {
            list = JsonSerializer.Deserialize<List<ProductDto>>(Convert.ToString(response.Result) ?? string.Empty, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        return View(list);
    }

    public async Task<IActionResult> ProductCreate()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductCreate(ProductDto model)
    {
        if (ModelState.IsValid)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.CreateProductAsync<ResponseDto>(model, accessToken);
            if (response is {IsSuccess: true})
            {
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        return View(model);
    }

    public async Task<IActionResult> ProductEdit(int productId)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
        if (response is not { IsSuccess: true })
        {
            return NotFound();
        }

        var model = JsonSerializer.Deserialize<ProductDto>(Convert.ToString(response.Result) ?? string.Empty,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return View(model);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductEdit(ProductDto model)
    {
        if (ModelState.IsValid)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.UpdateProductAsync<ResponseDto>(model, accessToken);
            if (response is {IsSuccess: true})
            {
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ProductDelete(int productId)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
        if (response is not { IsSuccess: true })
        {
            return NotFound();
        }

        var model = JsonSerializer.Deserialize<ProductDto>(Convert.ToString(response.Result) ?? string.Empty,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return View(model);

    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductDelete(ProductDto model)
    {
        if (ModelState.IsValid)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.DeleteProductAsync<ResponseDto>(model.ProductId, accessToken);
            if (response is {IsSuccess: true})
            {
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        return View(model);
    }

}