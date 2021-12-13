namespace Mango.Services.ShoppingCartAPI.RabbitMQSender;

using MessageBus;

public class RabbitMQCartMessageSender : IRabbitMQCartMessageSender
{
    public void SendMessage(BaseMessage baseMessage, string queueName)
    {
        throw new NotImplementedException();
    }
}