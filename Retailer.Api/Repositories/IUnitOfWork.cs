using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.Entities;
namespace Retailer.POS.Api.Repositories;
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Item> Items { get; }
    IGenericRepository<PurchaseMaster> PurchaseMasters { get; }
    IGenericRepository<PurchaseDetail> PurchaseDetails { get; }
    IGenericRepository<Customer> Customers { get; }
    IGenericRepository<Vendor> Vendors { get; }
    IGenericRepository<Branch> Branches { get; }
    IGenericRepository<Employee> Employees { get; }
    IGenericRepository<SalesMaster> SalesMasters { get; }
    IGenericRepository<SalesDetail> SalesDetails { get; }
    IGenericRepository<StockTransfer> StockTransfers { get; }
    IGenericRepository<StockTransferDetail> StockTransferDetails { get; }
    IGenericRepository<Login> Logins { get; }
    IGenericRepository<ItemCategory> ItemCategories { get; }
    IGenericRepository<ItemType> ItemTypes { get; }
    IGenericRepository<ItemGroup> ItemGroups { get; }
    IGenericRepository<ItemSubGroup> ItemSubGroups { get; }
    IGenericRepository<Role> Roles { get; }
    IGenericRepository<Scope> Scopes { get; }
    IGenericRepository<RoleScope> RoleScopes { get; }
    Task<List<ItemSubGroupDto>> GetSubGroupsWithGroupAsync();
    Task<ItemSubGroupDto?> GetSubGroupByIdWithGroupAsync(int id);
    Task<int> SaveChangesAsync();
}
