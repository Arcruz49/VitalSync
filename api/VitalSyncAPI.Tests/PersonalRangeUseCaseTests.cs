using VitalSyncAPI.Application.UseCases;

namespace VitalSyncAPI.Tests;

public class PersonalRangeUseCaseTests
{
    private static readonly Guid UserId  = Guid.NewGuid();
    private static readonly Guid RangeId = Guid.NewGuid();

    private static PersonalRange MakeRange(int metricTypeId = 1) => new()
    {
        Id           = RangeId,
        UserId       = UserId,
        MetricTypeId = metricTypeId,
        MinNormal    = 90,
        MaxNormal    = 120,
        Method       = RangeMethod.Default,
        CalculatedAt = DateTime.UtcNow,
        MetricType   = new MetricType { Id = metricTypeId, Name = "Pressão Sistólica", Unit = "mmHg" },
    };

    // ── GetPersonalRangeUseCase ────────────────────────────────────────────

    [Fact]
    public async Task GetPersonalRange_RetornaResponseMapeado()
    {
        var repo = Substitute.For<IPersonalRangeRepository>();
        repo.GetByUserIdAndMetricId(UserId, 1).Returns(MakeRange());

        var useCase = new GetPersonalRangeUseCase(repo);
        var result  = await useCase.ExecuteAsync(UserId, 1);

        Assert.Equal(RangeId, result.Id);
        Assert.Equal(90, result.MinNormal);
        Assert.Equal(120, result.MaxNormal);
        Assert.Equal("Pressão Sistólica", result.MetricTypeName);
        Assert.Equal("Default", result.Method);
    }

    [Fact]
    public async Task GetPersonalRange_NaoEncontrada_LancaNotFoundException()
    {
        var repo = Substitute.For<IPersonalRangeRepository>();
        repo.GetByUserIdAndMetricId(UserId, 1).Returns((PersonalRange?)null);

        var useCase = new GetPersonalRangeUseCase(repo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(UserId, 1));
    }

    // ── GetAllPersonalRangeUseCase ─────────────────────────────────────────

    [Fact]
    public async Task GetAllPersonalRange_RetornaListaMapeada()
    {
        var repo = Substitute.For<IPersonalRangeRepository>();
        repo.GetByUserId(UserId).Returns(new List<PersonalRange>
        {
            MakeRange(1),
            MakeRange(2),
        });

        var useCase = new GetAllPersonalRangeUseCase(repo);
        var result  = await useCase.ExecuteAsync(UserId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllPersonalRange_SemFaixas_RetornaListaVazia()
    {
        var repo = Substitute.For<IPersonalRangeRepository>();
        repo.GetByUserId(UserId).Returns(new List<PersonalRange>());

        var useCase = new GetAllPersonalRangeUseCase(repo);
        var result  = await useCase.ExecuteAsync(UserId);

        Assert.Empty(result);
    }

    // ── RecalculatePersonalRangeUseCase ────────────────────────────────────

    [Fact]
    public async Task Recalculate_SemPerfil_LancaNotFoundException()
    {
        var profileRepo     = Substitute.For<IUserProfileRepository>();
        var conditionRepo   = Substitute.For<IUserConditionRepository>();
        var medicationRepo  = Substitute.For<IUserMedicationRepository>();
        var personalRepo    = Substitute.For<IPersonalRangeRepository>();
        var metricTypeRepo  = Substitute.For<IMetricTypesRepository>();
        var unitOfWork      = Substitute.For<IUnitOfWork>();

        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);

        var useCase = new RecalculatePersonalRangeUseCase(profileRepo, conditionRepo,
            medicationRepo, personalRepo, metricTypeRepo, unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(UserId));
    }

    [Fact]
    public async Task Recalculate_ComPerfil_RemoveAntigasEAdicionaNovas()
    {
        var profileRepo    = Substitute.For<IUserProfileRepository>();
        var conditionRepo  = Substitute.For<IUserConditionRepository>();
        var medicationRepo = Substitute.For<IUserMedicationRepository>();
        var personalRepo   = Substitute.For<IPersonalRangeRepository>();
        var metricTypeRepo = Substitute.For<IMetricTypesRepository>();
        var unitOfWork     = Substitute.For<IUnitOfWork>();

        profileRepo.GetByUserId(UserId).Returns(new UserProfile
        {
            Id = Guid.NewGuid(), UserId = UserId, HeightCm = 175,
            ActivityLevel = ActivityLevel.ModeratelyActive, Goal = HealthGoal.Maintenance,
            TrainingFrequencyDays = 3, TrainingTypes = "[]",
            HoursSeated = HoursSeated.Between4And6, HabitualSleepHours = 7, SleepQuality = 4,
        });
        conditionRepo.GetByUserId(UserId).Returns(new List<UserCondition>());
        medicationRepo.GetByUserId(UserId).Returns(new List<UserMedication>());
        metricTypeRepo.GetAll().Returns(new List<MetricType>
        {
            new() { Id = 1, Name = "Pressão Sistólica", Unit = "mmHg", MinNormal = 90, MaxNormal = 120 },
            new() { Id = 4, Name = "Frequência Cardíaca", Unit = "bpm", MinNormal = 60, MaxNormal = 100 },
        });

        var useCase = new RecalculatePersonalRangeUseCase(profileRepo, conditionRepo,
            medicationRepo, personalRepo, metricTypeRepo, unitOfWork);

        await useCase.ExecuteAsync(UserId);

        await personalRepo.Received(1).RemoveAllByUserId(UserId);
        await personalRepo.Received(1).AddPersonalRanges(Arg.Any<List<PersonalRange>>());
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Recalculate_ComPerfil_FaixasCalculadasContemMetricasEsperadas()
    {
        var profileRepo    = Substitute.For<IUserProfileRepository>();
        var conditionRepo  = Substitute.For<IUserConditionRepository>();
        var medicationRepo = Substitute.For<IUserMedicationRepository>();
        var personalRepo   = Substitute.For<IPersonalRangeRepository>();
        var metricTypeRepo = Substitute.For<IMetricTypesRepository>();
        var unitOfWork     = Substitute.For<IUnitOfWork>();

        profileRepo.GetByUserId(UserId).Returns(new UserProfile
        {
            Id = Guid.NewGuid(), UserId = UserId, HeightCm = 170,
            ActivityLevel = ActivityLevel.Sedentary, Goal = HealthGoal.Maintenance,
            TrainingFrequencyDays = 0, TrainingTypes = "[]",
            HoursSeated = HoursSeated.MoreThan8, HabitualSleepHours = 7, SleepQuality = 3,
        });
        conditionRepo.GetByUserId(UserId).Returns(new List<UserCondition>());
        medicationRepo.GetByUserId(UserId).Returns(new List<UserMedication>());
        metricTypeRepo.GetAll().Returns(new List<MetricType>
        {
            new() { Id = 1, Name = "Pressão Sistólica", Unit = "mmHg", MinNormal = 90, MaxNormal = 120 },
        });

        List<PersonalRange>? capturedRanges = null;
        await personalRepo.AddPersonalRanges(Arg.Do<List<PersonalRange>>(r => capturedRanges = r));

        var useCase = new RecalculatePersonalRangeUseCase(profileRepo, conditionRepo,
            medicationRepo, personalRepo, metricTypeRepo, unitOfWork);

        await useCase.ExecuteAsync(UserId);

        Assert.NotNull(capturedRanges);
        Assert.All(capturedRanges, r => Assert.Equal(UserId, r.UserId));
    }
}
