using Microsoft.Extensions.Logging;

namespace WholesalePlatform.WebApi.Logging;

public sealed class ColoredConsoleLogger : ILogger
{
    private static readonly object SyncRoot = new();
    private readonly string _categoryName;
    private readonly ColoredConsoleLoggerOptions _options;

    public ColoredConsoleLogger(string categoryName, ColoredConsoleLoggerOptions options)
    {
        _categoryName = categoryName;
        _options = options;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return NullScope.Instance;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None && logLevel >= ResolveMinimumLevel();
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        if (string.IsNullOrWhiteSpace(message) && exception is null)
        {
            return;
        }

        var now = DateTimeOffset.Now;
        var logType = ResolveType(logLevel, eventId, message);
        var color = ResolveColor(logType, logLevel);
        var from = ShortenCategory(_categoryName);
        var renderedMessage = exception is null
            ? message
            : $"{message} {exception.GetType().Name}: {exception.Message}";

        lock (SyncRoot)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(
                "[{0}][{1:yyyy-MM-dd}][{1:HH:mm:ss.fff}][{2}] {3};",
                logType,
                now,
                from,
                renderedMessage);
            Console.ForegroundColor = previousColor;
        }
    }

    private static string ResolveType(LogLevel logLevel, EventId eventId, string message)
    {
        if (eventId.Name == CustomLogEvents.TestsPassed.Name
            || message.Contains("tests passed", StringComparison.OrdinalIgnoreCase)
            || message.Contains("тесты пройдены", StringComparison.OrdinalIgnoreCase)
            || message.Contains("пройден", StringComparison.OrdinalIgnoreCase))
        {
            return "TEST";
        }

        return logLevel switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "FATAL",
            _ => "LOG"
        };
    }

    private static ConsoleColor ResolveColor(string logType, LogLevel logLevel)
    {
        if (logType == "TEST")
        {
            return ConsoleColor.Green;
        }

        return logLevel switch
        {
            LogLevel.Information => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.DarkRed,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Trace => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };
    }

    private LogLevel ResolveMinimumLevel()
    {
        var minimumLevel = _options.MinimumLevel;
        var longestMatchedPrefixLength = -1;

        foreach (var (categoryPrefix, categoryMinimumLevel) in _options.CategoryMinimumLevels)
        {
            if (!_categoryName.StartsWith(categoryPrefix, StringComparison.Ordinal)
                || categoryPrefix.Length <= longestMatchedPrefixLength)
            {
                continue;
            }

            minimumLevel = categoryMinimumLevel;
            longestMatchedPrefixLength = categoryPrefix.Length;
        }

        return minimumLevel;
    }

    private static string ShortenCategory(string categoryName)
    {
        var lastDot = categoryName.LastIndexOf('.');
        return lastDot >= 0 ? categoryName[(lastDot + 1)..] : categoryName;
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
