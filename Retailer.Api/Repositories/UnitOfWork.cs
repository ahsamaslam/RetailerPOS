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

        private IGenericRepository<Item> _items;
        private IGenericRepository<PurchaseMaster> _purchaseMasters;
        private IGenericRepository<PurchaseDetail> _purchaseDetails;
        private IGenericRepository<Customer> _customers;
        private IGenericRepository<Vendor> _vendors;
        private IGenericRepository<Branch> _branches;
        private IGenericRepository<Employee> _employees;
        private IGenericRepository<SalesMaster> _salesMasters;
        private IGenericRepository<SalesDetail> _salesDetails;
        private IGenericRepository<StockTransfer> _stockTransfers;
        private IGenericRepository<StockTransferDetail> _stockTransferDetails;
        private IGenericRepository<Login> _logins;

        public IGenericRepository<Item> Items => _items ??= new GenericRepository<Item>(_context);
        public IGenericRepository<PurchaseMaster> PurchaseMasters => _purchaseMasters ??= new GenericRepository<PurchaseMaster>(_context);
        public IGenericRepository<PurchaseDetail> PurchaseDetails => _purchaseDetails ??= new GenericRepository<PurchaseDetail>(_context);
        public IGenericRepository<Customer> Customers => _customers ??= new GenericRepository<Customer>(_context);
        public IGenericRepository<Vendor> Vendors => _vendors ??= new GenericRepository<Vendor>(_context);
        public IGenericRepository<Branch> Branches => _branches ??= new GenericRepository<Branch>(_context);
        public IGenericRepository<Employee> Employees => _employees ??= new GenericRepository<Employee>(_context);
        public IGenericRepository<SalesMaster> SalesMasters => _salesMasters ??= new GenericRepository<SalesMaster>(_context);
        public IGenericRepository<SalesDetail> SalesDetails => _salesDetails ??= new GenericRepository<SalesDetail>(_context);
        public IGenericRepository<StockTransfer> StockTransfers => _stockTransfers ??= new GenericRepository<StockTransfer>(_context);
        public IGenericRepository<StockTransferDetail> StockTransferDetails => _stockTransferDetails ??= new GenericRepository<StockTransferDetail>(_context);
        public IGenericRepository<Login> Logins => _logins ??= new GenericRepository<Login>(_context);
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
        #endregion
    }
}
