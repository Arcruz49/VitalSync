using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class NutritionRecordRepository(Context db) : BaseRepository<NutritionRecord>(db), INutritionRecordRepository
{
    public async Task<List<NutritionRecord>> GetByUserId(Guid userId, DateTime? from, DateTime? to)
    {
        var query = Query().Where(x => x.UserId == userId);

        if(from != null) query = query.Where(a => a.MeasuredAt >= from);
        if(to != null) query = query.Where(a => a.MeasuredAt <= to);

        return await query.ToListAsync();
    }
    public async Task<NutritionRecord?> GetById(Guid nutritionId)
    {
        return await Query()
            .Where(x => x.Id == nutritionId)
            .FirstOrDefaultAsync();
    }

    public async Task AddNutritionRecord(NutritionRecord nutritionRecord)
    {
        await AddAsync(nutritionRecord);
    }

    public async Task RemoveNutritionRecord(Guid nutritionId)
    {
        var record = await _db.NutritionRecords
            .FirstOrDefaultAsync(x => x.Id == nutritionId)
            ?? throw new NotFoundException("Registro não encontrada.");

        _db.NutritionRecords.Remove(record);
    }
    public void UpdateNutritionRecord(NutritionRecord nutritionRecord)
    {
        Update(nutritionRecord);
    }

}
