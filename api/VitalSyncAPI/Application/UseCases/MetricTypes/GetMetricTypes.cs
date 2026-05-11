using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.UseCases;

public class GetMetricTypes : IGetMetricTypes
{
    private readonly IMetricTypesRepository _metricTypesRepository;

    public GetMetricTypes(IMetricTypesRepository metricTypesRepository)
    {
        _metricTypesRepository = metricTypesRepository;
    }

    public async Task<List<MetricTypeResponse>> ExecuteAsync()
    {
        var result = await _metricTypesRepository.GetAll();

        return result.Select(x => new MetricTypeResponse(
            x.Id,
            x.Name,
            x.Unit,
            x.MinNormal,
            x.MaxNormal,
            x.Icon,
            x.SortOrder
        )).ToList();
    }
}