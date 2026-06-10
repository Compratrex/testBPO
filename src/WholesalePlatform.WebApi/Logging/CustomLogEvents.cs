using Microsoft.Extensions.Logging;

namespace WholesalePlatform.WebApi.Logging;

public static class CustomLogEvents
{
    public static readonly EventId TestsPassed = new(10_001, nameof(TestsPassed));
}
