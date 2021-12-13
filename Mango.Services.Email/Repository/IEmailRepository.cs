namespace Mango.Services.Email.Repository;

using Messages;

public interface IEmailRepository
{
    Task SendAndLogEmail(UpdatePaymentResultMessage message);
}