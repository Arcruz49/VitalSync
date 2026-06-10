using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class PasswordResetTokenRepository(Context db) : BaseRepository<PasswordResetToken>(db), IPasswordResetTokenRepository
{
    
    public async Task<PasswordResetToken?> GetByToken(string token)
    {
        return await Query().Where(a => a.Token == token).FirstOrDefaultAsync();
    }
    public async Task AddToken(PasswordResetToken passwordResetToken)
    {
        await AddAsync(passwordResetToken);
    }
    public async Task UpdateToken(PasswordResetToken passwordResetToken)
    {
        Update(passwordResetToken);
    }

}