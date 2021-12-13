namespace Mango.Services.OrderAPI.Messaging;

using System.Text;
using System.Text.Json;
using Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repository;

public class RabbitMQConsumer : BackgroundService
{
    private readonly OrderRepository _orderRepository;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQConsumer(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare("checkoutqueue", false, false, false, null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            CheckoutHeaderDto checkoutHeaderDto = JsonSerializer.Deserialize<CheckoutHeaderDto>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            HandlerMessage(checkoutHeaderDto).GetAwaiter().GetResult();
            
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume("checkoutqueue", false, consumer);

        return Task.CompletedTask;
    }

    private async Task HandlerMessage(CheckoutHeaderDto? checkoutHeaderDto)
    {
        
    }
}