using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Services;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class GetAllPersonalRangeUseCase(
    IPersonalRangeRepository personalRangeRepository
) : IGetAllPersonalRangeUseCase
{
    public async Task<List<PersonalRangeResponse>> ExecuteAsync(Guid userId)
    {
        var ranges = await personalRangeRepository.GetByUserId(userId);

        return ranges.Select(x => new PersonalRangeResponse(
            x.Id,
            x.MetricTypeId,
            x.MetricType.Name,
            x.MetricType.Unit,
            x.MinNormal,
            x.MaxNormal,
            x.Method.ToString(),
            x.CalculatedAt
        )).ToList();
    }
}