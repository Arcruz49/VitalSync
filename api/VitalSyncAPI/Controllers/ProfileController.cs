using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Interfaces;

namespace VitalSyncAPI.Controllers;

[ApiController]
[Authorize]
[Route("profile")]
public class ProfileController(
    ISaveProfileUseCase saveProfileUseCase,
    IGetUserProfileUseCase getUserProfileUseCase,
    IGetAllUserProfileUseCase getAllUserProfileUseCase,
    IGetUserConditionUseCase getUserConditionUseCase,
    IAddUserConditionUseCase addUserConditionUseCase,
    IAddUserMedicationUseCase addUserMedicationUseCase,
    IGetUserMedicationUseCase getUserMedicationUseCase
    ) : BaseController
{

    // profile
    [HttpPost]
    public async Task<IActionResult> SaveProfile([FromBody] UserProfileRequest request)
    {
        var result = await saveProfileUseCase.ExecuteAsync(UserId, request);
        return Created($"/profile", result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await getUserProfileUseCase.ExecuteAsync(UserId);
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetProfileHistory()
    {
        var result = await getAllUserProfileUseCase.ExecuteAsync(UserId);
        return Ok(result);
    }

    // conditions
    [HttpGet("conditions")]
    public async Task<IActionResult> GetConditions()
    {
        var result = await getUserConditionUseCase.ExecuteAsync(UserId);
        return Ok(result);
    }

    [HttpPost("conditions")]
    public async Task<IActionResult> AddConditions([FromBody] List<UserConditionRequest> request)
    {
        await addUserConditionUseCase.ExecuteAsync(UserId, request);
        return NoContent();
    }

    // medications
    [HttpGet("medications")]
    public async Task<IActionResult> GetMedications()
    {
        var result = await getUserMedicationUseCase.ExecuteAsync(UserId);
        return Ok(result);
    }

    [HttpPost("medications")]
    public async Task<IActionResult> AddMedications([FromBody] List<UserMedicationRequest> request)
    {
        await addUserMedicationUseCase.ExecuteAsync(UserId, request);
        return NoContent();
    }
}