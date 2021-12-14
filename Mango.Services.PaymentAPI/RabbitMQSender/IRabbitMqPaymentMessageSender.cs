namespace Mango.Services.PaymentAPI.RabbitMQSender;

using MessageBus;

public interface IRabbitMqPaymentMessageSender
{
    void SendMessage(BaseMessage baseMessage);
}