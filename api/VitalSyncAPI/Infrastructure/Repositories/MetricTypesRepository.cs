using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization.Infrastructure;

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
    public async Task<MetricType> GetById(int id)
    {
        return await _db.MetricTypes.AsNoTracking().Where(a => a.Id == id).FirstOrDefaultAsync() ?? throw new NotFoundException("Métrica não encontrada");
    }
    
}
