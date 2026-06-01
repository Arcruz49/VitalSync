using System.ComponentModel.DataAnnotations;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Services;

namespace VitalSyncAPI.Application.UseCases;

public class EditHealthRecordUseCase(
    IHealthRecordsRepository recordRepository,
    IAlertRepository alertRepository,
    IMetricTypesRepository metricTypesRepository,
    IPersonalRangeRepository personalRangeRepository,
    IUnitOfWork unitOfWork) : IEditHealthRecordUseCase
{
    public async Task ExecuteAsync(Guid userId, Guid id, HealthRecordRequest request)
    {
        var originalRecord = await recordRepository.GetById(id);

        if (originalRecord.UserId != userId)
            throw new ForbiddenException("Você não tem permissão para editar este registro.");

        originalRecord.MetricTypeId = request.MetricTypeId;
        originalRecord.Value = request.Value;
        originalRecord.MeasuredAt = request.MeasuredAt;
        originalRecord.Notes = request.Notes;

        recordRepository.Edit(originalRecord);

        // remove o alerta que já existe no banco
        await alertRepository.DeleteByHealthRecordId(id);

        var metricType = await metricTypesRepository.GetById(request.MetricTypeId);
        var personalRange = await personalRangeRepository.GetByUserIdAndMetricId(userId, request.MetricTypeId);

        var newAlert = personalRange is not null
            ? AlertGenerator.Generate(originalRecord, metricType, personalRange.MinNormal, personalRange.MaxNormal)
            : AlertGenerator.Generate(originalRecord, metricType, (decimal?)metricType.MinNormal, (decimal?)metricType.MaxNormal);

        if (newAlert is not null)
            await alertRepository.AddAsync(newAlert);

        await unitOfWork.SaveChangesAsync();
    }
}