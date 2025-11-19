using Microsoft.EntityFrameworkCore;
using Retailer.Api.Entities;
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
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Scope> Scopes => Set<Scope>();
    public DbSet<RoleScope> RoleScopes => Set<RoleScope>();
    public DbSet<Login> Logins => Set<Login>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PurchaseMaster> PurchaseMasters => Set<PurchaseMaster>();
    public DbSet<PurchaseDetail> PurchaseDetails => Set<PurchaseDetail>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockTransferDetail> StockTransferDetails => Set<StockTransferDetail>();
    public DbSet<SalesMaster> SalesMasters => Set<SalesMaster>();
    public DbSet<SalesDetail> SalesDetails => Set<SalesDetail>();
    public DbSet<Menu> Menus { get; set; } = default!;
    public DbSet<SubMenu> SubMenus { get; set; } = default!;
    public DbSet<SubMenuPermission> SubMenuPermissions { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>().HasOne(i => i.Category).WithMany(c => c.Items).HasForeignKey(i => i.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Item>().HasOne(i => i.Group).WithMany(g => g.Items).HasForeignKey(i => i.GroupId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Item>().HasOne(i => i.SubGroup).WithMany(s => s.Items).HasForeignKey(i => i.SubGroupId).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Item>().Property(i => i.Rate).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Item>().Property(i => i.Cost).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Menu>(b =>
        {
            b.HasKey(m => m.Id);
            b.Property(m => m.Title).IsRequired().HasMaxLength(200);
            b.Property(m => m.Icon).HasMaxLength(200);
            b.HasMany(m => m.SubMenus).WithOne(s => s.Menu).HasForeignKey(s => s.MenuId).OnDelete(DeleteBehavior.Cascade);
        });

        // SubMenu
        modelBuilder.Entity<SubMenu>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Title).IsRequired().HasMaxLength(200);
            b.Property(s => s.Route).HasMaxLength(500);
            b.Property(s => s.Icon).HasMaxLength(200);
        });

        // SubMenuPermission (many-to-many)
        modelBuilder.Entity<SubMenuPermission>(b =>
        {
            b.HasKey(sp => new { sp.SubMenuId });

            b.HasOne(sp => sp.SubMenu)
             .WithMany(s => s.SubMenuPermissions)
             .HasForeignKey(sp => sp.SubMenuId)
             .OnDelete(DeleteBehavior.Cascade);

            // no b.HasOne(...Permission...) since Permission type is not in this DbContext
        });
    }
}
