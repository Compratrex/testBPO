using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace WholesalePlatform.WebApi.Logging;

public sealed class ColoredConsoleLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ColoredConsoleLogger> _loggers = new();
    private readonly ColoredConsoleLoggerOptions _options;

    public ColoredConsoleLoggerProvider(ColoredConsoleLoggerOptions options)
    {
        _options = options;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, category => new ColoredConsoleLogger(category, _options));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}
