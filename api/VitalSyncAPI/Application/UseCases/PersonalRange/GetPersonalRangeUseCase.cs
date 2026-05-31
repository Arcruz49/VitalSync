using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Services;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class GetPersonalRangeUseCase(
    IPersonalRangeRepository personalRangeRepository
) : IGetPersonalRangeUseCase
{
    public async Task<PersonalRangeResponse> ExecuteAsync(Guid userId, int metricTypeId)
    {
        var range = await personalRangeRepository.GetByUserIdAndMetricId(userId, metricTypeId);

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