namespace Mango.Services.ShoppingCartAPI;

using AutoMapper;
using Models;
using Models.Dto;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<ProductDto, Product>().ReverseMap();
            config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
            config.CreateMap<CartDto, Cart>().ReverseMap();
        });

        return mappingConfig;
    }
}