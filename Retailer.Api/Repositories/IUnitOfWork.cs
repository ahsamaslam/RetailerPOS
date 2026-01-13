using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.Api.Entities.Ledger;
using Retailer.Api.Entities.Views;
using Retailer.POS.Api.Entities;
namespace Retailer.POS.Api.Repositories;
public interface IUnitOfWork : IDisposable
{
    IRepository<Item> Items { get; }
    IRepository<VendorLedger> VendorLedger { get; }
    IRepository<CustomerLedger> CustomerLedger { get; }
    IRepository<BankLedger> BankLedger { get; }
    IRepository<CustomerPayment> CustomerPayment { get; }
    IRepository<VendorPayment> VendorPayment { get; }
    IRepository<PurchaseMaster> PurchaseMasters { get; }
    IRepository<PurchaseDetail> PurchaseDetails { get; }
    IRepository<PurchaseReturnMaster> PurchaseReturnMasters { get; }
    IRepository<PurchaseReturnDetail> PurchaseReturnDetails { get; }
    IRepository<Customer> Customers { get; }
    IRepository<Vendor> Vendors { get; }
    IRepository<vwStockLedger> VwStockLedger { get; }
    IRepository<Branch> Branches { get; }
    IRepository<SalesMaster> SalesMasters { get; }
    IRepository<SalesReturnMaster> SalesReturnMaster { get; }
    IRepository<SalesReturnDetail> SalesReturnDetails { get; }
    IRepository<SalesDetail> SalesDetails { get; }
    IRepository<StockTransfer> StockTransfers { get; }
    IRepository<StockTransferDetail> StockTransferDetails { get; }
    IRepository<ItemCategory> ItemCategories { get; }
    IRepository<ItemType> ItemTypes { get; }
    IRepository<ItemGroup> ItemGroups { get; }
    IRepository<ItemSubGroup> ItemSubGroups { get; }
    IRepository<OpeningBalance> OpeningBalances { get; }
    IRepository<Cities> Cities { get; }
    IRepository<Provience> Proviences { get; }
    IRepository<Banks> Banks { get; }
    Task<int> SaveChangesAsync();
    Task<bool> UpdateQtys(List<int> productIDs, int year);
}
