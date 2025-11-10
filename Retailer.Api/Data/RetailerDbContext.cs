using Microsoft.EntityFrameworkCore;
using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.Data;

public class RetailerDbContext : DbContext
{
    public RetailerDbContext(DbContextOptions<RetailerDbContext> options) : base(options) { }

    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
    public DbSet<ItemGroup> ItemGroups => Set<ItemGroup>();
    public DbSet<ItemSubGroup> ItemSubGroups => Set<ItemSubGroup>();
    public DbSet<ItemType> ItemTypes => Set<ItemType>();
    public DbSet<UnitOfMeasure> UnitOfMeasures => Set<UnitOfMeasure>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Login> Logins => Set<Login>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PurchaseMaster> PurchaseMasters => Set<PurchaseMaster>();
    public DbSet<PurchaseDetail> PurchaseDetails => Set<PurchaseDetail>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockTransferDetail> StockTransferDetails => Set<StockTransferDetail>();
    public DbSet<SalesMaster> SalesMasters => Set<SalesMaster>();
    public DbSet<SalesDetail> SalesDetails => Set<SalesDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>().HasOne(i => i.Category).WithMany(c => c.Items).HasForeignKey(i => i.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Item>().HasOne(i => i.Group).WithMany(g => g.Items).HasForeignKey(i => i.GroupId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Item>().HasOne(i => i.SubGroup).WithMany(s => s.Items).HasForeignKey(i => i.SubGroupId).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Item>().Property(i => i.Rate).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Item>().Property(i => i.Cost).HasColumnType("decimal(18,2)");
    }
}
