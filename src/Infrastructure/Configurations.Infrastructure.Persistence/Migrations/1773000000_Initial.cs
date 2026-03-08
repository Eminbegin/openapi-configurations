using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Configurations.Infrastructure.Persistence.Migrations;

[Migration(1773000000, "Initial")]
public sealed class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
    """
    create table configurations
    (
        configuration_key text not null primary key ,
        configuration_value text not null ,
        configuration_updated_at timestamp with time zone not null 
    );
    """;
    

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
    """
    drop table configurations;
    """;
}
