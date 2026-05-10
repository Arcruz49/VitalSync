using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class UserRepository : IUserRepository{

    private readonly Context _db;
    public UserRepository(Context db)
    {
        _db = db;
    }

    public async Task<List<User>> GetUsers(string search = "")
    {
        return await _db.Users.AsNoTracking().Where(a => (a.Name ?? "").Contains(search ?? "")).ToListAsync();
    }
    public async Task<User> GetUserById(Guid id)
    {
        return await _db.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(a => a.Id == id)
        ?? throw new NotFoundException("Usuário não encontrado");
    }
    public User UpdateUser(User Users)
    {
        _db.Users.Update(Users);
        return Users;
    }
    public User CreateUser(User Users)
    {
        _db.Users.Add(Users);
        return Users;
    }
    public async Task DeleteUser(Guid id)
    {
        var Users = await GetUserById(id);
        _db.Users.Remove(Users);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email);
    }
}
