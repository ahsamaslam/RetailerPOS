using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.POS.Api.Services;
public class CustomerPaymentService : ICustomerPaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CustomerPaymentService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CustomerPaymentDto> CreateAsync(CustomerPaymentDto dto,Guid CompanyId)
    {

        var entity = _mapper.Map<CustomerPayment>(dto);
        entity.companyId = CompanyId;
        await _uow.CustomerPayment.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return _mapper.Map<CustomerPaymentDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _uow.CustomerPayment.GetAsync(b => b.Id == id);
        if (e == null) throw new KeyNotFoundException("Item not found");
        e.status = 0;
        _uow.CustomerPayment.Update(e);
        await _uow.SaveChangesAsync();
    }
    public async Task<IEnumerable<CustomerPaymentDto>> GetByDateWiseAsync(DateTime sdate, DateTime edate, Guid companyID)
    {

        var items = await _uow.CustomerPayment.Query()
            .Where(r=>r.companyId== companyID)
            .Include(x=>x.PaymentMethod) 
    .Where(r => r.status==1 )
    .ToListAsync();
        return _mapper.Map<IEnumerable<CustomerPaymentDto>>(items);
    }
        public async Task<IEnumerable<CustomerPaymentDto>> GetAllAsync(Guid CompanyId)
    {


        var items = await _uow.CustomerPayment.Query() // IQueryable<Item>
         .Include(i => i.PaymentMethod)
    
         .Where(i => i.companyId == CompanyId)
         .Select(i => new CustomerPaymentDto
         {
             Id = i.Id,
             CustomerId = i.CustomerId,
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

        return _mapper.Map<IEnumerable<CustomerPaymentDto>>(items);
    }

    public async Task<CustomerPaymentDto?> GetByIdAsync(int id)
    {
        var item = await _uow.CustomerPayment.Query() 
        .Include(i => i.PaymentMethod)
        .Where(i => i.Id == id)
        .Select(i => new CustomerPaymentDto
        {
            Id = i.Id,
            CustomerId = i.CustomerId,
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


        return item is null ? new CustomerPaymentDto() : _mapper.Map<CustomerPaymentDto>(item);
    }

    public async Task UpdateAsync(int id, CustomerPaymentDto dto)
    {
        var e = await _uow.CustomerPayment.GetAsync(b => b.Id == id);
        if (e == null) throw new KeyNotFoundException("Item not found");
        _mapper.Map(dto, e);
        _uow.CustomerPayment.Update(e);
        await _uow.SaveChangesAsync();
    }
}
