namespace Mango.Services.PaymentAPI.Messaging;

using System.Text;
using System.Text.Json;
using Messages;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQSender;

public class RabbitMqPaymentConsumer : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    private readonly IRabbitMqPaymentMessageSender _rabbitMqPaymentMessageSender;
    private readonly IProcessPayment _processPayment;

    public RabbitMqPaymentConsumer(IRabbitMqPaymentMessageSender rabbitMqPaymentMessageSender, IProcessPayment processPayment)
    {
        _rabbitMqPaymentMessageSender = rabbitMqPaymentMessageSender;
        _processPayment = processPayment;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare("orderpaymentprocesstopic", false, false, false, null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            PaymentRequestMessage paymentRequestMessage = JsonSerializer.Deserialize<PaymentRequestMessage>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            HandlerMessage(paymentRequestMessage).GetAwaiter().GetResult();
            
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume("orderpaymentprocesstopic", false, consumer);

        return Task.CompletedTask;
    }

    private async Task HandlerMessage(PaymentRequestMessage paymentRequestMessage)
    {
        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage updatePaymentResultMessage = new()
        {
            Status = result,
            OrderId = paymentRequestMessage.OrderId,
            Email = paymentRequestMessage.Email
        };

        try
        {
            // await _messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
            // await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}