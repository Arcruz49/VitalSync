using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IUserProfileRepository
{
    Task<UserProfile> GetProfileById(Guid id);
    Task<UserProfile?> GetByUserId(Guid id);
    void UpdateProfile(UserProfile user);
    Task CreateProfile(UserProfile user);

}