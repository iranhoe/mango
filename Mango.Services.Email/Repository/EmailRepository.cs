﻿namespace Mango.Services.Email.Repository;

using DbContexts;
using Microsoft.EntityFrameworkCore;

public class OrderRepository : IOrderRepository
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContext;

    public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddOrder(OrderHeader orderHeader)
    {
        await using var _db = new ApplicationDbContext(_dbContext);
        _db.OrderHeaders.Add(orderHeader);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
    {
        await using var _db = new ApplicationDbContext(_dbContext);
        var orderHeadersFromDb = await 
            _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == orderHeaderId);

        if (orderHeadersFromDb != null)
        {
            orderHeadersFromDb.PaymentStatus = paid; 
            await _db.SaveChangesAsync();
        }
    }
}