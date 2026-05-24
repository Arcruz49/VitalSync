using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Interfaces;

namespace VitalSyncAPI.Controllers;

[ApiController]
[Authorize]
[Route("metrics")]
public class MetricTypesController : ControllerBase
{
    private readonly IGetMetricTypes _getMetricTypes;

    public MetricTypesController(IGetMetricTypes getMetricTypes)
    {
        _getMetricTypes = getMetricTypes;
    }

    [HttpGet]
    public async Task<IActionResult> GetMetricTypes()
    {
        var result = await _getMetricTypes.ExecuteAsync();
        return Ok(result);
    }

    

}
