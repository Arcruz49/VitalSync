using System.Text.Json;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.UseCases;

namespace VitalSyncAPI.Tests;

public class ProfileUseCaseTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static User MakeUser() => new()
    {
        Id           = UserId,
        Name         = "Test User",
        Email        = "test@test.com",
        Gender       = "Male",
        BirthDate    = new DateTime(1994, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreationDate = DateTime.UtcNow,
    };

    private static UserProfileRequest MakeRequest() => new(
        HeightCm:               175,
        WeightKg:               75,
        TargetWeightKg:         70,
        WaistCircumferenceCm:   null,
        BodyFatPercent:         null,
        ActivityLevel:          ActivityLevel.ModeratelyActive,
        Goal:                   HealthGoal.WeightLoss,
        TrainingFrequencyDays:  3,
        TrainingTypes:          new List<string> { "Running" },
        HoursSeated:            HoursSeated.Between4And6,
        HabitualSleepHours:     7.5m,
        SleepQuality:           4
    );

    private static UserProfile MakeProfile() => new()
    {
        Id                    = Guid.NewGuid(),
        UserId                = UserId,
        HeightCm              = 175,
        ActivityLevel         = ActivityLevel.ModeratelyActive,
        Goal                  = HealthGoal.WeightLoss,
        TrainingFrequencyDays = 3,
        TrainingTypes         = JsonSerializer.Serialize(new List<string> { "Running" }),
        HoursSeated           = HoursSeated.Between4And6,
        HabitualSleepHours    = 7.5m,
        SleepQuality          = 4,
        CreatedAt             = DateTime.UtcNow,
        UpdatedAt             = DateTime.UtcNow,
    };

    private static BodyMetrics MakeBodyMetrics() => new()
    {
        Id             = Guid.NewGuid(),
        UserId         = UserId,
        WeightKg       = 75,
        BMI            = 24.49m,
        IdealWeightMinKg = 63.5m,
        IdealWeightMaxKg = 76.4m,
        BMR            = 1800,
        TDEE           = 2790,
        CalorieGoal    = 2290,
        ProteinGoalG   = 120,
        CarbsGoalG     = 280,
        FatGoalG       = 64,
        RecordedAt     = DateTime.UtcNow,
    };

    // ── SaveProfileUseCase ─────────────────────────────────────────────────

    [Fact]
    public async Task Save_SemPerfilExistente_CriaNovoPerfilERetornaResponse()
    {
        var userRepo        = Substitute.For<IUserRepository>();
        var profileRepo     = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo = Substitute.For<IBodyMetricsRepository>();
        var unitOfWork      = Substitute.For<IUnitOfWork>();

        userRepo.GetUserById(UserId).Returns(MakeUser());
        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);

        var useCase = new SaveProfileUseCase(userRepo, profileRepo, bodyMetricsRepo, unitOfWork);
        var result  = await useCase.ExecuteAsync(UserId, MakeRequest());

        await profileRepo.Received(1).CreateProfile(Arg.Any<UserProfile>());
        await bodyMetricsRepo.Received(1).CreateMetrics(Arg.Any<BodyMetrics>());
        await unitOfWork.Received(1).SaveChangesAsync();
        Assert.Equal(UserId, result.UserId);
        Assert.Equal(175, result.HeightCm);
    }

    [Fact]
    public async Task Save_ComPerfilExistente_AtualizaPerfilExistente()
    {
        var userRepo        = Substitute.For<IUserRepository>();
        var profileRepo     = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo = Substitute.For<IBodyMetricsRepository>();
        var unitOfWork      = Substitute.For<IUnitOfWork>();

        userRepo.GetUserById(UserId).Returns(MakeUser());
        profileRepo.GetByUserId(UserId).Returns(MakeProfile());

        var useCase = new SaveProfileUseCase(userRepo, profileRepo, bodyMetricsRepo, unitOfWork);
        await useCase.ExecuteAsync(UserId, MakeRequest());

        profileRepo.Received(1).UpdateProfile(Arg.Any<UserProfile>());
        await profileRepo.DidNotReceive().CreateProfile(Arg.Any<UserProfile>());
    }

    [Fact]
    public async Task Save_SempreCalculaBodyMetrics()
    {
        var userRepo        = Substitute.For<IUserRepository>();
        var profileRepo     = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo = Substitute.For<IBodyMetricsRepository>();
        var unitOfWork      = Substitute.For<IUnitOfWork>();

        userRepo.GetUserById(UserId).Returns(MakeUser());
        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);

        var useCase = new SaveProfileUseCase(userRepo, profileRepo, bodyMetricsRepo, unitOfWork);
        var result  = await useCase.ExecuteAsync(UserId, MakeRequest());

        await bodyMetricsRepo.Received(1).CreateMetrics(Arg.Any<BodyMetrics>());
        Assert.True(result.BMI > 0);
        Assert.True(result.TDEE > 0);
        Assert.True(result.CalorieGoal > 0);
    }

    // ── GetUserProfileUseCase ──────────────────────────────────────────────

    [Fact]
    public async Task GetProfile_RetornaResponseMapeado()
    {
        var profileRepo     = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo = Substitute.For<IBodyMetricsRepository>();

        profileRepo.GetByUserId(UserId).Returns(MakeProfile());
        bodyMetricsRepo.GetLatestByUserId(UserId).Returns(MakeBodyMetrics());

        var useCase = new GetUserProfileUseCase(profileRepo, bodyMetricsRepo);
        var result  = await useCase.ExecuteAsync(UserId);

        Assert.Equal(UserId, result.UserId);
        Assert.Equal(175, result.HeightCm);
        Assert.Equal(75, result.WeightKg);
        Assert.Contains("Running", result.TrainingTypes);
    }

    [Fact]
    public async Task GetProfile_SemPerfil_LancaNotFoundException()
    {
        var profileRepo     = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo = Substitute.For<IBodyMetricsRepository>();

        profileRepo.GetByUserId(UserId).Returns((UserProfile?)null);

        var useCase = new GetUserProfileUseCase(profileRepo, bodyMetricsRepo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(UserId));
    }

    [Fact]
    public async Task GetProfile_SemBodyMetrics_LancaNotFoundException()
    {
        var profileRepo     = Substitute.For<IUserProfileRepository>();
        var bodyMetricsRepo = Substitute.For<IBodyMetricsRepository>();

        profileRepo.GetByUserId(UserId).Returns(MakeProfile());
        bodyMetricsRepo.GetLatestByUserId(UserId).Returns((BodyMetrics?)null);

        var useCase = new GetUserProfileUseCase(profileRepo, bodyMetricsRepo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(UserId));
    }
}
