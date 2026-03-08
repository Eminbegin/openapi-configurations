namespace Configurations.Presentation.Http.Models;

public sealed class GetConfigurationsResponse
{
    public required IEnumerable<ConfigurationEntry> Entries { get; init; }

    public required string? PageToken { get; init; }

    public class ConfigurationEntry
    {
        public required string Key { get; init; }

        public required string Value { get; init; }
    }
}
