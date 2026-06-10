using Microsoft.Extensions.Logging;

namespace WholesalePlatform.WebApi.Logging;

public sealed class ColoredConsoleLoggerOptions
{
    private readonly Dictionary<string, LogLevel> _categoryMinimumLevels = new(StringComparer.Ordinal);

    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

    public IReadOnlyDictionary<string, LogLevel> CategoryMinimumLevels => _categoryMinimumLevels;

    public void SetCategoryMinimumLevel(string categoryPrefix, LogLevel minimumLevel)
    {
        if (string.IsNullOrWhiteSpace(categoryPrefix))
        {
            throw new ArgumentException("Category prefix cannot be empty.", nameof(categoryPrefix));
        }

        _categoryMinimumLevels[categoryPrefix] = minimumLevel;
    }
}
