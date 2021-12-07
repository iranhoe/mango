namespace Mango.MessageBus;

using System.Text;
using System.Text.Json;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

public class AzureServiceBusMessageBus : IMessageBus
{
    //can be improved
    private string connectionString = "";
    public async Task PublishMessage(BaseMessage message, string topicName)
    {
        ISenderClient senderClient = new TopicClient(connectionString, topicName);

        var jsonMessage = JsonSerializer.Serialize(message, message.GetType());
        var finalMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await senderClient.SendAsync(finalMessage);

        await senderClient.CloseAsync();
    }
}