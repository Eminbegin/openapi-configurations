using Configurations.Application.Abstractions.Persistence.Queries;
using Configurations.Application.Model;

namespace Configurations.Application.Abstractions.Persistence.Repositories;

public interface IConfigurationRepository
{
    IAsyncEnumerable<ConfigurationEntry> QueryAsync(
        ConfigurationQuery query,
        CancellationToken cancellationToken);

    Task AddOrUpdateAsync(
        IReadOnlyCollection<ConfigurationEntry> entries,
        CancellationToken cancellationToken);
}
