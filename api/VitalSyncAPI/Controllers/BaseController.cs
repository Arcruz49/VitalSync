using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace VitalSyncAPI.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected Guid UserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Claim 'NameIdentifier' ausente."));

    protected string UserName =>
        User.FindFirstValue(ClaimTypes.Name)
            ?? throw new UnauthorizedAccessException("Claim 'Name' ausente.");

}