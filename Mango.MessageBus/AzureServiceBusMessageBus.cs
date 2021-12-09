namespace Mango.MessageBus;

using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

public class AzureServiceBusMessageBus : IMessageBus
{
    private readonly IConfiguration _configuration;

    //can be improved
    private readonly string _connectionString = "";

    public AzureServiceBusMessageBus(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetValue<string>("ServiceBusConnectionString");;
    }
    public async Task PublishMessage(BaseMessage message, string topicName)
    {
        await using var client = new ServiceBusClient(_connectionString);
        ServiceBusSender sender = client.CreateSender(topicName);

        var jsonMessage = JsonSerializer.Serialize(message, message.GetType());
        ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(finalMessage);

        await client.DisposeAsync();
    }
}