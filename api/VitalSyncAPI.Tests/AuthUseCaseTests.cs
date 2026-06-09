using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.Security;
using VitalSyncAPI.Application.UseCases;

namespace VitalSyncAPI.Tests;

public class AuthUseCaseTests
{
    private static JwtTokenGenerator MakeJwt() =>
        new(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]              = "test-key-that-is-at-least-32-chars-long!!",
                ["Jwt:Issuer"]           = "vitalsync",
                ["Jwt:Audience"]         = "vitalsync",
                ["Jwt:ExpiresInMinutes"] = "60",
            })
            .Build());

    private static User MakeHashedUser(string email = "user@test.com", string plainPassword = "password123")
    {
        var user = new User
        {
            Id            = Guid.NewGuid(),
            Name          = "Test User",
            Email         = email,
            Gender        = "Male",
            BirthDate     = new DateTime(1994, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreationDate  = DateTime.UtcNow,
        };
        user.Password = new PasswordHasher<User>().HashPassword(user, plainPassword);
        return user;
    }

    // ── RegisterUserUseCase ────────────────────────────────────────────────

    [Fact]
    public async Task Register_ComDadosValidos_CriaUsuarioERetornaToken()
    {
        var userRepo   = Substitute.For<IUserRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        userRepo.GetUserByEmail(Arg.Any<string>()).Returns((User?)null);
        userRepo.CreateUser(Arg.Any<User>()).Returns(ci => ci.Arg<User>());

        var useCase = new RegisterUserUseCase(userRepo, MakeJwt(), unitOfWork);
        var request = new RegisterUserRequest
        {
            Name = "Test", Email = "user@test.com", Password = "password123",
            Gender = "Male", BirthDate = new DateTime(1994, 1, 1),
        };

        var result = await useCase.ExecuteAsync(request);

        Assert.Equal("Test", result.name);
        Assert.NotEmpty(result.token);
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Register_ComEmailDuplicado_LancaValidationException()
    {
        var userRepo   = Substitute.For<IUserRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        userRepo.GetUserByEmail(Arg.Any<string>()).Returns(MakeHashedUser());

        var useCase = new RegisterUserUseCase(userRepo, MakeJwt(), unitOfWork);
        var request = new RegisterUserRequest
        {
            Name = "Test", Email = "user@test.com", Password = "password123",
            Gender = "Male", BirthDate = new DateTime(1994, 1, 1),
        };

        await Assert.ThrowsAsync<ValidationException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task Register_ComEmailInvalido_LancaValidationException()
    {
        var userRepo   = Substitute.For<IUserRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var useCase = new RegisterUserUseCase(userRepo, MakeJwt(), unitOfWork);
        var request = new RegisterUserRequest
        {
            Name = "Test", Email = "nao-e-um-email", Password = "password123",
            Gender = "Male", BirthDate = new DateTime(1994, 1, 1),
        };

        await Assert.ThrowsAsync<ValidationException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task Register_ComSenhaCurta_LancaValidationException()
    {
        var userRepo   = Substitute.For<IUserRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var useCase = new RegisterUserUseCase(userRepo, MakeJwt(), unitOfWork);
        var request = new RegisterUserRequest
        {
            Name = "Test", Email = "user@test.com", Password = "123",
            Gender = "Male", BirthDate = new DateTime(1994, 1, 1),
        };

        await Assert.ThrowsAsync<ValidationException>(() => useCase.ExecuteAsync(request));
    }

    // ── AuthenticateUseCase ────────────────────────────────────────────────

    [Fact]
    public async Task Login_ComCredenciaisCorretas_RetornaUserDto()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var logger   = Substitute.For<ILogger<AuthenticateUseCase>>();
        var user     = MakeHashedUser();

        userRepo.GetUserByEmail(user.Email).Returns(user);

        var useCase = new AuthenticateUseCase(MakeJwt(), userRepo, logger);
        var result  = await useCase.ExecuteAsync(new LoginRequest { Email = user.Email, Password = "password123" });

        Assert.Equal(user.Name, result.name);
        Assert.NotEmpty(result.token);
    }

    [Fact]
    public async Task Login_ComEmailInexistente_LancaNotFoundException()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var logger   = Substitute.For<ILogger<AuthenticateUseCase>>();

        userRepo.GetUserByEmail(Arg.Any<string>()).Returns((User?)null);

        var useCase = new AuthenticateUseCase(MakeJwt(), userRepo, logger);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(new LoginRequest { Email = "naoexiste@test.com", Password = "password123" }));
    }

    [Fact]
    public async Task Login_ComSenhaIncorreta_LancaValidationException()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var logger   = Substitute.For<ILogger<AuthenticateUseCase>>();
        var user     = MakeHashedUser();

        userRepo.GetUserByEmail(user.Email).Returns(user);

        var useCase = new AuthenticateUseCase(MakeJwt(), userRepo, logger);

        await Assert.ThrowsAsync<ValidationException>(
            () => useCase.ExecuteAsync(new LoginRequest { Email = user.Email, Password = "senha-errada" }));
    }
}
