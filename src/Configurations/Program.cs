using Configurations.Application;
using Configurations.Infrastructure.Persistence;
using Configurations.Presentation.Http;
using Itmo.Dev.Platform.Common.Extensions;
using Prometheus;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPlatform();

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

await app.RunAsync();
