using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Infrastructure.Data;
public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}

