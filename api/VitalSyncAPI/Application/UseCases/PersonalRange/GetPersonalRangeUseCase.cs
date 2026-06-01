using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class GetPersonalRangeUseCase(
    IPersonalRangeRepository personalRangeRepository
) : IGetPersonalRangeUseCase
{
    public async Task<PersonalRangeResponse> ExecuteAsync(Guid userId, int metricTypeId)
    {
        var range = await personalRangeRepository.GetByUserIdAndMetricId(userId, metricTypeId)
            ?? throw new NotFoundException("Faixa personalizada não encontrada para esta métrica.");

        return new PersonalRangeResponse(
            range.Id,
            range.MetricTypeId,
            range.MetricType.Name,
            range.MetricType.Unit,
            range.MinNormal,
            range.MaxNormal,
            range.Method.ToString(),
            range.CalculatedAt
        );
    }
}