using System.ComponentModel.DataAnnotations;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.UseCases;

public class EditHealthRecordUseCase (IHealthRecordsRepository recordRepository, IUnitOfWork unitOfWork) : IEditHealthRecordUseCase{

   
    public async Task ExecuteAsync(Guid userId, Guid id, HealthRecordRequest request)
    {
        var originalRecord = await recordRepository.GetById(id);

        if(originalRecord.UserId != userId) throw new ForbiddenException("Você não tem permissão para editar este registro.");

        originalRecord.MetricTypeId = request.MetricTypeId;
        originalRecord.Value = request.Value;
        originalRecord.MeasuredAt = request.MeasuredAt;
        originalRecord.Notes = request.Notes;

        recordRepository.Edit(originalRecord);
        await unitOfWork.SaveChangesAsync();
    }

}
