using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SimpleCustomerApi.Models;

namespace SimpleCustomerApi.Data;

public class DataContext : DbContext
{

    public DataContext(DbContextOptions options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Seed();
    }

    public DbSet<Customer> Customers { get; set; }

    public override int SaveChanges()
    {
        AddTimestamp();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        AddTimestamp();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamp()
    {
        var now = DateTime.UtcNow;
        IEnumerable<EntityEntry> trackedEntities = ChangeTracker.Entries();
        foreach (var entry in trackedEntities)
        {
            if (entry.Entity is not BaseModel entity)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = now;
                    entity.UpdatedAt = now;
                    break;
                case EntityState.Modified:
                    entity.UpdatedAt = now;
                    break;
                default:
                    break;
            }
        }
    }
}