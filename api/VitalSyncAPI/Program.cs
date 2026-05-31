using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Application.Security;
using VitalSyncAPI.Application.Services;
using VitalSyncAPI.Application.UseCases;
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

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

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
app.MapControllers();

app.Run();
