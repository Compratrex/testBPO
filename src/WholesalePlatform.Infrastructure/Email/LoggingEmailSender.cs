using Microsoft.Extensions.Logging;
using WholesalePlatform.Application.Abstractions.Email;

namespace WholesalePlatform.Infrastructure.Email;

public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "\u0422\u0415\u0421\u0422 email stub. To: {To}; Subject: {Subject}; Body: {Body}",
            to,
            subject,
            body);
        return Task.CompletedTask;
    }
}
