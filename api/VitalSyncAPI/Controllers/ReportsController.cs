using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Interfaces;

namespace VitalSyncAPI.Controllers;

[ApiController]
[Authorize]
[Route("reports")]
public class ReportsController(
    IAddWeeklyReportUseCase generateWeeklyReportUseCase,
    IGetWeeklyReportsByIdUseCase getWeeklyReportsByIdUseCase,
    IGetWeeklyReportsUseCase getWeeklyReportsUseCase
) : BaseController
{
    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] WeeklyReportRequest request)
    {
        var result = await generateWeeklyReportUseCase.ExecuteAsync(UserId, request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await getWeeklyReportsUseCase.ExecuteAsync(UserId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await getWeeklyReportsByIdUseCase.ExecuteAsync(id, UserId);
        return Ok(result);
    }
}