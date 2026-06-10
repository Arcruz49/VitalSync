using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using VitalSyncAPI.Application.Interfaces;

namespace VitalSyncAPI.Application.Services;

public class EmailService(IConfiguration config) : IEmailService
{
    private readonly string _host = config["Email:Host"] ?? throw new Exception("Email:Host not configured");
    private readonly string _user = config["Email:User"] ?? throw new Exception("Email:User not configured");
    private readonly string _password = config["Email:Password"] ?? throw new Exception("Email:Password not configured");
    private readonly string _from = config["Email:From"] ?? throw new Exception("Email:From not configured");
    private readonly int _port = int.Parse(config["Email:Port"] ?? "587");

    public async Task SendPasswordResetAsync(string toEmail, string token)
    {
        var resetLink = $"{config["App:BaseUrl"]}/reset-password?token={Uri.EscapeDataString(token)}";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("VitalSync", _from));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Recuperação de senha — VitalSync";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <h2>Recuperação de senha</h2>
                <p>Clique no link abaixo para redefinir sua senha. O link expira em 15 minutos.</p>
                <a href="{resetLink}" style="background:#7C3AED;color:white;padding:12px 24px;border-radius:8px;text-decoration:none;">
                    Redefinir senha
                </a>
                <p style="color:#9CA3AF;font-size:12px;margin-top:24px;">
                    Se você não solicitou a recuperação, ignore este email.
                </p>
                """
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_user, _password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}