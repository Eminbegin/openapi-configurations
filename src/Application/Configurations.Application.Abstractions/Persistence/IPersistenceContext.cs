using Configurations.Application.Abstractions.Persistence.Repositories;

namespace Configurations.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IConfigurationRepository Configurations { get; }
}
