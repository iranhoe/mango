namespace Mango.Services.ShoppingCartAPI.Repository;

using AutoMapper;
using DbContext;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CartRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CartDto> GetCartByUserId(string userId)
    {
        Cart cart = new Cart()
        {
            CartHeader = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId)
        };

        cart.CartDetails = _db.CartDetails
            .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId)
            .Include(u => u.Product);

        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
    {
        try
        {
            Cart cart = _mapper.Map<Cart>(cartDto);

            var prodInDb = _db.Products.FirstOrDefault(u => u.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);

            if (prodInDb == null)
            {
                _db.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _db.SaveChangesAsync();
            }

            var cartHeaderFromDb = await _db.CartHeader.AsNoTracking()
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
            else
            {
                var CartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                         u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (CartDetailsFromDb == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += CartDetailsFromDb.Count;
                    _db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
            }
            
            return _mapper.Map<CartDto>(cart);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> RemoveFromCart(int cartDetailsId)
    {
        try
        {
            var cartDetails = await _db.CartDetails
                .FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);

            var totalCountOfCartItems = _db.CartDetails
                .Count(u => u.CartHeaderId == cartDetails.CartHeaderId);

            _db.CartDetails.Remove(cartDetails);
            if (totalCountOfCartItems == 1)
            {
                var cartHeaderToRemove = await _db.CartHeader
                    .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetailsId);

                _db.CartHeader.Remove(cartHeaderToRemove);
            }

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> ClearCart(string userId)
    {
        var cartHeaderFromDb = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId);
        if (cartHeaderFromDb == null) return false;
        
        _db.CartDetails
            .RemoveRange(_db.CartDetails.Where(u => u.CartHeaderId == cartHeaderFromDb.CartHeaderId));
        _db.CartHeader.Remove(cartHeaderFromDb);
        await _db.SaveChangesAsync();
        return true;
    }
}