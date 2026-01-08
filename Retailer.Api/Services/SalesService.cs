using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DtoReport;
using Retailer.Api.DTOs;
using Retailer.Api.Services;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.POS.Api.Services;

public class SalesService : ISalesService
{
    private readonly IUnitOfWork _uow;
    private readonly IFbrClient _fbrClient;
    private readonly ICompanyService _companyService;
    private readonly RetailerDbContext _context;
    private readonly IMapper _mapper;

    public SalesService(IUnitOfWork uow, IFbrClient fbrClient, ICompanyService companyService, RetailerDbContext context, IMapper mapper)
    {
        _uow = uow;
        _fbrClient = fbrClient;
        _companyService = companyService;
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<SalesMasterDto?>> GetDateWiseAsync(DateTime sdate, DateTime edate, Guid CompanyId)
    {
        var pm = await _uow.SalesMasters.Query().Include(p => p.Details)
            .Where(p => p.CompanyId == CompanyId && p.Date.Date >= sdate.Date && p.Date.Date <= edate.Date && p.Active).ToListAsync();

        return _mapper.Map<List<SalesMasterDto>>(pm);
    }

    public async Task<List<SalesMasterDto?>> GetCustomerWiseAsync(int CustomerId, DateTime sdate, DateTime edate, Guid CompanyId)
    {
        var pm = await _uow.SalesMasters.Query().Include(p => p.Details).Where(p =>
        p.CompanyId == CompanyId && p.CustomerCode == CustomerId
        && p.Date.Date >= sdate.Date && p.Date.Date <= edate.Date && p.Active).ToListAsync();
        return _mapper.Map<List<SalesMasterDto>>(pm);
    }

    public async Task<List<ItemSalesReportDtoR?>> GetItemWiseAsync(int itemID, DateTime sdate, DateTime edate, Guid CompanyId)
    {
        var pm = await _uow.SalesDetails.Query()
            .Include(p => new { p.ItemCode, p.ItemName })
            .Include(p => p.SalesMaster)
            .ThenInclude(p => new { p.CustomerCode, p.CustomerName })
            .Where(p => p.SalesMaster!.CompanyId == CompanyId && p.ItemCode == itemID
                    && p.SalesMaster.Date.Date >= sdate.Date && 
                    p.SalesMaster.Date.Date <= edate.Date && p.SalesMaster.Active)
            .ToListAsync();
        return _mapper.Map<List<ItemSalesReportDtoR>>(pm);
    }


    public async Task<SalesMasterDto?> GetAsync(int id, Guid companyId, LoginDto user)
    {
        return await _uow.SalesMasters.Query()
            .Include(x => x.Details)
            .Where(x => x.Id == id && x.CompanyId == companyId && x.Active)
            .Select(s => new SalesMasterDto
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
                Details = s.Details.Select(d => new SalesDetailDto
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
            }).FirstOrDefaultAsync();
    }

    public async Task<SalesMaster> CreateAsync(SalesMaster model, Guid companyId, LoginDto user)
    {
        model.CompanyId = companyId;
        model.Active = true;
        model.UserId = user.Id;
        model.CreateDate = DateTime.UtcNow;
        model.totalAmount = model.BalanceAmount;

        foreach (var d in model.Details)
        {
            d.Id = 0;
            d.SalesMasterId = 0;
            d.SalesMaster = model;
        }

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == model.CustomerCode);
        if (customer != null)
            model.CustomerName = customer.Name;

        await _uow.SalesMasters.AddAsync(model);
        await _uow.SaveChangesAsync();

        var company = await _companyService.GetCompanyByIdAsync(companyId);
        if (company?.fbrActive == true)
        {
            try
            {
                await _fbrClient.SendInvoiceAsync(company, model, customer);
            }
            catch { /* log */ }
        }

        var ledger = new CustomerLedgerService(_context);
        await ledger.PostLedgerAsync(model);

        var itemLedger = new ItemLedgerService(_context);
        foreach (var item in model.Details)
            await itemLedger.PostLedgerAsync(item);

        return model;
    }

    public async Task UpdateAsync(int id, SalesMaster model, Guid companyId, LoginDto user)
    {
        var existing = await _uow.SalesMasters.Query()
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId);

        if (existing == null)
            throw new KeyNotFoundException("Sale not found");

        existing.Date = model.Date;
        existing.UserId = user.Id;
        existing.BranchId = model.BranchId;
        existing.SubTotal = model.SubTotal;
        existing.TotalDiscount = model.TotalDiscount;
        existing.TaxAmount = model.TaxAmount;
        existing.BalanceAmount = model.BalanceAmount;
        existing.totalAmount = model.SubTotal + model.TaxAmount - model.TotalDiscount;

        var toRemove = existing.Details.Where(d => !model.Details.Any(md => md.Id == d.Id)).ToList();
        foreach (var rem in toRemove)
            _uow.SalesDetails.Remove(rem);

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
                await _uow.SalesDetails.AddAsync(new SalesDetail
                {
                    SalesMasterId = existing.Id,
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

        _uow.SalesMasters.Update(existing);
        await _uow.SaveChangesAsync();

        var ledger = new CustomerLedgerService(_context);
        await ledger.UpdateLedgerAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var sale = await _uow.SalesMasters.GetAsync(x => x.Id == id);
        if (sale == null) return;

        sale.Active = false;
        _uow.SalesMasters.Update(sale);
        await _uow.SaveChangesAsync();

        var ledger = new CustomerLedgerService(_context);
        await ledger.ReverseLedgerAsync(sale);
    }
}
