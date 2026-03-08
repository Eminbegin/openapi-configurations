using System.Data;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Configurations.Application.Abstractions.Persistence;
using Configurations.Application.Abstractions.Persistence.Queries;
using Configurations.Application.Contracts.Configurations;
using Configurations.Application.Contracts.Configurations.Operations;
using Configurations.Application.Model;

namespace Configurations.Application.Configurations;

internal sealed class ConfigurationService : IConfigurationService
{
    private readonly IPersistenceContext _context;
    private readonly IPersistenceTransactionProvider _transactionProvider;

    public ConfigurationService(IPersistenceContext context, IPersistenceTransactionProvider transactionProvider)
    {
        _context = context;
        _transactionProvider = transactionProvider;
    }

    public async Task SetConfigurationsAsync(
        SetConfigurations.Request request,
        CancellationToken cancellationToken)
    {
        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.Configurations.AddOrUpdateAsync(
            request.Entries,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<GetConfigurations.Response> GetConfigurationAsync(
        GetConfigurations.Request request,
        CancellationToken cancellationToken)
    {
        var query = ConfigurationQuery.Build(builder => builder
            .WithKeyCursor(request.PageToken?.Key)
            .WithPageSize(request.PageSize));

        ConfigurationEntry[] entries = await _context.Configurations
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        GetConfigurations.PageToken? responsePageToken = entries.Length < request.PageSize
            ? null
            : new GetConfigurations.PageToken(entries[^1].Key);

        return new GetConfigurations.Response(entries, responsePageToken);
    }
}
