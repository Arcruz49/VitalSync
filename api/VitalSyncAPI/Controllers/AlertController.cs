using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Controllers;

[ApiController]
[Authorize]
[EnableRateLimiting("global")]
[Route("alerts")]
public class AlertController(IGetAlertsUseCase getAlertsUseCase) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAlerts()
    {
        var result = await getAlertsUseCase.ExecuteAsync(UserId);

        return Ok(result);
    }
}
