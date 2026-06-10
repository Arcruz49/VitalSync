using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using VitalSyncAPI.Domain.ValueObjects;

namespace VitalSyncAPI.Application.UseCases;

public class ResetPasswordUseCase(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IUnitOfWork unitOfWork,
    PasswordHasher<User> passwordHasher
    ) : IResetPasswordUseCase{

    public async Task ExecuteAsync(string token, string password)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ValidationException("Token inválido.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ValidationException("Senha é obrigatória.");

        var hashedToken = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        var passwordToken = await passwordResetTokenRepository.GetByToken(hashedToken)
            ?? throw new ValidationException("Token inválido.");

        if (passwordToken.ExpiresAt < DateTime.UtcNow)
            throw new ValidationException("Token expirado.");

        if (passwordToken.UsedAt is not null)
            throw new ValidationException("Token já utilizado.");

        var user = await userRepository.GetUserById(passwordToken.UserId);

        var passwordObject = new Password(password);
        user.Password = passwordHasher.HashPassword(user, passwordObject.Value);

        passwordToken.UsedAt = DateTime.UtcNow;

        userRepository.UpdateUser(user);
        await passwordResetTokenRepository.UpdateToken(passwordToken);

        await unitOfWork.SaveChangesAsync();
    }

}
