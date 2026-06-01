using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class PersonalRangeRepository(Context db) : BaseRepository<PersonalRange>(db), IPersonalRangeRepository
{
    public async Task<List<PersonalRange>> GetByUserId(Guid userId)
    {
        return await Query()
            .Include(x => x.MetricType)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<PersonalRange?> GetByUserIdAndMetricId(Guid userId, int metricId)
    {
        return await Query()
            .Include(x => x.MetricType)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MetricTypeId == metricId);
    }

    public async Task AddPersonalRanges(List<PersonalRange> personalRanges)
    {
        await _db.PersonalRanges.AddRangeAsync(personalRanges);
    }

    public async Task RemovePersonalRange(Guid userId, int metricId)
    {
        var personalRange = await _db.PersonalRanges
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MetricTypeId == metricId)
            ?? throw new NotFoundException("Condição não encontrada.");

        _db.PersonalRanges.Remove(personalRange);
    }
    public async Task RemoveAllByUserId(Guid userId)
    {
        var personalRange = await _db.PersonalRanges
            .Where(x => x.UserId == userId)
            .ToListAsync();

        _db.PersonalRanges.RemoveRange(personalRange);
    }
}