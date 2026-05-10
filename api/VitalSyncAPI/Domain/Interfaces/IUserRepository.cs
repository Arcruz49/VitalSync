using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IUserRepository
{
    Task<List<User>> GetUsers(string search = "");
    Task<User> GetUserById(Guid id);
    User UpdateUser(User user);
    User CreateUser(User user);
    Task DeleteUser(Guid id);
    Task<User?> GetUserByEmail(string email);

}