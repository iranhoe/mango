namespace Mango.Services.OrderAPI.Messages;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}