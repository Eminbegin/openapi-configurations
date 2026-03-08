using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using Configurations.Application.Abstractions.Persistence.Queries;
using Configurations.Application.Abstractions.Persistence.Repositories;
using Configurations.Application.Model;

namespace Configurations.Infrastructure.Persistence.Repositories;

internal sealed class ConfigurationRepository : IConfigurationRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public ConfigurationRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<ConfigurationEntry> QueryAsync(
        ConfigurationQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT configuration_key,
               configuration_value, 
               configuration_updated_at
        FROM configurations
        WHERE (:key_cursor IS NULL or configuration_key > :key_cursor)
        ORDER BY configuration_key
        LIMIT :page_size;
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("key_cursor", query.KeyCursor ?? string.Empty)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new ConfigurationEntry(
                reader.GetString("configuration_key"),
                reader.GetString("configuration_value"),
                reader.GetFieldValue<DateTimeOffset>("configuration_updated_at"));
        }
    }

    public async Task AddOrUpdateAsync(
        IReadOnlyCollection<ConfigurationEntry> entries,
        CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO configurations (configuration_key, configuration_value, configuration_updated_at)
        SELECT source.key, source.value, now() 
        FROM unnest(:keys, :values) AS source(key, value)
        ON CONFLICT ON CONSTRAINT configurations_pkey DO UPDATE
        SET configuration_value = excluded.configuration_value,
            configuration_updated_at = excluded.configuration_updated_at;
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("keys", entries.Select(entry => entry.Key))
            .AddParameter("values", entries.Select(entry => entry.Value));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
