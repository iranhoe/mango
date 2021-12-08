namespace Mango.Services.OrderAPI.Messaging;

using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Messages;

public class AzureServiceBusConsumer
{
    private async Task OnCeckOutMessageReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        CheckoutHeaderDto checkoutHeaderDto = JsonSerializer.Deserialize<CheckoutHeaderDto>(body,
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
    }
}