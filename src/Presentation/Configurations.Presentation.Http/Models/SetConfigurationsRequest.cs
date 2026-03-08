using Microsoft.Extensions.Options;

namespace Configurations.Presentation.Http.Models;

public sealed class SetConfigurationsRequest
{
    [ValidateEnumeratedItems]
    public required IReadOnlyCollection<ConfigurationEntry> Entries { get; init; }

    public class ConfigurationEntry
    {
        public required string Key { get; init; }

        public required string Value { get; init; }
    }
}
