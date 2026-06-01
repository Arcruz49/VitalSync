using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Services;

public static class AlertGenerator
{
    public static Alert? Generate(HealthRecord record, MetricType metricType, decimal? minNormal, decimal? maxNormal)
    {
        if (minNormal is null && maxNormal is null) return null;

        var aboveMax = maxNormal.HasValue && record.Value > maxNormal.Value;
        var belowMin = minNormal.HasValue && record.Value < minNormal.Value;

        if (!aboveMax && !belowMin) return null;

        var severity = CalculateSeverity(record.Value, minNormal, maxNormal, aboveMax);
        var message = BuildAlertMessage(record.Value, metricType, aboveMax, minNormal, maxNormal);

        return new Alert
        {
            Id = Guid.NewGuid(),
            UserId = record.UserId,
            HealthRecordId = record.Id,
            MetricTypeId = record.MetricTypeId,
            Severity = severity,
            Message = message,
            TriggeredAt = DateTime.UtcNow
        };
    }

    private static string BuildAlertMessage(decimal value, MetricType metricType, bool isAbove, decimal? minNormal, decimal? maxNormal)
    {
        var direction = isAbove ? "acima" : "abaixo";
        var limit = isAbove ? maxNormal : minNormal;

        return $"{metricType.Name} registrada {direction} do normal: " +
            $"{value} {metricType.Unit} (limite: {limit} {metricType.Unit}).";
    }

    private static AlertSeverity CalculateSeverity(decimal value, decimal? minNormal, decimal? maxNormal, bool isAbove)
    {
        var limit = isAbove ? maxNormal!.Value : minNormal!.Value;
        var deviation = Math.Abs((value - limit) / limit * 100);
        return deviation >= 20 ? AlertSeverity.Critical : AlertSeverity.Warning;
    }
}