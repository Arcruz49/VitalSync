using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Enums;
using VitalSyncAPI.Domain.Services;

namespace VitalSyncAPI.Tests;

public class PersonalRangeCalculatorTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static UserProfile DefaultProfile() => new()
    {
        HeightCm = 170,
        ActivityLevel = ActivityLevel.ModeratelyActive,
        Goal = HealthGoal.Maintenance,
        TrainingFrequencyDays = 3,
        HabitualSleepHours = 7,
    };

    private static MetricType MetricWith(int id, string name = "Métrica", double? min = null, double? max = null) => new()
    {
        Id = id,
        Name = name,
        Unit = "u",
        MinNormal = min,
        MaxNormal = max,
    };

    private static UserCondition Condition(int conditionId) => new() { ConditionId = conditionId };
    private static UserMedication Medication(MedicationClass cls) => new() { MedicationClass = cls };

    private List<PersonalRange> Calculate(
        UserProfile? profile = null,
        List<UserCondition>? conditions = null,
        List<UserMedication>? medications = null,
        List<MetricType>? metricTypes = null)
    {
        return PersonalRangeCalculator.Calculate(
            UserId,
            profile ?? DefaultProfile(),
            conditions ?? [],
            medications ?? [],
            metricTypes ?? []);
    }

    // ── Frequência Cardíaca (id=4) ─────────────────────────────────────────

    [Fact]
    public void FrequenciaCardiaca_PerfilPadrao_RetornaFaixa60a100()
    {
        var result = Calculate(metricTypes: [MetricWith(4)]);

        var range = Assert.Single(result);
        Assert.Equal(60, range.MinNormal);
        Assert.Equal(100, range.MaxNormal);
        Assert.Equal(RangeMethod.Default, range.Method);
    }

    [Fact]
    public void FrequenciaCardiaca_ComBetaBlockers_RetornaFaixa40a70()
    {
        var result = Calculate(
            medications: [Medication(MedicationClass.BetaBlockers)],
            metricTypes: [MetricWith(4)]);

        var range = Assert.Single(result);
        Assert.Equal(40, range.MinNormal);
        Assert.Equal(70, range.MaxNormal);
        Assert.Equal(RangeMethod.MedicationBased, range.Method);
    }

    [Fact]
    public void FrequenciaCardiaca_Atleta5DiasOuMais_RetornaFaixa40a70()
    {
        var profile = DefaultProfile();
        profile.TrainingFrequencyDays = 5;

        var result = Calculate(profile: profile, metricTypes: [MetricWith(4)]);

        var range = Assert.Single(result);
        Assert.Equal(40, range.MinNormal);
        Assert.Equal(70, range.MaxNormal);
        Assert.Equal(RangeMethod.AthleteAdjusted, range.Method);
    }

    [Fact]
    public void FrequenciaCardiaca_BetaBlockersTemPrioridadeSobreAtleta()
    {
        var profile = DefaultProfile();
        profile.TrainingFrequencyDays = 6;

        var result = Calculate(
            profile: profile,
            medications: [Medication(MedicationClass.BetaBlockers)],
            metricTypes: [MetricWith(4)]);

        var range = Assert.Single(result);
        Assert.Equal(RangeMethod.MedicationBased, range.Method);
    }

    // ── Pressão Sistólica (id=1) ────────────────────────────────────────────

    [Fact]
    public void PressaoSistolica_PerfilPadrao_RetornaFaixa90a120()
    {
        var result = Calculate(metricTypes: [MetricWith(1)]);

        var range = Assert.Single(result);
        Assert.Equal(90, range.MinNormal);
        Assert.Equal(120, range.MaxNormal);
    }

    [Fact]
    public void PressaoSistolica_ComAntihipertensivos_RetornaFaixa90a130()
    {
        var result = Calculate(
            medications: [Medication(MedicationClass.Antihypertensives)],
            metricTypes: [MetricWith(1)]);

        var range = Assert.Single(result);
        Assert.Equal(130, range.MaxNormal);
        Assert.Equal(RangeMethod.MedicationBased, range.Method);
    }

    [Fact]
    public void PressaoSistolica_ComHipertensao_RetornaFaixa90a140()
    {
        var result = Calculate(
            conditions: [Condition(1)],
            metricTypes: [MetricWith(1)]);

        var range = Assert.Single(result);
        Assert.Equal(140, range.MaxNormal);
        Assert.Equal(RangeMethod.ConditionBased, range.Method);
    }

    // ── Pressão Diastólica (id=2) ───────────────────────────────────────────

    [Fact]
    public void PressaoDiastolica_PerfilPadrao_RetornaFaixa60a80()
    {
        var result = Calculate(metricTypes: [MetricWith(2)]);

        var range = Assert.Single(result);
        Assert.Equal(60, range.MinNormal);
        Assert.Equal(80, range.MaxNormal);
    }

    [Fact]
    public void PressaoDiastolica_ComAntihipertensivos_RetornaFaixa60a85()
    {
        var result = Calculate(
            medications: [Medication(MedicationClass.Antihypertensives)],
            metricTypes: [MetricWith(2)]);

        var range = Assert.Single(result);
        Assert.Equal(85, range.MaxNormal);
    }

    // ── Glicemia (id=3) ────────────────────────────────────────────────────

    [Fact]
    public void Glicemia_PerfilPadrao_RetornaFaixa70a99()
    {
        var result = Calculate(metricTypes: [MetricWith(3)]);

        var range = Assert.Single(result);
        Assert.Equal(70, range.MinNormal);
        Assert.Equal(99, range.MaxNormal);
    }

    [Fact]
    public void Glicemia_ComDiabetesTipo1_RetornaFaixa80a130()
    {
        var result = Calculate(
            conditions: [Condition(2)],
            metricTypes: [MetricWith(3)]);

        var range = Assert.Single(result);
        Assert.Equal(80, range.MinNormal);
        Assert.Equal(130, range.MaxNormal);
        Assert.Equal(RangeMethod.ConditionBased, range.Method);
    }

    [Fact]
    public void Glicemia_ComDiabetesTipo2_RetornaFaixa80a130()
    {
        var result = Calculate(
            conditions: [Condition(3)],
            metricTypes: [MetricWith(3)]);

        var range = Assert.Single(result);
        Assert.Equal(130, range.MaxNormal);
    }

    [Fact]
    public void Glicemia_ComPreDiabetes_RetornaFaixa70a110()
    {
        var result = Calculate(
            conditions: [Condition(4)],
            metricTypes: [MetricWith(3)]);

        var range = Assert.Single(result);
        Assert.Equal(110, range.MaxNormal);
    }

    // ── Sono (id=7) ────────────────────────────────────────────────────────

    [Fact]
    public void Sono_HabitualMenorQue6Horas_RetornaFaixa6a9()
    {
        var profile = DefaultProfile();
        profile.HabitualSleepHours = 5;

        var result = Calculate(profile: profile, metricTypes: [MetricWith(7)]);

        var range = Assert.Single(result);
        Assert.Equal(6, range.MinNormal);
        Assert.Equal(9, range.MaxNormal);
    }

    [Fact]
    public void Sono_HabitualIgualOuMaiorQue6Horas_RetornaFaixa7a9()
    {
        var profile = DefaultProfile();
        profile.HabitualSleepHours = 6;

        var result = Calculate(profile: profile, metricTypes: [MetricWith(7)]);

        var range = Assert.Single(result);
        Assert.Equal(7, range.MinNormal);
        Assert.Equal(9, range.MaxNormal);
    }

    // ── Peso (id=5) ────────────────────────────────────────────────────────

    [Fact]
    public void Peso_CalculadoPorAltura170cm_FaixaCorreta()
    {
        // heightM = 1.70, min = 18.5 * 1.70^2 = 53.5, max = 24.9 * 1.70^2 = 72.0
        var result = Calculate(metricTypes: [MetricWith(5)]);

        var range = Assert.Single(result);
        Assert.Equal(53.5m, range.MinNormal);
        Assert.Equal(72.0m, range.MaxNormal);
        Assert.Equal(RangeMethod.AgeAdjusted, range.Method);
    }

    // ── Case default: métricas genéricas ──────────────────────────────────

    [Fact]
    public void MetricaGenerica_ComAmbosLimitesDefinidos_EhIncluida()
    {
        var result = Calculate(metricTypes: [MetricWith(99, min: 10, max: 50)]);

        var range = Assert.Single(result);
        Assert.Equal(10, range.MinNormal);
        Assert.Equal(50, range.MaxNormal);
    }

    [Fact]
    public void MetricaGenerica_ComAmbosLimitesNulos_NaoEhIncluida()
    {
        // Métricas sem faixa definida (ex: passos) não devem gerar PersonalRange
        var result = Calculate(metricTypes: [MetricWith(99, min: null, max: null)]);

        Assert.Empty(result);
    }

    [Fact]
    public void MetricaGenerica_ComSomenteMaxNulo_NaoEhIncluida()
    {
        // Bug fix: MinNormal definido mas MaxNormal nulo não deve criar range com max=0
        var result = Calculate(metricTypes: [MetricWith(99, min: 5000, max: null)]);

        Assert.Empty(result);
    }

    [Fact]
    public void MetricaGenerica_ComSomenteMinNulo_NaoEhIncluida()
    {
        var result = Calculate(metricTypes: [MetricWith(99, min: null, max: 100)]);

        Assert.Empty(result);
    }

    // ── Múltiplas métricas ─────────────────────────────────────────────────

    [Fact]
    public void MultiplasMétricas_RetornaUmRangePorMetrica()
    {
        var metrics = new List<MetricType>
        {
            MetricWith(1),  // sistólica
            MetricWith(2),  // diastólica
            MetricWith(4),  // FC
        };

        var result = Calculate(metricTypes: metrics);

        Assert.Equal(3, result.Count);
        Assert.All(result, r => Assert.Equal(UserId, r.UserId));
    }

    [Fact]
    public void TodasAsRanges_PossuemUserIdCorreto()
    {
        var result = Calculate(metricTypes: [MetricWith(1), MetricWith(4)]);

        Assert.All(result, r => Assert.Equal(UserId, r.UserId));
    }
}
