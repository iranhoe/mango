namespace Mango.Services.OrderAPI.Messaging;

using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Messages;
using Models;
using Repository;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string subscriptionCheckout;
    private readonly string checkoutMessageTopic;
    private readonly OrderRepository _orderRepository;
    private readonly IConfiguration _configuration;

    private ServiceBusProcessor checkOutProcessor;

    public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _configuration = configuration;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        subscriptionCheckout = _configuration.GetValue<string>("SubscriptionName");
        checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");

        var client = new ServiceBusClient(serviceBusConnectionString);
        checkOutProcessor = client.CreateProcessor(checkoutMessageTopic, subscriptionCheckout);
    }

    public async Task Start()
    {
        checkOutProcessor.ProcessMessageAsync += OnCeckOutMessageReceived;
        checkOutProcessor.ProcessErrorAsync += ErrorHandler;
        await checkOutProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await checkOutProcessor.StopProcessingAsync();
        await checkOutProcessor.DisposeAsync();
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
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