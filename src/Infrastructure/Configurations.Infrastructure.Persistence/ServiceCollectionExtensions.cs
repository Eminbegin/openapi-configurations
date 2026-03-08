using Configurations.Infrastructure.Persistence.Repositories;
using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Configurations.Application.Abstractions.Persistence;
using Configurations.Application.Abstractions.Persistence.Repositories;

namespace Configurations.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection collection)
    {
        collection.AddPlatformPersistence(persistence => persistence
            .UsePostgres(postgres => postgres
                .WithConnectionOptions(builder => builder.BindConfiguration(
                    "Infrastructure:Persistence:Postgres"))
                .WithMigrationsFrom(typeof(IAssemblyMarker).Assembly)));

        collection.AddScoped<IPersistenceContext, PersistenceContext>();

        collection.AddScoped<IConfigurationRepository, ConfigurationRepository>();

        return collection;
    }
}
