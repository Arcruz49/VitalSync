using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Services;

public static class PersonalRangeCalculator
{
    public static List<PersonalRange> Calculate(
        Guid userId,
        UserProfile profile,
        List<UserCondition> conditions,
        List<UserMedication> medications,
        List<MetricType> metricTypes)
    {
        var ranges = new List<PersonalRange>();
        var conditionIds = conditions.Select(c => c.ConditionId).ToList();
        var medicationClasses = medications.Select(m => m.MedicationClass).ToList();

        foreach (var metricType in metricTypes)
        {
            var (min, max, method) = metricType.Id switch
            {
                // Frequência Cardíaca
                4 => CalculateHeartRate(profile, medicationClasses),
                // Pressão Sistólica
                1 => CalculateSystolic(profile, conditionIds, medicationClasses),
                // Pressão Diastólica
                2 => CalculateDiastolic(profile, conditionIds, medicationClasses),
                // Glicemia em jejum
                3 => CalculateGlucose(conditionIds),
                // Sono
                7 => CalculateSleep(profile),
                // peso
                5 => CalculateWeightRange(profile),
                // Padrão — usa os valores do MetricType
                _ => (metricType.MinNormal ?? 0, metricType.MaxNormal ?? 0, RangeMethod.Default)
            };

            // Não cria range para métricas sem faixa definida
            if (min == 0 && max == 0) continue;

            ranges.Add(new PersonalRange
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MetricTypeId = metricType.Id,
                MinNormal = (decimal)min,
                MaxNormal = (decimal)max,
                Method = method,
                CalculatedAt = DateTime.UtcNow
            });
        }

        return ranges;
    }

    private static (double min, double max, RangeMethod method) CalculateHeartRate(
        UserProfile profile, List<MedicationClass> medications)
    {
        // Beta-bloqueadores reduzem a FC em repouso
        if (medications.Contains(MedicationClass.BetaBlockers))
            return (40, 70, RangeMethod.MedicationBased);

        // Atletas têm FC em repouso mais baixa
        if (profile.TrainingFrequencyDays >= 5)
            return (40, 70, RangeMethod.AthleteAdjusted);

        return (60, 100, RangeMethod.Default);
    }

    private static (double min, double max, RangeMethod method) CalculateSystolic(
        UserProfile profile, List<int> conditionIds, List<MedicationClass> medications)
    {
        // Anti-hipertensivos — faixa esperada mais baixa
        if (medications.Contains(MedicationClass.Antihypertensives))
            return (90, 130, RangeMethod.MedicationBased);

        // Hipertensão sem medicamento — faixa mais tolerante
        if (conditionIds.Contains(1))
            return (90, 140, RangeMethod.ConditionBased);

        return (90, 120, RangeMethod.Default);
    }

    private static (double min, double max, RangeMethod method) CalculateDiastolic(
        UserProfile profile, List<int> conditionIds, List<MedicationClass> medications)
    {
        if (medications.Contains(MedicationClass.Antihypertensives))
            return (60, 85, RangeMethod.MedicationBased);

        if (conditionIds.Contains(1))
            return (60, 90, RangeMethod.ConditionBased);

        return (60, 80, RangeMethod.Default);
    }

    private static (double min, double max, RangeMethod method) CalculateGlucose(List<int> conditionIds)
    {
        // Diabetes tipo 1 ou 2
        if (conditionIds.Contains(2) || conditionIds.Contains(3))
            return (80, 130, RangeMethod.ConditionBased);

        // Pré-diabetes
        if (conditionIds.Contains(4))
            return (70, 110, RangeMethod.ConditionBased);

        return (70, 99, RangeMethod.Default);
    }

    private static (double min, double max, RangeMethod method) CalculateSleep(UserProfile profile)
    {
        // Recomendação varia por idade (via BirthDate no User — aqui usa HabitualSleepHours como base)
        return profile.HabitualSleepHours < 6
            ? (6, 9, RangeMethod.Default)
            : (7, 9, RangeMethod.Default);
    }

    private static (double min, double max, RangeMethod method) CalculateWeightRange(UserProfile profile)
    {
        var heightM = (double)(profile.HeightCm / 100);
        return (
            Math.Round(heightM * heightM * 18.5, 1),
            Math.Round(heightM * heightM * 24.9, 1),
            RangeMethod.AgeAdjusted
        );
    }
}