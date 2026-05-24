using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IDeleteHealthRecord
{
    Task ExecuteAsync(Guid userId, Guid id);
}