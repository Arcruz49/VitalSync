using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;

namespace VitalSyncAPI.Infrastructure.Repositories;
public abstract class BaseRepository<T> where T : class
{
    protected readonly Context _db;

    protected BaseRepository(Context db)
    {
        _db = db;
    }

    protected async Task AddAsync(T entity)
        => await _db.AddAsync(entity);

    protected void Update(T entity)
        => _db.Update(entity);

    protected void Remove(T entity)
        => _db.Remove(entity);

    protected async Task<T?> FindAsync(Guid id)
        => await _db.FindAsync<T>(id);

    protected IQueryable<T> Query()
        => _db.Set<T>().AsNoTracking();
}