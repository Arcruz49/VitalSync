using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Interfaces;

namespace VitalSyncAPI.Controllers;

[ApiController]
[Route("auth")]
public class MetricTypesController : ControllerBase
{
    private readonly IAuthenticateUseCase _authenticateUseCase;
    private readonly IRegisterUserUseCase _registerUserUseCase;

    public MetricTypesController(IAuthenticateUseCase authenticateUseCase, IRegisterUserUseCase registerUserUseCase)
    {
        _authenticateUseCase = authenticateUseCase;
        _registerUserUseCase = registerUserUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authenticateUseCase.ExecuteAsync(request);

        var isHttps = Request.IsHttps;
        Response.Cookies.Append("vitalsync_token", result.token, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(60)
        });
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var result = await _registerUserUseCase.ExecuteAsync(request);

        var isHttps = Request.IsHttps;
        Response.Cookies.Append("vitalsync_token", result.token, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(60)
        });
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var name = User.FindFirstValue(ClaimTypes.Name);
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Ok(new { id, name });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var isHttps = Request.IsHttps;
        Response.Cookies.Append("vitalsync_token", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(-1)
        });

        return Ok();
    }
}
