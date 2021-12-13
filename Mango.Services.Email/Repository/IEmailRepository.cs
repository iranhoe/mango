namespace Mango.Services.Email.Repository;

public interface IOrderRepository
{
    Task<bool> AddOrder(OrderHeader orderHeader);
    Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);
}