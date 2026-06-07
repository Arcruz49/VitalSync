using MassTransit;
using VitalSyncAI.Consumers;
using VitalSyncAI.Services;
using VitalSync.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AnthropicService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InsightRequestedConsumer>();
    x.AddConsumer<NutritionAnalysisRequestedConsumer>();
    x.AddConsumer<WeeklyReportRequestedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:User"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.Message<InsightRequestedEvent>(m => m.SetEntityName("insight-requested-event"));
        cfg.Message<InsightGeneratedEvent>(m => m.SetEntityName("insight-generated-event"));
        cfg.Message<NutritionAnalysisRequestedEvent>(m => m.SetEntityName("nutrition-analysis-requested-event"));
        cfg.Message<NutritionAnalysisCompletedEvent>(m => m.SetEntityName("nutrition-analysis-completed-event"));
        cfg.Message<WeeklyReportRequestedEvent>(m => m.SetEntityName("weekly-report-requested-event"));
        cfg.Message<WeeklyReportGeneratedEvent>(m => m.SetEntityName("weekly-report-generated-event"));

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.MapGet("/health", () => "ok");

app.Run();