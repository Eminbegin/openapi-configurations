using Configurations.Application.Configurations;
using Configurations.Application.Contracts.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Configurations.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IConfigurationService, ConfigurationService>();

        return collection;
    }
}
