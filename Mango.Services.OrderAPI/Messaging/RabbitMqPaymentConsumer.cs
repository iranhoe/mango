namespace Mango.Services.OrderAPI.Messaging;

using System.Text;
using System.Text.Json;
using Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repository;

public class RabbitMqPaymentConsumer : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";
    private readonly OrderRepository _orderRepository;
    private string queueName = "";

    public RabbitMqPaymentConsumer(OrderRepository orderRepository)
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
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
        queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName, ExchangeName, "");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            UpdatePaymentResultMessage updatePaymentResultMessage = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            HandlerMessage(updatePaymentResultMessage).GetAwaiter().GetResult();
            
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume(queueName, false, consumer);

        return Task.CompletedTask;
    }

    private async Task HandlerMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
    {
        try
        {
            await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId,
                updatePaymentResultMessage.Status);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}