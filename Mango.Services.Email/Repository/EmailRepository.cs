namespace Mango.Services.Email.Repository;

using DbContexts;
using Messages;
using Microsoft.EntityFrameworkCore;
using Models;

public class EmailRepository : IEmailRepository
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContext;

    public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
    {
        EmailLog emailLog = new EmailLog()
        {
            Email = message.Email,
            EmailSent = DateTime.Now,
            Log = $"Order - {message.OrderId} has been created successfully"
        };

        await using var _db = new ApplicationDbContext(_dbContext);
        _db.EmailLogs.Add(emailLog);
        await _db.SaveChangesAsync();
    }
}