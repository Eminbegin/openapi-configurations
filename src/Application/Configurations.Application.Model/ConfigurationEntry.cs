namespace Configurations.Application.Model;

public sealed record ConfigurationEntry(
    string Key,
    string Value,
    DateTimeOffset UpdatedAt);
