namespace VitalSyncAPI.Application.DTOs.Request;
public class ForgotPasswordRequest()
{
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}