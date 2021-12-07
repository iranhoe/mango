namespace Mango.MessageBus;

using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

public class AzureServiceBusMessageBus : IMessageBus
{
    //can be improved
    private readonly string _connectionString = "";

    public AzureServiceBusMessageBus(IOptions<Config> options)
    {
        _connectionString = options.Value.ConnectionString;
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