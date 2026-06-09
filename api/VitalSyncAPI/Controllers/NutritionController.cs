using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Interfaces;

namespace VitalSyncAPI.Controllers;

[ApiController]
[Authorize]
[EnableRateLimiting("global")]
[Route("nutrition")]
public class NutritionController(
    IAddNutritionRecordUseCase addNutritionRecordUseCase,
    IUpdateNutritionRecordUseCase updateNutritionRecordUseCase,
    IDeleteNutritionRecordUseCase deleteNutritionRecordUseCase,
    IGetAllNutritionRecordUseCase getAllNutritionRecordsUseCase,
    IGetNutritionRecordUseCase getNutritionRecordUseCase,
    IGetNutritionSummaryUseCase getNutritionSummaryUseCase
) : BaseController
{
    [HttpPost]
    [EnableRateLimiting("ai-limit-image")]
    public async Task<IActionResult> AddNutritionRecord([FromBody] NutritionRequest request)
    {
        var result = await addNutritionRecordUseCase.ExecuteAsync(UserId, request);
        return CreatedAtAction(nameof(GetNutritionRecord), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetNutritionRecords([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await getAllNutritionRecordsUseCase.ExecuteAsync(UserId, from, to);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNutritionRecord(Guid id)
    {
        var result = await getNutritionRecordUseCase.ExecuteAsync(UserId, id);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNutritionRecord(Guid id, [FromBody] NutritionRequest request)
    {
        var result = await updateNutritionRecordUseCase.ExecuteAsync(UserId, id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNutritionRecord(Guid id)
    {
        await deleteNutritionRecordUseCase.ExecuteAsync(UserId, id);
        return NoContent();
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetNutritionSummary([FromQuery] DateTime? date)
    {
        var result = await getNutritionSummaryUseCase.ExecuteAsync(UserId, date ?? DateTime.UtcNow.Date);
        return Ok(result);
    }
}