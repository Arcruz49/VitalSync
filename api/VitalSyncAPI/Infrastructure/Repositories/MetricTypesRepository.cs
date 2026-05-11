using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class MetricTypesRepository : IMetricTypesRepository{

    private readonly Context _db;
    public MetricTypesRepository(Context db)
    {
        _db = db;
    }

    public async Task<List<MetricType>> GetAll()
    {
        return await _db.MetricTypes.AsNoTracking().OrderBy(a => a.SortOrder).ToListAsync();
    }
    
}
