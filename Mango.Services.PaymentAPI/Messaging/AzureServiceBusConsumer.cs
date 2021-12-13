namespace Mango.Services.PaymentAPI.Messaging;

using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using MessageBus;
using Messages;
using OrderAPI.Messaging;
using PaymentProcessor;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string subscriptionPayment;
    private readonly string orderPaymentProcessTopic;
    private readonly string orderUpdatePaymentResultTopic;
    
    private ServiceBusProcessor orderPaymentProcessor;
    private readonly IProcessPayment _processPayment;

    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;


    public AzureServiceBusConsumer(IConfiguration configuration, IMessageBus messageBus, IProcessPayment processPayment)
    {
        _configuration = configuration;
        _messageBus = messageBus;
        _processPayment = processPayment;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        subscriptionPayment = _configuration.GetValue<string>("OrderPaymentProcessSubscription");
        
        orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
        orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

        var client = new ServiceBusClient(serviceBusConnectionString);
        orderPaymentProcessor = client.CreateProcessor(orderPaymentProcessTopic, subscriptionPayment);
    }

    public async Task Start()
    {
        orderPaymentProcessor.ProcessMessageAsync += ProcessPayment;
        orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
        await orderPaymentProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await orderPaymentProcessor.StopProcessingAsync();
        await orderPaymentProcessor.DisposeAsync();
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task ProcessPayment(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        PaymentRequestMessage paymentRequestMessage = JsonSerializer.Deserialize<PaymentRequestMessage>(body,
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage updatePaymentResultMessage = new()
        {
            Status = result,
            OrderId = paymentRequestMessage.OrderId,
            Email = paymentRequestMessage.Email
        };

        try
        {
            await _messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}