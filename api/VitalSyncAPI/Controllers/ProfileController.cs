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
    IGetAllUserProfileUseCase getAllUserProfileUseCase) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> SaveProfile(UserProfileRequest request)
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
}