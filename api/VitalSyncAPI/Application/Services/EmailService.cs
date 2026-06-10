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
                <!DOCTYPE html>
                <html lang="pt-BR">
                <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                </head>
                <body style="margin:0;padding:0;background:#F4F3F0;font-family:'Inter',Arial,sans-serif;">
                <table width="100%" cellpadding="0" cellspacing="0" style="background:#F4F3F0;padding:40px 0;">
                    <tr>
                    <td align="center">
                        <table width="560" cellpadding="0" cellspacing="0" style="background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);">
                        
                        <!-- HEADER -->
                        <tr>
                            <td style="background:#111827;padding:32px 40px;">
                            <h1 style="margin:0;color:#ffffff;font-size:22px;font-weight:700;letter-spacing:-0.02em;">VitalSync.</h1>
                            <p style="margin:8px 0 0;color:#9CA3AF;font-size:13px;">Plataforma de saúde personalizada</p>
                            </td>
                        </tr>

                        <!-- BODY -->
                        <tr>
                            <td style="padding:40px 40px 32px;">
                            <h2 style="margin:0 0 12px;color:#111827;font-size:20px;font-weight:700;letter-spacing:-0.02em;">Recuperação de senha</h2>
                            <p style="margin:0 0 24px;color:#4B5563;font-size:15px;line-height:1.6;">
                                Recebemos uma solicitação para redefinir a senha da sua conta. Clique no botão abaixo para criar uma nova senha.
                            </p>

                            <!-- BUTTON -->
                            <table cellpadding="0" cellspacing="0" style="margin:0 0 28px;">
                                <tr>
                                <td style="background:#7C3AED;border-radius:10px;">
                                    <a href="{resetLink}"
                                    style="display:inline-block;padding:14px 32px;color:#ffffff;font-size:15px;font-weight:600;text-decoration:none;letter-spacing:-0.01em;">
                                    Redefinir senha
                                    </a>
                                </td>
                                </tr>
                            </table>

                            <!-- EXPIRY INFO -->
                            <table cellpadding="0" cellspacing="0" width="100%" style="background:#F4F3F0;border-radius:8px;margin-bottom:28px;">
                                <tr>
                                <td style="padding:14px 16px;">
                                    <p style="margin:0;color:#4B5563;font-size:13px;line-height:1.5;">
                                    ⏱ Este link expira em <strong>15 minutos</strong>. Se expirar, solicite um novo link na tela de login.
                                    </p>
                                </td>
                                </tr>
                            </table>

                            <!-- FALLBACK LINK -->
                            <p style="margin:0 0 8px;color:#9CA3AF;font-size:12px;">
                                Se o botão não funcionar, copie e cole este link no seu navegador:
                            </p>
                            <p style="margin:0;word-break:break-all;">
                                <a href="{resetLink}" style="color:#7C3AED;font-size:12px;">{resetLink}</a>
                            </p>
                            </td>
                        </tr>

                        <!-- FOOTER -->
                        <tr>
                            <td style="background:#F9FAFB;border-top:1px solid #E5E7EB;padding:24px 40px;">
                            <p style="margin:0 0 6px;color:#9CA3AF;font-size:12px;line-height:1.5;">
                                Se você não solicitou a recuperação de senha, ignore este email. Sua senha permanece a mesma.
                            </p>
                            <p style="margin:0;color:#D1D5DB;font-size:11px;">
                                © 2026 VitalSync · Plataforma de saúde personalizada
                            </p>
                            </td>
                        </tr>

                        </table>
                    </td>
                    </tr>
                </table>
                </body>
                </html>
                """
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_user, _password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}