namespace Mango.Services.ProductApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Repository;
using Repository.Dto;

[Route("api/products")]
public class ProductApiController : ControllerBase
{
    protected ResponseDto _response;
    private readonly IProductRepository _productRepository;

    public ProductApiController(IProductRepository productRepository)
    {
        this._response = new ResponseDto();
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ResponseDto> Get()
    {
        try
        {
            IEnumerable<ProductDto> productDtos = await _productRepository.GetProducts();
            _response.Result = productDtos;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages
                = new List<string> {ex.ToString()};
        }

        return _response;
    }
}