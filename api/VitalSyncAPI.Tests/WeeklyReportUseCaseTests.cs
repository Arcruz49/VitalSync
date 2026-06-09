using System.Text.Json;
using MassTransit;
using VitalSync.Contracts;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.UseCases;

namespace VitalSyncAPI.Tests;

public class WeeklyReportUseCaseTests
{
    private static readonly Guid UserId   = Guid.NewGuid();
    private static readonly Guid OtherId  = Guid.NewGuid();
    private static readonly Guid ReportId = Guid.NewGuid();

    private static WeeklyReport MakeReport(ReportStatus status = ReportStatus.Completed) => new()
    {
        Id              = ReportId,
        UserId          = UserId,
        WeekStart       = DateTime.UtcNow.Date.AddDays(-7),
        WeekEnd         = DateTime.UtcNow.Date,
        Summary         = "Boa semana.",
        MetricsAnalysis = "",
        Patterns        = "[]",
        Recommendations = "[]",
        NutritionSummary = null,
        Disclaimer      = "Não é aconselhamento médico.",
        Status          = status,
        CreatedAt       = DateTime.UtcNow,
    };

    // ── AddWeeklyReportUseCase ─────────────────────────────────────────────

    [Fact]
    public async Task Add_CriaRelatorioPendingEPublicaEvento()
    {
        var weeklyRepo       = Substitute.For<IWeeklyReportRepository>();
        var healthRepo       = Substitute.For<IHealthRecordsRepository>();
        var nutritionRepo    = Substitute.For<INutritionRecordRepository>();
        var profileRepo      = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo  = Substitute.For<IBodyMetricsRepository>();
        var conditionRepo    = Substitute.For<IUserConditionRepository>();
        var medicationRepo   = Substitute.For<IUserMedicationRepository>();
        var publishEndpoint  = Substitute.For<IPublishEndpoint>();
        var unitOfWork       = Substitute.For<IUnitOfWork>();

        weeklyRepo.GetByUserIdAndWeek(UserId, Arg.Any<DateTime>()).Returns((WeeklyReport?)null);
        healthRepo.GetByUserAsync(UserId, null, Arg.Any<DateTime?>(), Arg.Any<DateTime?>())
            .Returns(new List<HealthRecord>());
        nutritionRepo.GetByUserId(UserId, Arg.Any<DateTime?>(), Arg.Any<DateTime?>())
            .Returns(new List<NutritionRecord>());
        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);
        bodyMetricsRepo.GetLatestByUserId(UserId).Returns((BodyMetrics?)null);
        conditionRepo.GetByUserId(UserId).Returns(new List<UserCondition>());
        medicationRepo.GetByUserId(UserId).Returns(new List<UserMedication>());

        var useCase = new AddWeeklyReportUseCase(weeklyRepo, healthRepo, nutritionRepo,
            profileRepo, bodyMetricsRepo, conditionRepo, medicationRepo, publishEndpoint, unitOfWork);

        var result = await useCase.ExecuteAsync(UserId, new WeeklyReportRequest(null));

        await weeklyRepo.Received(1).AddWeeklyReport(Arg.Any<WeeklyReport>());
        await unitOfWork.Received(1).SaveChangesAsync();
        await publishEndpoint.Received(1)
            .Publish(Arg.Any<WeeklyReportRequestedEvent>(), Arg.Any<CancellationToken>());
        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public async Task Add_RelatorioExistenteComStatusAtivo_LancaValidationException()
    {
        var weeklyRepo      = Substitute.For<IWeeklyReportRepository>();
        var healthRepo      = Substitute.For<IHealthRecordsRepository>();
        var nutritionRepo   = Substitute.For<INutritionRecordRepository>();
        var profileRepo     = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo = Substitute.For<IBodyMetricsRepository>();
        var conditionRepo   = Substitute.For<IUserConditionRepository>();
        var medicationRepo  = Substitute.For<IUserMedicationRepository>();
        var publishEndpoint = Substitute.For<IPublishEndpoint>();
        var unitOfWork      = Substitute.For<IUnitOfWork>();

        weeklyRepo.GetByUserIdAndWeek(UserId, Arg.Any<DateTime>())
            .Returns(MakeReport(ReportStatus.Completed));

        var useCase = new AddWeeklyReportUseCase(weeklyRepo, healthRepo, nutritionRepo,
            profileRepo, bodyMetricsRepo, conditionRepo, medicationRepo, publishEndpoint, unitOfWork);

        await Assert.ThrowsAsync<ValidationException>(
            () => useCase.ExecuteAsync(UserId, new WeeklyReportRequest(null)));
    }

    [Fact]
    public async Task Add_RelatorioExistenteComStatusFailed_PermiteCriarNovo()
    {
        var weeklyRepo      = Substitute.For<IWeeklyReportRepository>();
        var healthRepo      = Substitute.For<IHealthRecordsRepository>();
        var nutritionRepo   = Substitute.For<INutritionRecordRepository>();
        var profileRepo     = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo = Substitute.For<IBodyMetricsRepository>();
        var conditionRepo   = Substitute.For<IUserConditionRepository>();
        var medicationRepo  = Substitute.For<IUserMedicationRepository>();
        var publishEndpoint = Substitute.For<IPublishEndpoint>();
        var unitOfWork      = Substitute.For<IUnitOfWork>();

        weeklyRepo.GetByUserIdAndWeek(UserId, Arg.Any<DateTime>())
            .Returns(MakeReport(ReportStatus.Failed));
        healthRepo.GetByUserAsync(UserId, null, Arg.Any<DateTime?>(), Arg.Any<DateTime?>())
            .Returns(new List<HealthRecord>());
        nutritionRepo.GetByUserId(UserId, Arg.Any<DateTime?>(), Arg.Any<DateTime?>())
            .Returns(new List<NutritionRecord>());
        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);
        bodyMetricsRepo.GetLatestByUserId(UserId).Returns((BodyMetrics?)null);
        conditionRepo.GetByUserId(UserId).Returns(new List<UserCondition>());
        medicationRepo.GetByUserId(UserId).Returns(new List<UserMedication>());

        var useCase = new AddWeeklyReportUseCase(weeklyRepo, healthRepo, nutritionRepo,
            profileRepo, bodyMetricsRepo, conditionRepo, medicationRepo, publishEndpoint, unitOfWork);

        var result = await useCase.ExecuteAsync(UserId, new WeeklyReportRequest(null));

        Assert.Equal("Pending", result.Status);
        await weeklyRepo.Received(1).AddWeeklyReport(Arg.Any<WeeklyReport>());
    }

    // ── GetWeeklyReportsByIdUseCase ────────────────────────────────────────

    [Fact]
    public async Task GetById_RetornaResponseMapeado()
    {
        var weeklyRepo = Substitute.For<IWeeklyReportRepository>();
        weeklyRepo.GetById(ReportId).Returns(MakeReport());

        var useCase = new GetWeeklyReportsByIdUseCase(weeklyRepo);
        var result  = await useCase.ExecuteAsync(ReportId, UserId);

        Assert.Equal(ReportId, result.Id);
        Assert.Equal("Completed", result.Status);
        Assert.Equal("Boa semana.", result.Summary);
    }

    [Fact]
    public async Task GetById_NaoEncontrado_LancaNotFoundException()
    {
        var weeklyRepo = Substitute.For<IWeeklyReportRepository>();
        weeklyRepo.GetById(ReportId).Returns((WeeklyReport?)null);

        var useCase = new GetWeeklyReportsByIdUseCase(weeklyRepo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(ReportId, UserId));
    }

    [Fact]
    public async Task GetById_OutroUserId_LancaForbiddenException()
    {
        var weeklyRepo = Substitute.For<IWeeklyReportRepository>();
        weeklyRepo.GetById(ReportId).Returns(MakeReport());

        var useCase = new GetWeeklyReportsByIdUseCase(weeklyRepo);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => useCase.ExecuteAsync(ReportId, OtherId));
    }

    [Fact]
    public async Task GetById_SemMetricsAnalysis_RetornaNull()
    {
        var weeklyRepo = Substitute.For<IWeeklyReportRepository>();
        weeklyRepo.GetById(ReportId).Returns(MakeReport());

        var useCase = new GetWeeklyReportsByIdUseCase(weeklyRepo);
        var result  = await useCase.ExecuteAsync(ReportId, UserId);

        Assert.Null(result.MetricsAnalysis);
    }

    [Fact]
    public async Task GetById_ComMetricsAnalysis_DeserializaLista()
    {
        var weeklyRepo = Substitute.For<IWeeklyReportRepository>();
        var report     = MakeReport();
        report.MetricsAnalysis = JsonSerializer.Serialize(new List<WeeklyMetricAnalysisResponse>
        {
            new("Pressão Sistólica", "mmHg", 115, 100, 130, "stable"),
        });
        weeklyRepo.GetById(ReportId).Returns(report);

        var useCase = new GetWeeklyReportsByIdUseCase(weeklyRepo);
        var result  = await useCase.ExecuteAsync(ReportId, UserId);

        Assert.NotNull(result.MetricsAnalysis);
        Assert.Single(result.MetricsAnalysis);
        Assert.Equal("Pressão Sistólica", result.MetricsAnalysis[0].MetricName);
    }
}
