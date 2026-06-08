using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using VitalSync.Contracts;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Application.Security;
using VitalSyncAPI.Application.Services;
using VitalSyncAPI.Application.UseCases;
using VitalSyncAPI.Consumers;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Infrastructure.Middlewares;
using VitalSyncAPI.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthenticateUseCase, AuthenticateUseCase>();
builder.Services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
builder.Services.AddScoped<IMetricTypesRepository, MetricTypesRepository>();
builder.Services.AddScoped<IGetMetricTypes, GetMetricTypes>();
builder.Services.AddScoped<IHealthRecordsRepository, HealthRecordsRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<ICreateHealthRecordUseCase, CreateHealthRecordUseCase>();
builder.Services.AddScoped<IEditHealthRecordUseCase, EditHealthRecordUseCase>();
builder.Services.AddScoped<IGetHealthRecordById, GetHealthRecordById>();
builder.Services.AddScoped<IGetHealthRecordByUser, GetHealthRecordByUser>();
builder.Services.AddScoped<IDeleteHealthRecord, DeleteHealthRecord>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IBodyMetricsRepository, UserMetricRepository>();
builder.Services.AddScoped<ISaveProfileUseCase, SaveProfileUseCase>();
builder.Services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
builder.Services.AddScoped<IGetAllUserProfileUseCase, GetAllUserProfileUseCase>();
builder.Services.AddScoped<IUserConditionRepository, UserConditionRepository>();
builder.Services.AddScoped<IGetUserConditionUseCase, GetUserConditionUseCase>();
builder.Services.AddScoped<IAddUserConditionUseCase, AddUserConditionUseCase>();
builder.Services.AddScoped<IUserMedicationRepository, UserMedicationRepository>();
builder.Services.AddScoped<IAddUserMedicationUseCase, AddUserMedicationUseCase>();
builder.Services.AddScoped<IGetUserMedicationUseCase, GetUserMedicationUseCase>();
builder.Services.AddScoped<IPersonalRangeRepository, PersonalRangeRepository>();
builder.Services.AddScoped<IRecalculatePersonalRangeUseCase, RecalculatePersonalRangeUseCase>();
builder.Services.AddScoped<IGetAllPersonalRangeUseCase, GetAllPersonalRangeUseCase>();
builder.Services.AddScoped<IGetPersonalRangeUseCase, GetPersonalRangeUseCase>();
builder.Services.AddScoped<IAIInsightRepository, AIInsightRepository>();
builder.Services.AddScoped<IGetAlertsUseCase, GetAlertsUseCase>();
builder.Services.AddScoped<INutritionRecordRepository, NutritionRecordRepository>();
builder.Services.AddScoped<IAddNutritionRecordUseCase, AddNutritionRecordUseCase>();
builder.Services.AddScoped<IDeleteNutritionRecordUseCase, DeleteNutritionRecordUseCase>();
builder.Services.AddScoped<IGetAllNutritionRecordUseCase, GetAllNutritionRecordUseCase>();
builder.Services.AddScoped<IGetNutritionRecordUseCase, GetNutritionRecordUseCase>();
builder.Services.AddScoped<IUpdateNutritionRecordUseCase, UpdateNutritionRecordUseCase>();
builder.Services.AddScoped<IGetNutritionSummaryUseCase, GetNutritionSummaryUseCase>();
builder.Services.AddScoped<IWeeklyReportRepository, WeeklyReportRepository>();
builder.Services.AddScoped<IAddWeeklyReportUseCase, AddWeeklyReportUseCase>();
builder.Services.AddScoped<IGetWeeklyReportsUseCase, GetWeeklyReportsUseCase>();
builder.Services.AddScoped<IGetWeeklyReportUseCase, GetWeeklyReportUseCase>();
builder.Services.AddScoped<IGetWeeklyReportsByIdUseCase, GetWeeklyReportsByIdUseCase>();
builder.Services.AddScoped<IAIAnalysisService, AnthropicService>();
builder.Services.AddScoped<JwtTokenGenerator>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("JWT Key not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                ctx.Token = ctx.Request.Cookies["vitalsync_token"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InsightGeneratedConsumer>();
    x.AddConsumer<NutritionAnalysisCompletedConsumer>();
    x.AddConsumer<WeeklyReportGeneratedConsumer>();
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

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{
    // Rate Limit global
    options.AddPolicy("global", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    // Rate limit para endpoints de IA
    options.AddPolicy("ai-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 15,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    options.AddPolicy("ai-limit-image", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    // Login
    options.AddPolicy("login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 8,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    options.AddPolicy("register", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    options.RejectionStatusCode = 429;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Context>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.ConfigObject.AdditionalItems["withCredentials"] = true;
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();
