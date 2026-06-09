using MassTransit;
using VitalSync.Contracts;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.UseCases;

namespace VitalSyncAPI.Tests;

public class NutritionUseCaseTests
{
    private static readonly Guid UserId   = Guid.NewGuid();
    private static readonly Guid OtherId  = Guid.NewGuid();
    private static readonly Guid RecordId = Guid.NewGuid();

    private static NutritionRecord MakeRecord(Guid userId) => new()
    {
        Id            = RecordId,
        UserId        = userId,
        MealType      = MealType.Lunch,
        FoodDescription = "Frango com arroz",
        CaloriesKcal  = 450,
        ProteinG      = 35,
        CarbsG        = 50,
        FatG          = 10,
        Confidence    = 0.9m,
        Status        = NutritionStatus.Completed,
        MeasuredAt    = DateTime.UtcNow,
        CreatedAt     = DateTime.UtcNow,
    };

    private static NutritionRequest MakeRequest() =>
        new(MealType.Lunch, null, DateTime.UtcNow, "base64imagedata");

    // ── AddNutritionRecordUseCase ──────────────────────────────────────────

    [Fact]
    public async Task Add_CriaRegistroPendingEPublicaEvento()
    {
        var repo           = Substitute.For<INutritionRecordRepository>();
        var unitOfWork     = Substitute.For<IUnitOfWork>();
        var publishEndpoint = Substitute.For<IPublishEndpoint>();

        var useCase = new AddNutritionRecordUseCase(repo, unitOfWork, publishEndpoint);
        var result  = await useCase.ExecuteAsync(UserId, MakeRequest());

        await repo.Received(1).AddNutritionRecord(Arg.Any<NutritionRecord>());
        await unitOfWork.Received(1).SaveChangesAsync();
        await publishEndpoint.Received(1)
            .Publish(Arg.Any<NutritionAnalysisRequestedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Add_StatusRetornadoEhPending()
    {
        var repo            = Substitute.For<INutritionRecordRepository>();
        var unitOfWork      = Substitute.For<IUnitOfWork>();
        var publishEndpoint = Substitute.For<IPublishEndpoint>();

        var useCase = new AddNutritionRecordUseCase(repo, unitOfWork, publishEndpoint);
        var result  = await useCase.ExecuteAsync(UserId, MakeRequest());

        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public async Task Add_DescricaoRetornadaEhPlaceholder()
    {
        var repo            = Substitute.For<INutritionRecordRepository>();
        var unitOfWork      = Substitute.For<IUnitOfWork>();
        var publishEndpoint = Substitute.For<IPublishEndpoint>();

        var useCase = new AddNutritionRecordUseCase(repo, unitOfWork, publishEndpoint);
        var result  = await useCase.ExecuteAsync(UserId, MakeRequest());

        Assert.Equal("Analisando...", result.FoodDescription);
    }

    // ── GetNutritionRecordUseCase ──────────────────────────────────────────

    [Fact]
    public async Task Get_ComPermissao_RetornaResponse()
    {
        var repo = Substitute.For<INutritionRecordRepository>();
        repo.GetById(RecordId).Returns(MakeRecord(UserId));

        var useCase = new GetNutritionRecordUseCase(repo);
        var result  = await useCase.ExecuteAsync(UserId, RecordId);

        Assert.Equal(RecordId, result.Id);
        Assert.Equal(450, result.CaloriesKcal);
    }

    [Fact]
    public async Task Get_NaoEncontrado_LancaNotFoundException()
    {
        var repo = Substitute.For<INutritionRecordRepository>();
        repo.GetById(RecordId).Returns((NutritionRecord?)null);

        var useCase = new GetNutritionRecordUseCase(repo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(UserId, RecordId));
    }

    [Fact]
    public async Task Get_OutroUserId_LancaForbiddenException()
    {
        var repo = Substitute.For<INutritionRecordRepository>();
        repo.GetById(RecordId).Returns(MakeRecord(UserId));

        var useCase = new GetNutritionRecordUseCase(repo);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => useCase.ExecuteAsync(OtherId, RecordId));
    }

    // ── UpdateNutritionRecordUseCase ───────────────────────────────────────

    [Fact]
    public async Task Update_AtualizaCampos()
    {
        var repo       = Substitute.For<INutritionRecordRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var record     = MakeRecord(UserId);

        repo.GetById(RecordId).Returns(record);

        var useCase  = new UpdateNutritionRecordUseCase(repo, unitOfWork);
        var request  = new NutritionRequest(MealType.Dinner, "notas novas", DateTime.UtcNow.AddHours(-1), "img");
        var result   = await useCase.ExecuteAsync(UserId, RecordId, request);

        Assert.Equal("Dinner", result.MealType);
        Assert.Equal("notas novas", result.Notes);
        repo.Received(1).UpdateNutritionRecord(Arg.Any<NutritionRecord>());
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Update_NaoEncontrado_LancaNotFoundException()
    {
        var repo       = Substitute.For<INutritionRecordRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repo.GetById(RecordId).Returns((NutritionRecord?)null);

        var useCase = new UpdateNutritionRecordUseCase(repo, unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(UserId, RecordId, MakeRequest()));
    }

    [Fact]
    public async Task Update_OutroUserId_LancaForbiddenException()
    {
        var repo       = Substitute.For<INutritionRecordRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repo.GetById(RecordId).Returns(MakeRecord(UserId));

        var useCase = new UpdateNutritionRecordUseCase(repo, unitOfWork);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => useCase.ExecuteAsync(OtherId, RecordId, MakeRequest()));
    }

    // ── DeleteNutritionRecordUseCase ───────────────────────────────────────

    [Fact]
    public async Task Delete_ComPermissao_RemoveRegistro()
    {
        var repo       = Substitute.For<INutritionRecordRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repo.GetById(RecordId).Returns(MakeRecord(UserId));

        var useCase = new DeleteNutritionRecordUseCase(repo, unitOfWork);
        await useCase.ExecuteAsync(UserId, RecordId);

        await repo.Received(1).RemoveNutritionRecord(RecordId);
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Delete_NaoEncontrado_LancaNotFoundException()
    {
        var repo       = Substitute.For<INutritionRecordRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repo.GetById(RecordId).Returns((NutritionRecord?)null);

        var useCase = new DeleteNutritionRecordUseCase(repo, unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(UserId, RecordId));
    }

    [Fact]
    public async Task Delete_OutroUserId_LancaForbiddenException()
    {
        var repo       = Substitute.For<INutritionRecordRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repo.GetById(RecordId).Returns(MakeRecord(UserId));

        var useCase = new DeleteNutritionRecordUseCase(repo, unitOfWork);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => useCase.ExecuteAsync(OtherId, RecordId));
    }

    // ── GetNutritionSummaryUseCase ─────────────────────────────────────────

    [Fact]
    public async Task GetSummary_SomaCorretamenteMacros()
    {
        var repo        = Substitute.For<INutritionRecordRepository>();
        var metricsRepo = Substitute.For<IBodyMetricsRepository>();

        var records = new List<NutritionRecord>
        {
            new() { CaloriesKcal = 400, ProteinG = 30, CarbsG = 45, FatG = 8,
                    MealType = MealType.Breakfast, FoodDescription = "", Status = NutritionStatus.Completed,
                    MeasuredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { CaloriesKcal = 600, ProteinG = 40, CarbsG = 70, FatG = 15,
                    MealType = MealType.Lunch, FoodDescription = "", Status = NutritionStatus.Completed,
                    MeasuredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
        };

        repo.GetByUserId(UserId, Arg.Any<DateTime?>(), Arg.Any<DateTime?>()).Returns(records);
        metricsRepo.GetLatestByUserId(UserId).Returns((BodyMetrics?)null);

        var useCase = new GetNutritionSummaryUseCase(repo, metricsRepo);
        var result  = await useCase.ExecuteAsync(UserId, DateTime.UtcNow);

        Assert.Equal(1000, result.TotalCaloriesKcal);
        Assert.Equal(70, result.TotalProteinG);
        Assert.Equal(115, result.TotalCarbsG);
        Assert.Equal(23, result.TotalFatG);
        Assert.Equal(2, result.Records.Count);
    }

    [Fact]
    public async Task GetSummary_SemBodyMetrics_MetasRetornamZero()
    {
        var repo        = Substitute.For<INutritionRecordRepository>();
        var metricsRepo = Substitute.For<IBodyMetricsRepository>();

        repo.GetByUserId(UserId, Arg.Any<DateTime?>(), Arg.Any<DateTime?>())
            .Returns(new List<NutritionRecord>());
        metricsRepo.GetLatestByUserId(UserId).Returns((BodyMetrics?)null);

        var useCase = new GetNutritionSummaryUseCase(repo, metricsRepo);
        var result  = await useCase.ExecuteAsync(UserId, DateTime.UtcNow);

        Assert.Equal(0, result.CalorieGoal);
        Assert.Equal(0, result.ProteinGoalG);
        Assert.Equal(0, result.CarbsGoalG);
        Assert.Equal(0, result.FatGoalG);
    }

    [Fact]
    public async Task GetSummary_ComBodyMetrics_RetornaMetasCalculadas()
    {
        var repo        = Substitute.For<INutritionRecordRepository>();
        var metricsRepo = Substitute.For<IBodyMetricsRepository>();
        var metrics     = new BodyMetrics
        {
            CalorieGoal  = 2000,
            ProteinGoalG = 150,
            CarbsGoalG   = 250,
            FatGoalG     = 65,
        };

        repo.GetByUserId(UserId, Arg.Any<DateTime?>(), Arg.Any<DateTime?>())
            .Returns(new List<NutritionRecord>());
        metricsRepo.GetLatestByUserId(UserId).Returns(metrics);

        var useCase = new GetNutritionSummaryUseCase(repo, metricsRepo);
        var result  = await useCase.ExecuteAsync(UserId, DateTime.UtcNow);

        Assert.Equal(2000, result.CalorieGoal);
        Assert.Equal(150, result.ProteinGoalG);
    }
}
