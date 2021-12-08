namespace Mango.Services.OrderAPI.Messaging;

using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Messages;
using Models;
using Repository;

public class AzureServiceBusConsumer
{
    private readonly OrderRepository _orderRepository;

    public AzureServiceBusConsumer(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    private async Task OnCeckOutMessageReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        CheckoutHeaderDto checkoutHeaderDto = JsonSerializer.Deserialize<CheckoutHeaderDto>(body,
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

        OrderHeader orderHeader = new()
        {
            UserId = checkoutHeaderDto.UserId,
            FirstName = checkoutHeaderDto.FirstName,
            LastName = checkoutHeaderDto.LastName,
            OrderDetails = new List<OrderDetails>(),
            CardNumber = checkoutHeaderDto.CardNumber,
            CouponCode = checkoutHeaderDto.CouponCode,
            CVV = checkoutHeaderDto.CVV,
            DiscountTotal = checkoutHeaderDto.DiscountTotal,
            Email = checkoutHeaderDto.Email,
            ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
            OrderTime = DateTime.Now,
            OrderTotal = checkoutHeaderDto.OrderTotal,
            PaymentStatus = false,
            Phone = checkoutHeaderDto.Phone,
            PickupDateTime = checkoutHeaderDto.PickupDateTime
        };

        foreach (var detailsList in checkoutHeaderDto.CartDetails)
        {
            OrderDetails orderDetails = new()
            {
                ProductId = detailsList.ProductId,
                ProductName = detailsList.Product.Name,
                Price = detailsList.Product.Price,
                Count = detailsList.Count
            };
            orderHeader.CartTotalItems += detailsList.Count;
            orderHeader.OrderDetails.Add(orderDetails);
        }

        await _orderRepository.AddOrder(orderHeader);
    }
}