namespace Mango.Services.OrderAPI.RabbitMQSender;

using MessageBus;

public interface IRabbitMqOrderMessageSender
{
    void SendMessage(BaseMessage baseMessage, string queueName);
}