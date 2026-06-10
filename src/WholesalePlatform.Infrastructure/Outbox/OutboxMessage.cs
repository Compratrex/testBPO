using System.Text.Json;
using WholesalePlatform.Domain.Common;

namespace WholesalePlatform.Infrastructure.Outbox;

public sealed class OutboxMessage : AuditableEntity
{
    public const string SendWelcomeEmailType = "SendWelcomeEmail";

    private OutboxMessage()
    {
    }

    private OutboxMessage(
        Guid id,
        string type,
        string payload,
        DateTimeOffset occurredAt)
        : base(id)
    {
        Type = type;
        Payload = payload;
        OccurredAt = occurredAt;
    }

    public string Type { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }
    public DateTimeOffset? LockedAt { get; private set; }
    public DateTimeOffset? LockExpiresAt { get; private set; }
    public string? LockedBy { get; private set; }

    public static OutboxMessage CreateWelcomeEmail(
        string to,
        string subject,
        string body,
        DateTimeOffset occurredAt)
    {
        var payload = JsonSerializer.Serialize(new EmailOutboxPayload(to, subject, body));
        return new OutboxMessage(Guid.NewGuid(), SendWelcomeEmailType, payload, occurredAt);
    }

    public EmailOutboxPayload GetEmailPayload()
    {
        return JsonSerializer.Deserialize<EmailOutboxPayload>(Payload)
            ?? throw new InvalidOperationException("Outbox email payload is invalid.");
    }

    public void MarkProcessed(DateTimeOffset processedAt)
    {
        ProcessedAt = processedAt;
        Error = null;
        Payload = "{}";
        ClearLock();
    }

    public void MarkFailed(string error)
    {
        RetryCount++;
        Error = error;
        ClearLock();
    }

    public void MarkLocked(string lockedBy, DateTimeOffset lockedAt, DateTimeOffset lockExpiresAt)
    {
        if (string.IsNullOrWhiteSpace(lockedBy))
        {
            throw new DomainException("Outbox lock owner cannot be empty.");
        }

        LockedBy = lockedBy;
        LockedAt = lockedAt;
        LockExpiresAt = lockExpiresAt;
    }

    private void ClearLock()
    {
        LockedAt = null;
        LockExpiresAt = null;
        LockedBy = null;
    }
}
