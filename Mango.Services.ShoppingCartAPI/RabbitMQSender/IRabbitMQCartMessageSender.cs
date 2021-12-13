namespace Mango.Services.ShoppingCartAPI.RabbitMQSender;

using MessageBus;

public interface IRabbitMQCartMessageSender
{
    void SendMessage(BaseMessage baseMessage, string queueName);
}