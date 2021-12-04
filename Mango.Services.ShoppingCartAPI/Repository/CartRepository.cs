﻿namespace Mango.Services.ShoppingCartAPI.Repository;

using AutoMapper;
using DbContext;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    
    public async Task<CartDto> GetCartByUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
    {
        Cart cart = _mapper.Map<Cart>(cartDto);

        var prodInDb = _db.Products.FirstOrDefault(u => u.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);

        if (prodInDb == null)
        {
            _db.Products.Add(cart.CartDetails.FirstOrDefault().Product);
            await _db.SaveChangesAsync();
        }

        var cartHeaderFromDb = _db.CartHeader
            .FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);

        if (cartHeaderFromDb == null)
        {
            _db.CartHeader.Add(cart.CartHeader);
            await _db.SaveChangesAsync();
            cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
            cart.CartDetails.FirstOrDefault().Product = null;
            _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> RemoveFromCart(int cartDetailsId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ClearCart(string userId)
    {
        throw new NotImplementedException();
    }
}