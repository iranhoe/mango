namespace Mango.Services.CouponAPI;

using AutoMapper;
using Models;
using Models.Dto;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<CouponDto, Coupon>().ReverseMap();
            // config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            // config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
            // config.CreateMap<CartDto, Cart>().ReverseMap();
        });

        return mappingConfig;
    }
}