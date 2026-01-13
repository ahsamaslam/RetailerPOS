using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DtoReport;
using Retailer.Api.Services;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;
using Retailer.POS.API.UnitOfWork;

namespace Retailer.POS.Api.Services;
public class PurchaseService : IPurchaseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly RetailerDbContext _context;
	private readonly VendorLedgerService _vendorservice;
	private readonly ItemLedgerService _itemservice;
	public PurchaseService(IUnitOfWork uow, IMapper mapper, RetailerDbContext context)
    {
        _uow = uow;
        _mapper = mapper;
        _context = context;    
        _vendorservice = new VendorLedgerService(_context);
        _itemservice = new ItemLedgerService(_context);
    }

    public async Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto, Guid CompanyId,Guid UserId)
    {
        var db = (_uow as UnitOfWork)!.GetDbContext();
        using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            var pm = _mapper.Map<PurchaseMaster>(dto);
            pm.UserId = UserId;
            pm.CompanyId = CompanyId;
            pm.UserId = UserId;
            pm.UserName = dto.UserName ?? "";
            await _uow.PurchaseMasters.AddAsync(pm);
            await _uow.SaveChangesAsync(); 
            await _vendorservice.PostLedgerAsync(pm); 
            foreach (var d in pm.Details)
            {

                await _itemservice.PostLedgerAsync(d);
            }       
          
          
            //foreach (var d in dto.Details)
            //{
            //    var pd = _mapper.Map<PurchaseDetail>(d);
            //    pd.PurchaseId = pm.Id;
            //    await _uow.PurchaseDetails.AddAsync(pd);
            //}

            await _uow.SaveChangesAsync();
            await tx.CommitAsync();

            return _mapper.Map<PurchaseMasterDto>(pm);
        }
        catch(Exception e)
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async  Task<IEnumerable<PurchaseMasterDto?>> GetAll(Guid CompanyId)
    {
        var pm = await _uow.PurchaseMasters.Query().Include(p => p.Details).Where(x => x.CompanyId == CompanyId  && x.Active==1).ToListAsync();
        return _mapper.Map<IEnumerable<PurchaseMasterDto>>(pm);
    }

    public async Task<PurchaseMasterDto?> GetByIdAsync(int id ,Guid companyID)
    {
        var pm = await _uow.PurchaseMasters.Query().Include(p => p.Details).FirstOrDefaultAsync(p => p.Id == id  && p.CompanyId== companyID);
        if (pm == null) return null;
        return _mapper.Map<PurchaseMasterDto>(pm);
    }

    public async Task<IEnumerable<PurchaseMasterDto?>> GetDateWiseAsync(DateTime sdate, DateTime edate, Guid CompanyId)
    {
        var pm = await _uow.PurchaseMasters.Query().Include(p => p.Details).Include(x=>x.Vendor).Where(p => p.CompanyId == CompanyId && p.Date.Date>=sdate.Date  &&  p.Date.Date<=edate.Date  && p.Active==1).ToListAsync();
       
        return _mapper.Map<IEnumerable<PurchaseMasterDto>>(pm);
    }

    public async  Task<IEnumerable<PurchaseMasterDto?>> GetVendorWiseAsync(int vendorID, DateTime sdate, DateTime edate, Guid CompanyId)
    {
        var pm = await _uow.PurchaseMasters.Query().Include(p => p.Details).Where(p => 
        p.CompanyId == CompanyId  && p.VendorID == vendorID 
        && p.Date.Date >= sdate.Date && p.Date.Date <= edate.Date && p.Active == 1).ToListAsync();
        return _mapper.Map<IEnumerable<PurchaseMasterDto>>(pm);
    }

    public async  Task<IEnumerable<ItemPurchaseReportDtoR?>> GetItemWiseAsync(int itemID, DateTime sdate, DateTime edate, Guid CompanyId)
    {
        var pm = await _uow.PurchaseDetails.Query().Include(p => p.Item ).Include(p => p.Purchase).ThenInclude(p=>p.Vendor).Where(p => 
        p.Purchase.CompanyId == CompanyId  && p.ItemId == itemID
        && p.Purchase.Date.Date >= sdate.Date && p.Purchase.Date.Date <= edate.Date && p.Purchase.Active == 1).ToListAsync();
        return _mapper.Map<IEnumerable<ItemPurchaseReportDtoR>>(pm);
    }
}
