using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WholesalePlatform.Application.Abstractions.Clock;
using WholesalePlatform.Application.Abstractions.Email;
using WholesalePlatform.Infrastructure.Persistence;

namespace WholesalePlatform.Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private const int BatchSize = 20;
    private const int MaxRetryCount = 5;
    private const int LockMinutes = 2;

    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly string _workerId = $"{Environment.MachineName}-{Guid.NewGuid():N}";

    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(PollingInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Outbox batch processing failed.");
            }

            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var messages = await ClaimBatchAsync(context, dateTimeProvider, cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await ProcessMessageAsync(message, emailSender, cancellationToken);
                message.MarkProcessed(dateTimeProvider.UtcNow);
            }
            catch (Exception exception)
            {
                message.MarkFailed(exception.Message);
                _logger.LogWarning(
                    exception,
                    "Outbox message {OutboxMessageId} processing failed.",
                    message.Id);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<IReadOnlyList<OutboxMessage>> ClaimBatchAsync(
        AppDbContext context,
        IDateTimeProvider dateTimeProvider,
        CancellationToken cancellationToken)
    {
        var lockedAt = dateTimeProvider.UtcNow;
        var lockExpiresAt = lockedAt.AddMinutes(LockMinutes);

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var messages = await context.OutboxMessages
            .FromSqlInterpolated($"""
                SELECT *
                FROM outbox_messages
                WHERE "ProcessedAt" IS NULL
                  AND "RetryCount" < {MaxRetryCount}
                  AND "IsDeleted" = false
                  AND ("LockExpiresAt" IS NULL OR "LockExpiresAt" < {lockedAt})
                ORDER BY "OccurredAt"
                LIMIT {BatchSize}
                FOR UPDATE SKIP LOCKED
                """)
            .IgnoreQueryFilters()
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            message.MarkLocked(_workerId, lockedAt, lockExpiresAt);
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return messages;
    }

    private static Task ProcessMessageAsync(
        OutboxMessage message,
        IEmailSender emailSender,
        CancellationToken cancellationToken)
    {
        if (message.Type != OutboxMessage.SendWelcomeEmailType)
        {
            throw new InvalidOperationException($"Unsupported outbox message type '{message.Type}'.");
        }

        var payload = message.GetEmailPayload();
        return emailSender.SendAsync(payload.To, payload.Subject, payload.Body, cancellationToken);
    }
}
