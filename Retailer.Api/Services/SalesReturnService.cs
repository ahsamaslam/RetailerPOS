using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DtoReport;
using Retailer.Api.DTOs;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Services
{
    public class SalesReturnService : ISalesReturnService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICompanyService _companyService;
        private readonly RetailerDbContext _context;
        private readonly IMapper _mapper;


        public SalesReturnService(IUnitOfWork uow, ICompanyService companyService, RetailerDbContext context, IMapper mapper)
        {
            _uow = uow;
            _companyService = companyService;
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<SalesReturnMasterDto?>> GetDateWiseAsync(DateTime sdate, DateTime edate, Guid CompanyId)
        {
            var pm = await _uow.SalesReturnMaster.Query().Include(p => p.Details)
                .Include(x => x.CustomerCode)
                .Include(x => x.CustomerName)
                .Where(p => p.CompanyId == CompanyId && p.Date.Date >= sdate.Date && p.Date.Date <= edate.Date && p.Active).ToListAsync();

            return _mapper.Map<List<SalesReturnMasterDto>>(pm);
        }

        public async Task<List<SalesReturnMasterDto?>> GetCustomerWiseAsync(int CustomerId, DateTime sdate, DateTime edate, Guid CompanyId)
        {
            var pm = await _uow.SalesReturnMaster.Query().Include(p => p.Details).Where(p =>
            p.CompanyId == CompanyId && p.CustomerCode == CustomerId
            && p.Date.Date >= sdate.Date && p.Date.Date <= edate.Date && p.Active).ToListAsync();
            return _mapper.Map<List<SalesReturnMasterDto>>(pm);
        }

        public async Task<List<ItemSalesReturnReportDtoR?>> GetItemWiseAsync(int itemID, DateTime sdate, DateTime edate, Guid CompanyId)
        {
            var pm = await _uow.SalesReturnDetails.Query()
                .Include(p => new { p.ItemCode, p.ItemName })
                .Include(p => p.SalesReturnMaster)
                .ThenInclude(p => new { p.CustomerCode, p.CustomerName })
                .Where(p => p.SalesReturnMaster!.CompanyId == CompanyId && p.ItemCode == itemID
                        && p.SalesReturnMaster.Date.Date >= sdate.Date &&
                        p.SalesReturnMaster.Date.Date <= edate.Date && p.SalesReturnMaster.Active)
                .ToListAsync();
            return _mapper.Map<List<ItemSalesReturnReportDtoR>>(pm);
        }

        public async Task<SalesReturnMasterDto?> GetAsync(int id, Guid companyId, LoginDto user)
        {
            return await _uow.SalesReturnMaster.Query()
                .Include(s => s.Details)
                .Where(s => s.Id == id && s.CompanyId == companyId && s.Active)
                .Select(s => new SalesReturnMasterDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    UserId = user.Id,
                    UserName = user.UserName,
                    BranchId = s.BranchId,
                    CustomerName = s.CustomerName,
                    SaleType = s.SaleType,
                    SubTotal = s.SubTotal,
                    TotalDiscount = s.TotalDiscount,
                    TaxAmount = s.TaxAmount,
                    BalanceAmount = s.BalanceAmount,
                    CustomerCode = s.CustomerCode,
                    Details = s.Details.Select(d => new SalesReturnDetailDto
                    {
                        Id = d.Id,
                        ItemCode = d.ItemCode,
                        ItemName = d.ItemName,
                        Rate = d.Rate,
                        Qty = d.Qty,
                        Discount = d.Discount,
                        TaxPercentage = d.TaxPercentage,
                        TaxAmount = d.TaxAmount,
                        Amount = d.Amount
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<SalesReturnMaster> CreateAsync(SalesReturnMaster model, Guid companyId, LoginDto user)
        {
            model.CompanyId = companyId;
            model.Active = true;
            model.UserId = user.Id;
            model.CreateDate = DateTime.UtcNow;
            model.totalAmount = model.BalanceAmount;

            foreach (var d in model.Details)
            {
                d.Id = 0;
                d.SalesReturnMasterId = 0;
                d.SalesReturnMaster = model;
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == model.CustomerCode);
            if (customer != null)
                model.CustomerName = customer.Name;

            await _uow.SalesReturnMaster.AddAsync(model);
            await _uow.SaveChangesAsync();

            var ledger = new CustomerLedgerService(_context);
            await ledger.PostLedgerAsync(model);

            await _uow.UpdateQtys(
                model.Details.Select(x => x.ItemCode).Distinct().ToList(),
                model.Year
            );

            return model;
        }

        public async Task UpdateAsync(int id, SalesReturnMaster model, Guid companyId, LoginDto user)
        {
            var existing = await _uow.SalesReturnMaster.Query()
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId);

            if (existing == null)
                throw new KeyNotFoundException("Sales return not found");

            existing.Date = model.Date;
            existing.UserId = user.Id;
            existing.BranchId = model.BranchId;
            existing.SubTotal = model.SubTotal;
            existing.TotalDiscount = model.TotalDiscount;
            existing.TaxAmount = model.TaxAmount;
            existing.BalanceAmount = model.BalanceAmount;
            existing.totalAmount = model.SubTotal + model.TaxAmount - model.TotalDiscount;
            existing.Active = true;

            var toRemove = existing.Details.Where(d => !model.Details.Any(md => md.Id == d.Id)).ToList();
            foreach (var rem in toRemove)
                _uow.SalesReturnDetails.Remove(rem);

            foreach (var d in model.Details)
            {
                if (d.Id > 0)
                {
                    var ed = existing.Details.First(x => x.Id == d.Id);
                    ed.ItemCode = d.ItemCode;
                    ed.ItemName = d.ItemName;
                    ed.Rate = d.Rate;
                    ed.Qty = d.Qty;
                    ed.Discount = d.Discount;
                    ed.TaxPercentage = d.TaxPercentage;
                    ed.TaxAmount = d.TaxAmount;
                    ed.Amount = d.Amount;
                }
                else
                {
                    await _uow.SalesReturnDetails.AddAsync(new SalesReturnDetail
                    {
                        SalesReturnMasterId = existing.Id,
                        ItemCode = d.ItemCode,
                        ItemName = d.ItemName,
                        Rate = d.Rate,
                        Qty = d.Qty,
                        Discount = d.Discount,
                        TaxPercentage = d.TaxPercentage,
                        TaxAmount = d.TaxAmount,
                        Amount = d.Amount
                    });
                }
            }

            _uow.SalesReturnMaster.Update(existing);
            await _uow.SaveChangesAsync();

            var ledger = new CustomerLedgerService(_context);
            await ledger.UpdateLedgerAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _uow.SalesReturnMaster.GetAsync(x => x.Id == id);
            if (existing == null) return;

            existing.Active = false;
            _uow.SalesReturnMaster.Update(existing);
            await _uow.SaveChangesAsync();

            var ledger = new CustomerLedgerService(_context);
            await ledger.ReverseLedgerAsync(existing);
        }
    }

}
