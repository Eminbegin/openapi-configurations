using Configurations.Application.Contracts.Configurations.Operations;

namespace Configurations.Application.Contracts.Configurations;

public interface IConfigurationService
{
    Task SetConfigurationsAsync(
        SetConfigurations.Request request,
        CancellationToken cancellationToken);

    Task<GetConfigurations.Response> GetConfigurationAsync(
        GetConfigurations.Request request,
        CancellationToken cancellationToken);
}
