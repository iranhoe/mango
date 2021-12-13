﻿namespace Mango.Services.Email.Messaging;

using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Messages;
using Repository;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string subscriptionEmail;
    private readonly string orderUpdatePaymentResultTopic;
    private readonly EmailRepository _emailRepo;

    private ServiceBusProcessor orderUpdatePaymentStatusProcessor;

    private readonly IConfiguration _configuration;

    public AzureServiceBusConsumer(EmailRepository emailRepo, IConfiguration configuration)
    {
        _emailRepo = emailRepo;
        _configuration = configuration;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        subscriptionEmail = _configuration.GetValue<string>("SubscriptionName");
        orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

        var client = new ServiceBusClient(serviceBusConnectionString);
        
        orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionEmail);
    }

    public async Task Start()
    {
        orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
        await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
        await orderUpdatePaymentStatusProcessor.DisposeAsync();
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        UpdatePaymentResultMessage objMessage 
            = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(body,
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

        try
        {
            await _emailRepo.SendAndLogEmail(objMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}