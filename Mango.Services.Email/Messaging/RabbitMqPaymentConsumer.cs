namespace Mango.Services.Email.Messaging;

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
    private const string ExchangeName = "DirectPaymentUpdate_Exchange";
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
    private readonly EmailRepository _emailRepo;
    private string queueName = "";

    public RabbitMqPaymentConsumer(EmailRepository emailRepo)
    {
        _emailRepo = emailRepo;
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
        _channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, "PaymentEmail");
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
        _channel.BasicConsume(PaymentEmailUpdateQueueName, false, consumer);

        return Task.CompletedTask;
    }

    private async Task HandlerMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
    {
        try
        {
            await _emailRepo.SendAndLogEmail(updatePaymentResultMessage);
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