using AuthModule.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthModule.API.Data;

public class RetailerLookupDbContext : DbContext
{
    public RetailerLookupDbContext(DbContextOptions<RetailerLookupDbContext> options) : base(options)
    {
    }

    public DbSet<BranchSummary> Branches => Set<BranchSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BranchSummary>(entity =>
        {
            entity.ToTable("Branches");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).HasMaxLength(200);
        });
    }
}
