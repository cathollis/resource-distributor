using Hollis.ResourceDistributor.Functions.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hollis.ResourceDistributor.Functions;

public class ResourceDistributorDbContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var schemaName = nameof(ResourceDistributorDbContext).Replace(nameof(DbContext), string.Empty);
        modelBuilder.HasDefaultSchema(schemaName);

        base.OnModelCreating(modelBuilder);
    }
}
