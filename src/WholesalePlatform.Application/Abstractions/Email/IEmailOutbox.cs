namespace WholesalePlatform.Application.Abstractions.Email;

public interface IEmailOutbox
{
    Task EnqueueAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken);
}
