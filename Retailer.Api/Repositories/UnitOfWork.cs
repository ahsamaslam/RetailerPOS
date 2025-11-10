using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly RetailerDbContext _db;
    public IGenericRepository<Item> Items { get; }
    public IGenericRepository<PurchaseMaster> PurchaseMasters { get; }
    public IGenericRepository<PurchaseDetail> PurchaseDetails { get; }

    public UnitOfWork(RetailerDbContext db)
    {
        _db = db;
        Items = new GenericRepository<Item>(_db);
        PurchaseMasters = new GenericRepository<PurchaseMaster>(_db);
        PurchaseDetails = new GenericRepository<PurchaseDetail>(_db);
    }

    public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    public void Dispose() => _db.Dispose();

    public RetailerDbContext GetDbContext() => _db;
}
