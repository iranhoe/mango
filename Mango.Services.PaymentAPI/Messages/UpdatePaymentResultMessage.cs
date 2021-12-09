namespace Mango.Services.PaymentAPI.Messages;

using MessageBus;

public class UpdatePaymentResultMessage : BaseMessage
{
    public int OrderId { get; set; }
    public bool Status { get; set; }
}