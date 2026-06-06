using MassTransit;
using VitalSyncAI.Consumers;
using VitalSyncAI.Services;
using VitalSync.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AnthropicService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InsightRequestedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:User"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.Message<VitalSync.Contracts.InsightRequestedEvent>(m => m.SetEntityName("insight-requested-event"));
        cfg.Message<VitalSync.Contracts.InsightGeneratedEvent>(m => m.SetEntityName("insight-generated-event"));

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.MapGet("/health", () => "ok");

app.Run();