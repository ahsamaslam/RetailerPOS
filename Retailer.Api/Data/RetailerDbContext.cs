using Microsoft.EntityFrameworkCore;
using Retailer.Api.Entities;
using Retailer.Api.Entities.Ledger;
using Retailer.Api.Entities.Views;
using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.Data;

public class RetailerDbContext : DbContext
{
    public RetailerDbContext(DbContextOptions<RetailerDbContext> options) : base(options) { }

    public DbSet<vwStockLedger> vwStockLedger => Set<vwStockLedger>();
    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
    public DbSet<ItemGroup> ItemGroups => Set<ItemGroup>();
    public DbSet<ItemSubGroup> ItemSubGroups => Set<ItemSubGroup>();
    public DbSet<ItemType> ItemTypes => Set<ItemType>();
    public DbSet<UnitOfMeasure> UnitOfMeasures => Set<UnitOfMeasure>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Banks> Banks => Set<Banks>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PurchaseMaster> PurchaseMasters => Set<PurchaseMaster>();
    public DbSet<PurchaseDetail> PurchaseDetails => Set<PurchaseDetail>();
    public DbSet<PurchaseReturnMaster> PurchaseReturnMasters => Set<PurchaseReturnMaster>();
    public DbSet<PurchaseReturnDetail> PurchaseReturnDetails => Set<PurchaseReturnDetail>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockTransferDetail> StockTransferDetails => Set<StockTransferDetail>();
    public DbSet<SalesReturnMaster> SalesReturnMasters => Set<SalesReturnMaster>();
    public DbSet<SalesMaster> SalesMasters => Set<SalesMaster>();
    public DbSet<SalesDetail> SalesDetails => Set<SalesDetail>();
    public DbSet<SalesReturnDetail> SalesReturnDetails => Set<SalesReturnDetail>();
    public DbSet<Menu> Menus { get; set; } = default!;
    public DbSet<SubMenu> SubMenus { get; set; } = default!;
    public DbSet<OpeningBalance> OpeningBalances { get; set; } = null!;
    public DbSet<CustomerPayment> CustomerPayment { get; set; } = null!;
    public DbSet<VendorPayment> VendorPayment { get; set; } = null!;
    public DbSet<CustomerLedger> CustomerLedger { get; set; } = null!;
    public DbSet<ItemLedger> ItemLedger { get; set; } = null!;
    public DbSet<VendorLedger> VendorLedger { get; set; } = null!;
    public DbSet<BankLedger> BankLedger { get; set; } = null!;
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;


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
            b.Property(s => s.UrlTitle).HasMaxLength(500);
            b.Property(s => s.Icon).HasMaxLength(200);
        });
        modelBuilder.Entity<VendorPayment>()
    .Property(x => x.bankName)
    .IsRequired(false);

        modelBuilder.Entity<VendorPayment>()
            .HasOne(vp => vp.Vendor)
            .WithMany()
            .HasForeignKey(vp => vp.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VendorPayment>()
            .HasOne(vp => vp.Bank)
            .WithMany()
            .HasForeignKey(vp => vp.BankId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustomerPayment>()
            .HasOne(cp => cp.Bank)
            .WithMany()
            .HasForeignKey(cp => cp.BankId)
            .OnDelete(DeleteBehavior.Restrict);

        // SubMenuPermission (many-to-many)
        modelBuilder.Entity<OpeningBalance>()
          .HasIndex(ob => new { ob.Year, ob.ProductID })
          .IsUnique()
          .HasDatabaseName("UX_OpeningBalance_Year_Product");
    }
}
