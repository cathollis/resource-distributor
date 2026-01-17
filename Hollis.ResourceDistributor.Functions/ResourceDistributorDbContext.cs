using Hollis.ResourceDistributor.Functions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Hollis.ResourceDistributor.Functions;

public class ResourceDistributorDbContext(DbContextOptions<ResourceDistributorDbContext> options) : DbContext(options)
{
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var schemaName = nameof(ResourceDistributorDbContext).Replace(nameof(DbContext), string.Empty);
        modelBuilder.HasDefaultSchema(schemaName);

        modelBuilder.Entity<Resource>()
            .OwnsOne(res => res.RequestCopyHeaderName, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
            }).OwnsOne(res => res.ResponseCopyHeaderName, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
            });

        base.OnModelCreating(modelBuilder);
    }
}
