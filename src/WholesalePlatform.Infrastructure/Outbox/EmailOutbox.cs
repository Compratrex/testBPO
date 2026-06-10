using WholesalePlatform.Application.Abstractions.Clock;
using WholesalePlatform.Application.Abstractions.Email;
using WholesalePlatform.Infrastructure.Persistence;

namespace WholesalePlatform.Infrastructure.Outbox;

public sealed class EmailOutbox : IEmailOutbox
{
    private readonly AppDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EmailOutbox(AppDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task EnqueueAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        return _context.OutboxMessages
            .AddAsync(
                OutboxMessage.CreateWelcomeEmail(to, subject, body, _dateTimeProvider.UtcNow),
                cancellationToken)
            .AsTask();
    }
}
