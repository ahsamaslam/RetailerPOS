using Retailer.POS.Api.Entities;
namespace Retailer.POS.Api.Repositories;
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Item> Items { get; }
    IGenericRepository<PurchaseMaster> PurchaseMasters { get; }
    IGenericRepository<PurchaseDetail> PurchaseDetails { get; }
    // add others as needed
    Task<int> SaveChangesAsync();
}
