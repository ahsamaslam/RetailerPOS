using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;
using Retailer.POS.API.UnitOfWork;

namespace Retailer.POS.Api.Services;
public class PurchaseService : IPurchaseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PurchaseService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto)
    {
        var db = (_uow as UnitOfWork)!.GetDbContext();
        using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            var pm = _mapper.Map<PurchaseMaster>(dto);
            await _uow.PurchaseMasters.AddAsync(pm);
            await _uow.SaveChangesAsync();

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
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async  Task<IEnumerable<PurchaseMasterDto?>> GetAll()
    {
        var pm = await _uow.PurchaseMasters.Query().Include(p => p.Details).ToListAsync();
        return _mapper.Map<IEnumerable<PurchaseMasterDto>>(pm);
    }

    public async Task<PurchaseMasterDto?> GetByIdAsync(int id)
    {
        var pm = await _uow.PurchaseMasters.Query().Include(p => p.Details).FirstOrDefaultAsync(p => p.Id == id);
        if (pm == null) return null;
        return _mapper.Map<PurchaseMasterDto>(pm);
    }

    public async Task<IEnumerable<PurchaseMasterDto?>> GetDateWiseAsync(DateTime sdate, DateTime edate)
    {
        var pm = await _uow.PurchaseMasters.Query().Include(p => p.Details).Where(p => p.Date.Date>=sdate.Date  &&  p.Date.Date<=edate.Date).ToListAsync();
        return _mapper.Map<IEnumerable<PurchaseMasterDto>>(pm);
    }

    public async Task<bool> UpdateQtys(List<int> productIDs, int year)
    {
        try
        {
            var query =await _uow.VwStockLedger.Query().Where(r => productIDs.Contains(r.ProductID) && r.Year == year)
                .GroupBy(x => x.ProductID).Select(x => new Item { Id = x.Key, QtyInHand = x.Sum(x => x.Qty) }).ToListAsync(); ;


            var itemsToUpdate = _uow.Items.Query().Where(i => productIDs.Contains(i.Id)).ToList();

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
            await _uow.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }


    }
}
