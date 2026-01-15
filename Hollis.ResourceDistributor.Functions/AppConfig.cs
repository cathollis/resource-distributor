namespace Hollis.ResourceDistributor.Functions;

public class AppConfig
{
    public required string DefaultUserAgent { get; init; }

    public required IList<string> CopiedRequestHeaders { get; init; } = [];

    public required IList<string> CopiedResponseHeaders { get; init; } = [];
}
