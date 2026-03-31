using System.Text.Json;
using Configurations.Application.Contracts.Configurations;
using Configurations.Application.Contracts.Configurations.Operations;
using Configurations.Application.Model;
using Configurations.Presentation.Http.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Configurations.Presentation.Http.Controllers;

[ApiController]
[Route("api/configurations")]
public sealed class ConfigurationController : ControllerBase
{
    private readonly static Counter SetRequestsTotal = Metrics.CreateCounter(
        "configurations_set_requests_total",
        "Total number of set configurations requests.");

    private readonly static Counter GetRequestsTotal = Metrics.CreateCounter(
        "configurations_get_requests_total",
        "Total number of get configurations requests.");

    private readonly static Counter EntriesWrittenTotal = Metrics.CreateCounter(
        "configurations_entries_written_total",
        "Total number of configuration entries received in set requests.");

    private readonly static Counter EntriesReadTotal = Metrics.CreateCounter(
        "configurations_entries_read_total",
        "Total number of configuration entries returned by get requests.");

    private readonly static Histogram SetBatchSize = Metrics.CreateHistogram(
        "configurations_set_batch_size",
        "Distribution of number of entries in set requests.",
        new HistogramConfiguration
        {
            Buckets = Histogram.ExponentialBuckets(1, 2, 10),
        });

    private readonly static Histogram GetResultSize = Metrics.CreateHistogram(
        "configurations_get_result_size",
        "Distribution of number of entries returned by get requests.",
        new HistogramConfiguration
        {
            Buckets = Histogram.ExponentialBuckets(1, 2, 10),
        });

    private readonly IConfigurationService _configurationService;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(
        IConfigurationService configurationService,
        ILogger<ConfigurationController> logger)
    {
        _configurationService = configurationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SetConfigurationsAsync(
        [FromBody] SetConfigurationsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            SetRequestsTotal.Inc();

            ConfigurationEntry[] entries = request.Entries
                .Select(entry => new ConfigurationEntry(entry.Key, entry.Value, default))
                .ToArray();

            if (entries.Length == 0)
            {
                _logger.LogWarning("Set configurations request received with empty entries collection");
            }

            _logger.LogInformation("Set configurations request accepted with {EntriesCount} entries", entries.Length);

            var applicationRequest = new SetConfigurations.Request(entries);

            await _configurationService.SetConfigurationsAsync(applicationRequest, cancellationToken);

            EntriesWrittenTotal.Inc(entries.Length);
            SetBatchSize.Observe(entries.Length);

            _logger.LogInformation("Set configurations request completed successfully, written {EntriesCount} entries", entries.Length);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Set configurations request failed");
            throw;
        }
    }

    [HttpGet]
    public async Task<ActionResult<GetConfigurationsResponse>> GetConfigurationsAsync(
        [FromQuery] GetConfigurationsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            GetRequestsTotal.Inc();
            _logger.LogInformation(
                "Get configurations request accepted with page size {PageSize}, page token provided: {HasPageToken}",
                request.PageSize,
                request.PageToken is not null);

            GetConfigurations.PageToken? pageToken = request.PageToken is null
                ? null
                : JsonSerializer.Deserialize<GetConfigurations.PageToken>(request.PageToken);

            var applicationRequest = new GetConfigurations.Request(request.PageSize, pageToken);

            GetConfigurations.Response applicationResponse = await _configurationService.GetConfigurationAsync(
                applicationRequest,
                cancellationToken);

            IEnumerable<GetConfigurationsResponse.ConfigurationEntry> entries = applicationResponse.Entries
                .Select(entry => new GetConfigurationsResponse.ConfigurationEntry
                {
                    Key = entry.Key,
                    Value = entry.Value,
                });

            string? responsePageToken = applicationResponse.PageToken is null
                ? null
                : JsonSerializer.Serialize(applicationResponse.PageToken.Value);

            EntriesReadTotal.Inc(applicationResponse.Entries.Count);
            GetResultSize.Observe(applicationResponse.Entries.Count);

            _logger.LogInformation(
                "Get configurations request completed, returned {EntriesCount} entries, next page token provided: {HasNextPageToken}",
                applicationResponse.Entries.Count,
                applicationResponse.PageToken is not null);

            return Ok(new GetConfigurationsResponse
            {
                Entries = entries,
                PageToken = responsePageToken,
            });
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Invalid page token format in get configurations request");
            return BadRequest(new
            {
                Message = "Invalid page token format",
                StatusCode = StatusCodes.Status400BadRequest,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get configurations request failed");
            throw;
        }
    }
}