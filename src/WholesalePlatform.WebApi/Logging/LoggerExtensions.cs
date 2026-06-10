using Microsoft.Extensions.Logging;

namespace WholesalePlatform.WebApi.Logging;

public static class LoggerExtensions
{
    public static ILoggingBuilder AddColoredConsoleLogger(
        this ILoggingBuilder logging,
        LogLevel minimumLevel = LogLevel.Information,
        Action<ColoredConsoleLoggerOptions>? configure = null)
    {
        var options = new ColoredConsoleLoggerOptions
        {
            MinimumLevel = minimumLevel
        };
        configure?.Invoke(options);

        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Trace);
        logging.AddProvider(new ColoredConsoleLoggerProvider(options));

        return logging;
    }

    public static void LogTestsPassed(this ILogger logger, string message)
    {
        logger.LogInformation(CustomLogEvents.TestsPassed, "{Message}", message);
    }
}
