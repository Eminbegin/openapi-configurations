using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Configurations.Presentation.Http.Models;

public sealed class GetConfigurationsRequest
{
    [FromQuery(Name = "pageToken")]
    public string? PageToken { get; init; }

    [FromQuery(Name = "pageSize")]
    [Range(minimum: 1, maximum: 1000)]
    public int PageSize { get; init; }
}
