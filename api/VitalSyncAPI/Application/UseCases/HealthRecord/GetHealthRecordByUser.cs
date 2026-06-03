using System.Text.Json;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Application.Models;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.UseCases;

public class GetHealthRecordByUser(IHealthRecordsRepository recordRepository) : IGetHealthRecordByUser
{
    public async Task<List<HealthRecordResponse>> ExecuteAsync(Guid userId, int? metricTypeId, DateTime? from, DateTime? to)
    {
        var records = await recordRepository.GetByUserAsync(userId, metricTypeId, from, to);

        return records.Select(record =>
        {
            var insight = record.AIInsight.FirstOrDefault();
            AIAnalysisResult? analysisResult = insight is not null
                ? new AIAnalysisResult
                  {
                      Insights = JsonSerializer.Deserialize<List<string>>(insight.Insights) ?? [],
                      Tips = JsonSerializer.Deserialize<List<string>>(insight.Tips) ?? [],
                      OverallAssessment = insight.OverallAssessment,
                      Disclaimer = insight.Disclaimer,
                  }
                : null;

            return new HealthRecordResponse(
                record.Id,
                record.MetricTypeId,
                record.MetricType.Name,
                record.MetricType.Unit,
                record.Value,
                record.MeasuredAt,
                record.Notes,
                record.CreatedAt,
                analysisResult
            );
        }).ToList();
    }
}