using Configurations.Application.Abstractions.Persistence;
using Configurations.Application.Abstractions.Persistence.Repositories;

namespace Configurations.Infrastructure.Persistence;

public sealed class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(IConfigurationRepository configurations)
    {
        Configurations = configurations;
    }

    public IConfigurationRepository Configurations { get; }
}
