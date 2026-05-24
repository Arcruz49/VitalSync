using Microsoft.AspNetCore.Identity;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Application.Security;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.ValueObjects;

namespace VitalSyncAPI.Application.UseCases;

public class RegisterUserUseCase : IRegisterUserUseCase{

    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;


    public RegisterUserUseCase(IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = new PasswordHasher<User>();
        _tokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }
    public async Task<UserDto> ExecuteAsync(RegisterUserRequest request)
    {
        var email = new Email(request.Email);

        if (await _userRepository.GetUserByEmail(email.Value) != null)
            throw new ValidationException("Email já cadastrado.");        
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Gender = request.Gender,
            BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc),
            CreationDate = DateTime.UtcNow,
        };


        var password = new Password(request.Password);
        user.Password = _passwordHasher.HashPassword(user, password.Value);    
        

        user = _userRepository.CreateUser(user);

        await _unitOfWork.SaveChangesAsync();

        var token = _tokenGenerator.GenerateToken(user.Id, user.Name);

        return new UserDto(user.Name, user.Email, token);
    }
}
