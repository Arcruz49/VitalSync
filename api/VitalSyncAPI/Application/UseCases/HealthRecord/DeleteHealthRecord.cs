using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.UseCases;

public class DeleteHealthRecord (IHealthRecordsRepository recordRepository, IUnitOfWork unitOfWork) : IDeleteHealthRecord{

   
    public async Task ExecuteAsync(Guid userId, Guid id)
    {
        var record = await recordRepository.GetById(id);
        if(record.UserId != userId) throw new ForbiddenException("Você não tem permissão para deletar este registro.");
        
        await recordRepository.Delete(id);
        await unitOfWork.SaveChangesAsync();

    }

}
