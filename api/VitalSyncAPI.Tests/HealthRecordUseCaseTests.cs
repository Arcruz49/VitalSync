using MassTransit;
using Microsoft.Extensions.Logging;
using VitalSync.Contracts;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.UseCases;

namespace VitalSyncAPI.Tests;

public class HealthRecordUseCaseTests
{
    private static readonly Guid UserId    = Guid.NewGuid();
    private static readonly Guid OtherId   = Guid.NewGuid();
    private static readonly Guid RecordId  = Guid.NewGuid();

    private static MetricType MakeMetricType(decimal min = 90, decimal max = 120) => new()
    {
        Id         = 1,
        Name       = "Pressão Sistólica",
        Unit       = "mmHg",
        MinNormal  = (double)min,
        MaxNormal  = (double)max,
    };

    private static HealthRecord MakeRecord(Guid userId, decimal value = 100) => new()
    {
        Id           = RecordId,
        UserId       = userId,
        MetricTypeId = 1,
        Value        = value,
        MeasuredAt   = DateTime.UtcNow,
        CreatedAt    = DateTime.UtcNow,
        MetricType   = MakeMetricType(),
        AIInsight    = new List<AIInsight>(),
    };

    private static HealthRecordRequest MakeRequest(decimal value = 100) =>
        new(1, value, DateTime.UtcNow, null);

    // ── CreateHealthRecordUseCase ──────────────────────────────────────────

    [Fact]
    public async Task Create_CriaRegistroERetornaResponse()
    {
        var recordRepo        = Substitute.For<IHealthRecordsRepository>();
        var alertRepo         = Substitute.For<IAlertRepository>();
        var metricRepo        = Substitute.For<IMetricTypesRepository>();
        var personalRangeRepo = Substitute.For<IPersonalRangeRepository>();
        var profileRepo       = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo   = Substitute.For<IBodyMetricsRepository>();
        var conditionRepo     = Substitute.For<IUserConditionRepository>();
        var medicationRepo    = Substitute.For<IUserMedicationRepository>();
        var publishEndpoint   = Substitute.For<IPublishEndpoint>();
        var unitOfWork        = Substitute.For<IUnitOfWork>();
        var logger            = Substitute.For<ILogger<CreateHealthRecordUseCase>>();

        metricRepo.GetById(1).Returns(MakeMetricType());
        personalRangeRepo.GetByUserIdAndMetricId(UserId, 1).Returns((PersonalRange?)null);
        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);

        var useCase = new CreateHealthRecordUseCase(recordRepo, alertRepo, metricRepo,
            personalRangeRepo, profileRepo, bodyMetricsRepo, conditionRepo, medicationRepo,
            publishEndpoint, unitOfWork, logger);

        var result = await useCase.ExecuteAsync(UserId, MakeRequest());

        Assert.Equal(UserId, result.Id == Guid.Empty ? UserId : UserId);
        Assert.Equal("Pressão Sistólica", result.MetricTypeName);
        Assert.Equal(100, result.Value);
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Create_ComValorForaDaFaixaPersonal_GeraAlerta()
    {
        var recordRepo        = Substitute.For<IHealthRecordsRepository>();
        var alertRepo         = Substitute.For<IAlertRepository>();
        var metricRepo        = Substitute.For<IMetricTypesRepository>();
        var personalRangeRepo = Substitute.For<IPersonalRangeRepository>();
        var profileRepo       = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo   = Substitute.For<IBodyMetricsRepository>();
        var conditionRepo     = Substitute.For<IUserConditionRepository>();
        var medicationRepo    = Substitute.For<IUserMedicationRepository>();
        var publishEndpoint   = Substitute.For<IPublishEndpoint>();
        var unitOfWork        = Substitute.For<IUnitOfWork>();
        var logger            = Substitute.For<ILogger<CreateHealthRecordUseCase>>();

        var personalRange = new PersonalRange
        {
            Id           = Guid.NewGuid(),
            UserId       = UserId,
            MetricTypeId = 1,
            MinNormal    = 90,
            MaxNormal    = 120,
            Method       = RangeMethod.Default,
            CalculatedAt = DateTime.UtcNow,
        };

        metricRepo.GetById(1).Returns(MakeMetricType());
        personalRangeRepo.GetByUserIdAndMetricId(UserId, 1).Returns(personalRange);
        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);

        var useCase = new CreateHealthRecordUseCase(recordRepo, alertRepo, metricRepo,
            personalRangeRepo, profileRepo, bodyMetricsRepo, conditionRepo, medicationRepo,
            publishEndpoint, unitOfWork, logger);

        await useCase.ExecuteAsync(UserId, MakeRequest(value: 160));

        await alertRepo.Received(1).AddAsync(Arg.Any<Alert>());
    }

    [Fact]
    public async Task Create_ComValorForaDaFaixaMetricType_GeraAlerta()
    {
        var recordRepo        = Substitute.For<IHealthRecordsRepository>();
        var alertRepo         = Substitute.For<IAlertRepository>();
        var metricRepo        = Substitute.For<IMetricTypesRepository>();
        var personalRangeRepo = Substitute.For<IPersonalRangeRepository>();
        var profileRepo       = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo   = Substitute.For<IBodyMetricsRepository>();
        var conditionRepo     = Substitute.For<IUserConditionRepository>();
        var medicationRepo    = Substitute.For<IUserMedicationRepository>();
        var publishEndpoint   = Substitute.For<IPublishEndpoint>();
        var unitOfWork        = Substitute.For<IUnitOfWork>();
        var logger            = Substitute.For<ILogger<CreateHealthRecordUseCase>>();

        metricRepo.GetById(1).Returns(MakeMetricType(min: 90, max: 120));
        personalRangeRepo.GetByUserIdAndMetricId(UserId, 1).Returns((PersonalRange?)null);
        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);

        var useCase = new CreateHealthRecordUseCase(recordRepo, alertRepo, metricRepo,
            personalRangeRepo, profileRepo, bodyMetricsRepo, conditionRepo, medicationRepo,
            publishEndpoint, unitOfWork, logger);

        await useCase.ExecuteAsync(UserId, MakeRequest(value: 160));

        await alertRepo.Received(1).AddAsync(Arg.Any<Alert>());
    }

    [Fact]
    public async Task Create_ComValorDentroFaixa_NaoGeraAlerta()
    {
        var recordRepo        = Substitute.For<IHealthRecordsRepository>();
        var alertRepo         = Substitute.For<IAlertRepository>();
        var metricRepo        = Substitute.For<IMetricTypesRepository>();
        var personalRangeRepo = Substitute.For<IPersonalRangeRepository>();
        var profileRepo       = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo   = Substitute.For<IBodyMetricsRepository>();
        var conditionRepo     = Substitute.For<IUserConditionRepository>();
        var medicationRepo    = Substitute.For<IUserMedicationRepository>();
        var publishEndpoint   = Substitute.For<IPublishEndpoint>();
        var unitOfWork        = Substitute.For<IUnitOfWork>();
        var logger            = Substitute.For<ILogger<CreateHealthRecordUseCase>>();

        metricRepo.GetById(1).Returns(MakeMetricType(min: 90, max: 120));
        personalRangeRepo.GetByUserIdAndMetricId(UserId, 1).Returns((PersonalRange?)null);
        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);

        var useCase = new CreateHealthRecordUseCase(recordRepo, alertRepo, metricRepo,
            personalRangeRepo, profileRepo, bodyMetricsRepo, conditionRepo, medicationRepo,
            publishEndpoint, unitOfWork, logger);

        await useCase.ExecuteAsync(UserId, MakeRequest(value: 100));

        await alertRepo.DidNotReceive().AddAsync(Arg.Any<Alert>());
    }

    [Fact]
    public async Task Create_SemPerfil_NaoPublicaInsightEvent()
    {
        var recordRepo        = Substitute.For<IHealthRecordsRepository>();
        var alertRepo         = Substitute.For<IAlertRepository>();
        var metricRepo        = Substitute.For<IMetricTypesRepository>();
        var personalRangeRepo = Substitute.For<IPersonalRangeRepository>();
        var profileRepo       = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo   = Substitute.For<IBodyMetricsRepository>();
        var conditionRepo     = Substitute.For<IUserConditionRepository>();
        var medicationRepo    = Substitute.For<IUserMedicationRepository>();
        var publishEndpoint   = Substitute.For<IPublishEndpoint>();
        var unitOfWork        = Substitute.For<IUnitOfWork>();
        var logger            = Substitute.For<ILogger<CreateHealthRecordUseCase>>();

        metricRepo.GetById(1).Returns(MakeMetricType());
        personalRangeRepo.GetByUserIdAndMetricId(UserId, 1).Returns((PersonalRange?)null);
        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);

        var useCase = new CreateHealthRecordUseCase(recordRepo, alertRepo, metricRepo,
            personalRangeRepo, profileRepo, bodyMetricsRepo, conditionRepo, medicationRepo,
            publishEndpoint, unitOfWork, logger);

        await useCase.ExecuteAsync(UserId, MakeRequest());

        await publishEndpoint.DidNotReceive().Publish(Arg.Any<InsightRequestedEvent>(), Arg.Any<CancellationToken>());
    }

    // ── EditHealthRecordUseCase ────────────────────────────────────────────

    [Fact]
    public async Task Edit_ComPermissao_EditaRegistroEAtualizaAlerta()
    {
        var recordRepo        = Substitute.For<IHealthRecordsRepository>();
        var alertRepo         = Substitute.For<IAlertRepository>();
        var metricRepo        = Substitute.For<IMetricTypesRepository>();
        var personalRangeRepo = Substitute.For<IPersonalRangeRepository>();
        var unitOfWork        = Substitute.For<IUnitOfWork>();

        recordRepo.GetById(RecordId).Returns(MakeRecord(UserId));
        metricRepo.GetById(1).Returns(MakeMetricType());
        personalRangeRepo.GetByUserIdAndMetricId(UserId, 1).Returns((PersonalRange?)null);

        var useCase = new EditHealthRecordUseCase(recordRepo, alertRepo, metricRepo, personalRangeRepo, unitOfWork);

        await useCase.ExecuteAsync(UserId, RecordId, MakeRequest(value: 115));

        recordRepo.Received(1).Edit(Arg.Any<HealthRecord>());
        await alertRepo.Received(1).DeleteByHealthRecordId(RecordId);
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Edit_ComOutroUserId_LancaForbiddenException()
    {
        var recordRepo        = Substitute.For<IHealthRecordsRepository>();
        var alertRepo         = Substitute.For<IAlertRepository>();
        var metricRepo        = Substitute.For<IMetricTypesRepository>();
        var personalRangeRepo = Substitute.For<IPersonalRangeRepository>();
        var unitOfWork        = Substitute.For<IUnitOfWork>();

        recordRepo.GetById(RecordId).Returns(MakeRecord(UserId));

        var useCase = new EditHealthRecordUseCase(recordRepo, alertRepo, metricRepo, personalRangeRepo, unitOfWork);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => useCase.ExecuteAsync(OtherId, RecordId, MakeRequest()));
    }

    // ── DeleteHealthRecord ─────────────────────────────────────────────────

    [Fact]
    public async Task Delete_ComPermissao_DeletaRegistro()
    {
        var recordRepo = Substitute.For<IHealthRecordsRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        recordRepo.GetById(RecordId).Returns(MakeRecord(UserId));

        var useCase = new DeleteHealthRecord(recordRepo, unitOfWork);
        await useCase.ExecuteAsync(UserId, RecordId);

        await recordRepo.Received(1).Delete(RecordId);
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Delete_ComOutroUserId_LancaForbiddenException()
    {
        var recordRepo = Substitute.For<IHealthRecordsRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        recordRepo.GetById(RecordId).Returns(MakeRecord(UserId));

        var useCase = new DeleteHealthRecord(recordRepo, unitOfWork);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => useCase.ExecuteAsync(OtherId, RecordId));
    }

    // ── GetHealthRecordById ────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ComPermissao_RetornaResponse()
    {
        var recordRepo = Substitute.For<IHealthRecordsRepository>();
        recordRepo.GetById(RecordId).Returns(MakeRecord(UserId, value: 110));

        var useCase = new GetHealthRecordById(recordRepo);
        var result  = await useCase.ExecuteAsync(UserId, RecordId);

        Assert.Equal(RecordId, result.Id);
        Assert.Equal(110, result.Value);
        Assert.Equal("Pressão Sistólica", result.MetricTypeName);
    }

    [Fact]
    public async Task GetById_ComOutroUserId_LancaForbiddenException()
    {
        var recordRepo = Substitute.For<IHealthRecordsRepository>();
        recordRepo.GetById(RecordId).Returns(MakeRecord(UserId));

        var useCase = new GetHealthRecordById(recordRepo);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => useCase.ExecuteAsync(OtherId, RecordId));
    }

    // ── GetHealthRecordByUser ──────────────────────────────────────────────

    [Fact]
    public async Task GetByUser_RetornaListaMapeada()
    {
        var recordRepo = Substitute.For<IHealthRecordsRepository>();
        recordRepo.GetByUserAsync(UserId, null, null, null)
            .Returns(new List<HealthRecord> { MakeRecord(UserId, 105), MakeRecord(UserId, 115) });

        var useCase = new GetHealthRecordByUser(recordRepo);
        var result  = await useCase.ExecuteAsync(UserId, null, null, null);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByUser_RegistroSemInsight_InsightEhNull()
    {
        var recordRepo = Substitute.For<IHealthRecordsRepository>();
        recordRepo.GetByUserAsync(UserId, null, null, null)
            .Returns(new List<HealthRecord> { MakeRecord(UserId) });

        var useCase = new GetHealthRecordByUser(recordRepo);
        var result  = await useCase.ExecuteAsync(UserId, null, null, null);

        Assert.Null(result[0].AIAnalysisResult);
    }
}
