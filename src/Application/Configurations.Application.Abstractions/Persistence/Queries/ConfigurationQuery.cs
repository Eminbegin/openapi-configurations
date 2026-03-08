using SourceKit.Generators.Builder.Annotations;

namespace Configurations.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record ConfigurationQuery(
    string? KeyCursor,
    [RequiredValue] int PageSize);
