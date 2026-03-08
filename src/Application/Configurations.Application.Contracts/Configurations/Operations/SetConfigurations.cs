using Configurations.Application.Model;

namespace Configurations.Application.Contracts.Configurations.Operations;

public static class SetConfigurations
{
    public readonly record struct Request(IReadOnlyCollection<ConfigurationEntry> Entries);
}
