using System.Text.Json;
using Configurations.Application.Contracts.Configurations;
using Configurations.Application.Contracts.Configurations.Operations;
using Configurations.Application.Model;
using Configurations.Presentation.Http.Models;
using Microsoft.AspNetCore.Mvc;

namespace Configurations.Presentation.Http.Controllers;

[ApiController]
[Route("api/configurations")]
public sealed class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    [HttpPost]
    public async Task<IActionResult> SetConfigurationsAsync(
        [FromBody] SetConfigurationsRequest request,
        CancellationToken cancellationToken)
    {
        ConfigurationEntry[] entries = request.Entries
            .Select(entry => new ConfigurationEntry(entry.Key, entry.Value, default))
            .ToArray();

        var applicationRequest = new SetConfigurations.Request(entries);

        await _configurationService.SetConfigurationsAsync(applicationRequest, cancellationToken);

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<GetConfigurationsResponse>> GetConfigurationsAsync(
        [FromQuery] GetConfigurationsRequest request,
        CancellationToken cancellationToken)
    {
        GetConfigurations.PageToken? pageToken = request.PageToken is null
            ? null
            : JsonSerializer.Deserialize<GetConfigurations.PageToken>(request.PageToken);

        var applicationRequest = new GetConfigurations.Request(request.PageSize, pageToken);

        GetConfigurations.Response applicationResponse = await _configurationService.GetConfigurationAsync(
            applicationRequest,
            cancellationToken);

        var entries = applicationResponse.Entries
            .Select(entry => new GetConfigurationsResponse.ConfigurationEntry
            {
                Key = entry.Key,
                Value = entry.Value,
            });

        string? responsePageToken = applicationResponse.PageToken is null
            ? null
            : JsonSerializer.Serialize(applicationResponse.PageToken.Value);

        return Ok(new GetConfigurationsResponse
        {
            Entries = entries,
            PageToken = responsePageToken,
        });
    }
}