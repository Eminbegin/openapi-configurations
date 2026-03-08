using Configurations.Application;
using Configurations.Infrastructure.Persistence;
using Configurations.Presentation.Http;
using Itmo.Dev.Platform.Common.Extensions;

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
app.UsePresentationHttp();

await app.RunAsync();
