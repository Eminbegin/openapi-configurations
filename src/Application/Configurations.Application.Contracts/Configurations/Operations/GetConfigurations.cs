using Configurations.Application.Model;

namespace Configurations.Application.Contracts.Configurations.Operations;

public static class GetConfigurations
{
    public readonly record struct PageToken(string Key);

    public readonly record struct Request(
        int PageSize,
        PageToken? PageToken);

    public readonly record struct Response(
        IReadOnlyCollection<ConfigurationEntry> Entries,
        PageToken? PageToken);
}
