namespace WholesalePlatform.Infrastructure.Outbox;

public sealed record EmailOutboxPayload(
    string To,
    string Subject,
    string Body);
