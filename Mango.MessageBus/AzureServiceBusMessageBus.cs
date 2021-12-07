namespace Mango.MessageBus;

using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;

public class AzureServiceBusMessageBus : IMessageBus
{
    //can be improved
    private string connectionString = "";
    public async Task PublishMessage(BaseMessage message, string topicName)
    {
        // ISenderClient senderClient = new TopicClient(connectionString, topicName);
        await using var client = new ServiceBusClient(connectionString);
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