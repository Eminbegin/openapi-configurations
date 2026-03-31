using Configurations.Application;
using Configurations.Infrastructure.Persistence;
using Configurations.Presentation.Http;
using Itmo.Dev.Platform.Common.Extensions;
using Prometheus;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

string lokiUrl = Environment.GetEnvironmentVariable("LOKI_URL") ?? "http://loki:3100";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki(
        lokiUrl,
        labels:
        [
            new LokiLabel { Key = "app", Value = "configurations-service" },
            new LokiLabel { Key = "environment", Value = "dev" },
        ])
    .CreateLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();
    builder.Services.AddPlatform(platform => platform.WithNewtonsoftSerialization());

    builder.Services.AddSwaggerGen(c =>
    {
        c.CustomSchemaIds(t => t.FullName!.Replace("+", "."));
    });

    builder.Services
        .AddApplication()
        .AddPersistence()
        .AddPresentationHttp();

    WebApplication app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();
    app.UseHttpMetrics();
    app.UsePresentationHttp();
    app.MapMetrics();

    Log.Information("Configuration service started with Loki URL {LokiUrl}", lokiUrl);
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Configuration service terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
