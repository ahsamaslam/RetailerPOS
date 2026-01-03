using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.POS.Api.Services;
public class VendorPaymentService : IVendorPaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public VendorPaymentService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<VendorPaymentDto> CreateAsync(VendorPaymentDto dto,Guid CompanyId)
    {

        var entity = _mapper.Map<VendorPayment>(dto);
        entity.companyId = CompanyId;
        await _uow.VendorPayment.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return _mapper.Map<VendorPaymentDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _uow.VendorPayment.GetAsync(b => b.Id == id);
        if (e == null) throw new KeyNotFoundException("Item not found");
        e.status = 0;
        _uow.VendorPayment.Update(e);
        await _uow.SaveChangesAsync();
    }
    public async Task<IEnumerable<VendorPaymentDto>> GetByDateWiseAsync(DateTime sdate, DateTime edate, Guid companyID)
    {

        var items = await _uow.VendorPayment.Query()
            .Where(r=>r.companyId== companyID)
            .Include(x=>x.PaymentMethod) 
    .Where(r => r.status==1 )
    .ToListAsync();
        return _mapper.Map<IEnumerable<VendorPaymentDto>>(items);
    }
        public async Task<IEnumerable<VendorPaymentDto>> GetAllAsync(Guid CompanyId)
    {


        var items = await _uow.VendorPayment.Query() // IQueryable<Item>
         .Include(i => i.PaymentMethod)
    
         .Where(i => i.companyId == CompanyId)
         .Select(i => new VendorPaymentDto
         {
             Id = i.Id,
             VendorId = i.VendorId,
             Type = i.Type,
             Amount = i.Amount,
             PaymentDate = i.PaymentDate,
             PaymentMethodId = i.PaymentMethodId,
             PaymentMethod = i.PaymentMethod,
             bankId = i.BankId,
             bankName = i.bankName,
             taxPercent = i.taxPercent,
             taxAmount = i.taxAmount,
             whtPercent = i.whtPercent,
             whtAmount = i.whtAmount,
             companyId = i.companyId,
             status = i.status
         })
         .ToListAsync();

        return _mapper.Map<IEnumerable<VendorPaymentDto>>(items);
    }

    public async Task<VendorPaymentDto?> GetByIdAsync(int id)
    {
        var item = await _uow.VendorPayment.Query() 
        .Include(i => i.PaymentMethod)
        .Where(i => i.Id == id)
        .Select(i => new VendorPaymentDto
        {
            Id = i.Id,
            VendorId = i.VendorId,
            Type = i.Type,
            Amount = i.Amount,
            PaymentDate = i.PaymentDate,
            PaymentMethodId = i.PaymentMethodId,
            PaymentMethod = i.PaymentMethod,
            bankId = i.BankId,
            bankName = i.bankName,
            taxPercent = i.taxPercent,
            taxAmount = i.taxAmount,
            whtPercent = i.whtPercent,
            whtAmount = i.whtAmount,
            companyId = i.companyId,
            status = i.status
        })
        .FirstOrDefaultAsync();


        return item is null ? new VendorPaymentDto() : _mapper.Map<VendorPaymentDto>(item);
    }

    public async Task UpdateAsync(int id, VendorPaymentDto dto)
    {
        var e = await _uow.VendorPayment.GetAsync(b => b.Id == id);
        if (e == null) throw new KeyNotFoundException("Item not found");
        _mapper.Map(dto, e);
        _uow.VendorPayment.Update(e);
        await _uow.SaveChangesAsync();
    }
}
