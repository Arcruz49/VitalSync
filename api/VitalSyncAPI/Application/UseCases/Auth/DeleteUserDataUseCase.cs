using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace VitalSyncAPI.Application.UseCases;

public class DeleteUserDataUseCase(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    ) : IDeleteUserDataUseCase{

    public async Task ExecuteAsync(Guid userId)
    {
        await userRepository.DeleteUser(userId);

        await unitOfWork.SaveChangesAsync();
    }

}
