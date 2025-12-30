using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.Api.Entities.Ledger;
using Retailer.Api.Entities.Views;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;
namespace Retailer.POS.API.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RetailerDbContext _context;

        public UnitOfWork(RetailerDbContext context)
        {
            _context = context;
        }

        private IRepository<Item> _items;
        private IRepository<PurchaseMaster> _purchaseMasters;
        private IRepository<PurchaseDetail> _purchaseDetails;
        private IRepository<Customer> _customers;
        private IRepository<Vendor> _vendors;
        private IRepository<Branch> _branches;
        private IRepository<SalesMaster> _salesMasters;
        private IRepository<SalesReturnMaster> _salesReturnMasters;
        private IRepository<SalesDetail> _salesDetails;
        private IRepository<SalesReturnDetail> _salesReturnDetails;
        private IRepository<StockTransfer> _stockTransfers;
        private IRepository<StockTransferDetail> _stockTransferDetails;
        private IRepository<vwStockLedger> _vwStockLedger;
        private IRepository<ItemCategory>? _itemCategories;
        private IRepository<ItemGroup>? _itemGroups;
        private IRepository<ItemSubGroup>? _itemSubGroups;
        private IRepository<ItemType>? _ItemTypes;
        private IRepository<OpeningBalance>? _OpeningBalances;
        private IRepository<Cities>? _cities;
        private IRepository<Provience>? _provience;
        private IRepository<Banks>? _banks;
        private IRepository<CustomerPayment>? _customerpayment; 
        private IRepository<VendorPayment>? _vendorpayment; 
        private IRepository<CustomerLedger>? _customerledger; 
        private IRepository<VendorLedger>? _vendorledger; 
        private IRepository<BankLedger>? _bankledger; 
        public IRepository<CustomerLedger> CustomerLedger => _customerledger ??= new Repository<CustomerLedger>(_context);
        public IRepository<VendorLedger> VendorLedger => _vendorledger ??= new Repository<VendorLedger>(_context);
        public IRepository<BankLedger> BankLedger => _bankledger ??= new Repository<BankLedger>(_context);
        public IRepository<VendorPayment> VendorPayment => _vendorpayment ??= new Repository<VendorPayment>(_context);
        public IRepository<CustomerPayment> CustomerPayment => _customerpayment ??= new Repository<CustomerPayment>(_context);
        public IRepository<Item> Items => _items ??= new Repository<Item>(_context);
        public IRepository<vwStockLedger> VwStockLedger => _vwStockLedger ??= new Repository<vwStockLedger>(_context);
        public IRepository<PurchaseMaster> PurchaseMasters => _purchaseMasters ??= new Repository<PurchaseMaster>(_context);
        public IRepository<Cities> Cities => _cities ??= new Repository<Cities>(_context);
        public IRepository<Banks> Banks => _banks ??= new Repository<Banks>(_context);
        public IRepository<Provience> Proviences => _provience ??= new Repository<Provience>(_context);
        public IRepository<PurchaseDetail> PurchaseDetails => _purchaseDetails ??= new Repository<PurchaseDetail>(_context);
        public IRepository<Customer> Customers => _customers ??= new Repository<Customer>(_context);
        public IRepository<Vendor> Vendors => _vendors ??= new Repository<Vendor>(_context);
        public IRepository<Branch> Branches => _branches ??= new Repository<Branch>(_context);
        public IRepository<SalesMaster> SalesMasters => _salesMasters ??= new Repository<SalesMaster>(_context);
        public IRepository<SalesReturnMaster> SalesReturnMaster => _salesReturnMasters ??= new Repository<SalesReturnMaster>(_context);
        public IRepository<SalesDetail> SalesDetails => _salesDetails ??= new Repository<SalesDetail>(_context);
        public IRepository<SalesReturnDetail> SalesReturnDetails => _salesReturnDetails ??= new Repository<SalesReturnDetail>(_context);
        public IRepository<StockTransfer> StockTransfers => _stockTransfers ??= new Repository<StockTransfer>(_context);
        public IRepository<StockTransferDetail> StockTransferDetails => _stockTransferDetails ??= new Repository<StockTransferDetail>(_context);
        public IRepository<ItemCategory> ItemCategories => _itemCategories ??= new Repository<ItemCategory>(_context);
        public IRepository<ItemGroup> ItemGroups => _itemGroups ??= new Repository<ItemGroup>(_context);
        public IRepository<ItemSubGroup> ItemSubGroups => _itemSubGroups ??= new Repository<ItemSubGroup>(_context);

        public IRepository<ItemType> ItemTypes => _ItemTypes ??= new Repository<ItemType>(_context);
        public IRepository<OpeningBalance> OpeningBalances => _OpeningBalances ??= new Repository<OpeningBalance>(_context);

        public RetailerDbContext GetDbContext()
        {
            return _context;
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #region IDisposable Support
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async  Task<bool> UpdateQtys(List<int> productIDs, int year)
        {
            try
            {
                var query =   _context.vwStockLedger.Where(r => productIDs.Contains(r.ProductID) && r.Year == year)
                    .GroupBy(x => x.ProductID).Select(x => new Item { Id = x.Key, QtyInHand = x.Sum(x => x.Qty) }).ToList(); ;


                var itemsToUpdate = _context.Items.Where(i => productIDs.Contains(i.Id)).ToList();

                // Step 3: Update Qty in Item table
                foreach (var item in itemsToUpdate)
                {
                    var stock = query.FirstOrDefault(s => s.Id == item.Id);
                    if (stock != null)
                    {
                        item.QtyInHand = stock.QtyInHand;
                    }
                }

                // Step 4: Save changes
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
