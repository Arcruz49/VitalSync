using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class UserProfileRepository(Context db) : BaseRepository<UserProfile>(db), IUserProfileRepository
{
    public async Task<UserProfile?> GetByUserId(Guid userId)
    {
        return await Query().FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<UserProfile> GetProfileById(Guid id)
    {
        return await Query()
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException("Perfil não encontrado.");
    }

    public async Task CreateProfile(UserProfile profile)
    {
        await AddAsync(profile);
    }

    public void UpdateProfile(UserProfile profile)
    {
        Update(profile);
    }
}