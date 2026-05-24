using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Interfaces;

namespace VitalSyncAPI.Controllers;

[ApiController]
[Authorize]
[Route("health-record")]
public class HealthRecordController(ICreateHealthRecordUseCase createHealthRecordUseCase, IDeleteHealthRecord deleteHealthRecord,
        IEditHealthRecordUseCase editHealthRecordUseCase, IGetHealthRecordById getHealthRecordById, IGetHealthRecordByUser getHealthRecordByUser) : BaseController
{

    [HttpGet]
    public async Task<IActionResult> GetHealthRecordById(Guid id)
    {
        var result = await getHealthRecordById.ExecuteAsync(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetHealthRecordByUser([FromQuery] SearchHealthRecordRequest request)
    {
        var result = await getHealthRecordByUser.ExecuteAsync(UserId, request.MetricTypeId, request.From, request.To);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateHealthRecord(HealthRecordRequest request)
    {
        await createHealthRecordUseCase.ExecuteAsync(UserId, request);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHealthRecord(Guid id, HealthRecordRequest request)
    {
        await editHealthRecordUseCase.ExecuteAsync(id, request);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHealthRecord(Guid id)
    {
        await deleteHealthRecord.ExecuteAsync(id);
        return NoContent();
    }

}
