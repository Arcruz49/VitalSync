using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Interfaces;

namespace VitalSyncAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : BaseController
{
    private readonly IAuthenticateUseCase _authenticateUseCase;
    private readonly IRegisterUserUseCase _registerUserUseCase;
    private readonly ISendEmailForgotPasswordUseCase _sendEmailForgotPasswordUseCase;
    private readonly IResetPasswordUseCase _resetPasswordUseCase;
    private readonly IDeleteUserDataUseCase _deleteUserDataUseCase;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticateUseCase authenticateUseCase, IRegisterUserUseCase registerUserUseCase,
    ISendEmailForgotPasswordUseCase sendEmailForgotPasswordUseCase, ILogger<AuthController> logger, IResetPasswordUseCase resetPasswordUseCase,
    IDeleteUserDataUseCase deleteUserDataUseCase, IServiceScopeFactory scopeFactory)
    {
        _authenticateUseCase = authenticateUseCase;
        _registerUserUseCase = registerUserUseCase;
        _sendEmailForgotPasswordUseCase = sendEmailForgotPasswordUseCase;
        _resetPasswordUseCase = resetPasswordUseCase;
        _deleteUserDataUseCase = deleteUserDataUseCase;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    [EnableRateLimiting("login")]
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

    [EnableRateLimiting("register")]
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

    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword(string email)
    {
        _ = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<ISendEmailForgotPasswordUseCase>();
            try
            {
                await useCase.ExecuteAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar recuperação de senha para {Email}", email);
            }
        });

        return Ok("Se o seu email estiver cadastrado, você receberá um email de recuperação de senha.");
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ForgotPasswordRequest request)
    {
        await _resetPasswordUseCase.ExecuteAsync(request.Token, request.Password);
        return Ok();
    }

    [Authorize]
    [HttpDelete()]
    public async Task<IActionResult> DeleteUserData()
    {
        await _deleteUserDataUseCase.ExecuteAsync(UserId);
        return NoContent();
    }
}
