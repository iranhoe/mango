namespace Mango.Services.ProductApi;

using AutoMapper;
using Models;
using Repository.Dto;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<ProductDto, ProductDto>();
            config.CreateMap<Product, ProductDto>();
        });

        return mappingConfig;
    }
}