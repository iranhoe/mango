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

    [HttpGet]
    [Route("{id}")]
    public async Task<ResponseDto> Get(int id)
    {
        try
        {
            var productDto = await _productRepository.GetProductById(id);
            _response.Result = productDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages
                = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpPost]
    public async Task<ResponseDto> Post([FromBody] ProductDto productDto)
    {
        try
        {
            ProductDto model = await _productRepository.CreateUpdateProduct(productDto);
            _response.Result = model;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages
                = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpPut]
    public async Task<ResponseDto> Put([FromBody] ProductDto productDto)
    {
        try
        {
            ProductDto model = await _productRepository.CreateUpdateProduct(productDto);
            _response.Result = model;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages
                = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ResponseDto> Delete(int id)
    {
        try
        {
            var isSuccess = await _productRepository.DeleteProduct(id);
            _response.Result = isSuccess;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages
                = new List<string> { ex.ToString() };
        }

        return _response;
    }
}