using Microsoft.AspNetCore.Identity;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Application.Security;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.ValueObjects;

namespace VitalSyncAPI.Application.UseCases;

public class AuthenticateUseCase : IAuthenticateUseCase{

    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthenticateUseCase> _logger;


    public AuthenticateUseCase(JwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, ILogger<AuthenticateUseCase> logger)
    {
        _passwordHasher = new PasswordHasher<User>();
        _tokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
        _logger = logger;
    }
    public async Task<UserDto> ExecuteAsync(LoginRequest request)
    {
        var email = new Email(request.Email);

        var user = await _userRepository.GetUserByEmail(email.Value) ?? throw new NotFoundException("Email ou senha incorretos");

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Tentativa de login com senha incorreta para {Email}", email.Value);
            throw new ValidationException("Email ou senha incorretos");
        }

        var token = _tokenGenerator.GenerateToken(user.Id, user.Name);

        _logger.LogInformation("Login bem-sucedido: usuário {UserId} ({Email})", user.Id, email.Value);

        return new UserDto(user.Name, user.Email, token);
    }
}
