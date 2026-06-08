using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Enums;
using VitalSyncAPI.Domain.Services;

namespace VitalSyncAPI.Tests;

public class BodyMetricsCalculatorTests
{
    // Usuário com 30 anos garantidos (Jan 1 há exatamente 30 anos)
    private static User MakeUser(string gender = "M") => new()
    {
        Id = Guid.NewGuid(),
        Name = "Test",
        Email = "test@test.com",
        Gender = gender,
        BirthDate = new DateTime(DateTime.UtcNow.Year - 30, 1, 1),
    };

    private static UserProfile MakeProfile(
        decimal heightCm = 170,
        ActivityLevel activity = ActivityLevel.Sedentary,
        HealthGoal goal = HealthGoal.Maintenance) => new()
    {
        HeightCm = heightCm,
        ActivityLevel = activity,
        Goal = goal,
        TrainingFrequencyDays = 3,
        HabitualSleepHours = 7,
    };

    // ── BMI ────────────────────────────────────────────────────────────────

    [Fact]
    public void BMI_70kgE170cm_Retorna24_22()
    {
        // 70 / (1.70 * 1.70) = 24.22
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(), 70);

        Assert.Equal(24.22m, result.BMI);
    }

    [Fact]
    public void BMI_AumentaComPeso()
    {
        var leve = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(), 60);
        var pesado = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(), 90);

        Assert.True(pesado.BMI > leve.BMI);
    }

    [Fact]
    public void BMI_DiminuiComAltura()
    {
        var baixo = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(heightCm: 160), 70);
        var alto = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(heightCm: 190), 70);

        Assert.True(alto.BMI < baixo.BMI);
    }

    // ── BMR ───────────────────────────────────────────────────────────────

    [Fact]
    public void BMR_Masculino_MaiorQueFeминino_MesmosAtributos()
    {
        // Mifflin-St Jeor: masculino +5, feminino -161 (diferença de 166)
        var male = BodyMetricsCalculator.Calculate(MakeUser("M"), MakeProfile(), 70);
        var female = BodyMetricsCalculator.Calculate(MakeUser("F"), MakeProfile(), 70);

        Assert.True(male.BMR > female.BMR);
        Assert.Equal(166m, male.BMR - female.BMR);
    }

    [Fact]
    public void BMR_Masculino70kg170cm30anos_Retorna1617_5()
    {
        // 10*70 + 6.25*170 - 5*30 + 5 = 700 + 1062.5 - 150 + 5 = 1617.5
        var result = BodyMetricsCalculator.Calculate(MakeUser("M"), MakeProfile(), 70);

        Assert.Equal(1617.5m, result.BMR);
    }

    [Fact]
    public void BMR_Feminino70kg170cm30anos_Retorna1451_5()
    {
        // 10*70 + 6.25*170 - 5*30 - 161 = 1451.5
        var result = BodyMetricsCalculator.Calculate(MakeUser("F"), MakeProfile(), 70);

        Assert.Equal(1451.5m, result.BMR);
    }

    // ── TDEE (fator de atividade) ──────────────────────────────────────────

    [Theory]
    [InlineData(ActivityLevel.Sedentary,        1.20)]
    [InlineData(ActivityLevel.LightlyActive,    1.375)]
    [InlineData(ActivityLevel.ModeratelyActive, 1.55)]
    [InlineData(ActivityLevel.VeryActive,       1.725)]
    [InlineData(ActivityLevel.ExtremelyActive,  1.90)]
    public void TDEE_FatorDeAtividadeAplicadoCorretamente(ActivityLevel level, double factor)
    {
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(activity: level), 70);

        // BMR = 1617.5 para M/30anos/70kg/170cm
        var expectedTdee = Math.Round(1617.5 * factor, 2);
        Assert.Equal((decimal)expectedTdee, result.TDEE);
    }

    [Fact]
    public void TDEE_AumentaComNivelDeAtividade()
    {
        var sedentary = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(activity: ActivityLevel.Sedentary), 70);
        var veryActive = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(activity: ActivityLevel.VeryActive), 70);

        Assert.True(veryActive.TDEE > sedentary.TDEE);
    }

    // ── Meta calórica por objetivo ─────────────────────────────────────────

    [Fact]
    public void MetaCalorica_Manutencao_IgualAoTDEE()
    {
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(goal: HealthGoal.Maintenance), 70);

        Assert.Equal(result.TDEE, result.CalorieGoal);
    }

    [Fact]
    public void MetaCalorica_PerdeKg_TDEEMenos500()
    {
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(goal: HealthGoal.WeightLoss), 70);

        Assert.Equal(result.TDEE - 500, result.CalorieGoal);
    }

    [Fact]
    public void MetaCalorica_GanhoMuscular_TDEEMais250()
    {
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(goal: HealthGoal.MuscleGain), 70);

        Assert.Equal(result.TDEE + 250, result.CalorieGoal);
    }

    [Fact]
    public void MetaCalorica_PerderKg_MenorQueManutencao_MenorQueGanhoMuscular()
    {
        var loss = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(goal: HealthGoal.WeightLoss), 70);
        var maint = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(goal: HealthGoal.Maintenance), 70);
        var gain = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(goal: HealthGoal.MuscleGain), 70);

        Assert.True(loss.CalorieGoal < maint.CalorieGoal);
        Assert.True(maint.CalorieGoal < gain.CalorieGoal);
    }

    // ── Macros ────────────────────────────────────────────────────────────

    [Fact]
    public void Macros_GanhoMuscular_ProteinaMaiorQueManutencao()
    {
        // GanhoMuscular usa 1.8g/kg vs 1.6g/kg dos outros objetivos
        var gain = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(goal: HealthGoal.MuscleGain), 70);
        var maint = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(goal: HealthGoal.Maintenance), 70);

        Assert.True(gain.ProteinGoalG > maint.ProteinGoalG);
    }

    [Fact]
    public void Macros_SaoPositivos()
    {
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(), 70);

        Assert.True(result.ProteinGoalG > 0);
        Assert.True(result.CarbsGoalG > 0);
        Assert.True(result.FatGoalG > 0);
    }

    [Fact]
    public void Macros_CaloriasConferem()
    {
        // proteína(4 cal/g) + carb(4 cal/g) + gordura(9 cal/g) ≈ meta calórica (tolerância 5 kcal por arredondamentos)
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(), 70);

        var totalCal = result.ProteinGoalG * 4 + result.CarbsGoalG * 4 + result.FatGoalG * 9;
        Assert.InRange(totalCal, result.CalorieGoal - 5, result.CalorieGoal + 5);
    }

    // ── Peso ideal ────────────────────────────────────────────────────────

    [Fact]
    public void PesoIdeal_170cm_Retorna53_5a72_0()
    {
        // IMC 18.5 a 24.9 com altura 1.70m: 53.465 e 71.961 → 53.5 e 72.0
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(heightCm: 170), 70);

        Assert.Equal(53.5m, result.IdealWeightMinKg);
        Assert.Equal(72.0m, result.IdealWeightMaxKg);
    }

    [Fact]
    public void PesoIdeal_MaxMaiorQueMin()
    {
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(), 70);

        Assert.True(result.IdealWeightMaxKg > result.IdealWeightMinKg);
    }

    // ── Campos gerais ─────────────────────────────────────────────────────

    [Fact]
    public void Calculate_PesoCopiadoCorretamente()
    {
        var result = BodyMetricsCalculator.Calculate(MakeUser(), MakeProfile(), 75.5m);

        Assert.Equal(75.5m, result.WeightKg);
    }

    [Fact]
    public void Calculate_UserIdCopiadoCorretamente()
    {
        var user = MakeUser();
        var result = BodyMetricsCalculator.Calculate(user, MakeProfile(), 70);

        Assert.Equal(user.Id, result.UserId);
    }
}
