using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.POS.Api.Services;
public class ItemService : IItemService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ItemService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ItemDto> CreateAsync(CreateItemDto dto)
    {
        var entity = _mapper.Map<Item>(dto);
        await _uow.Items.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return _mapper.Map<ItemDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _uow.Items.GetByIdAsync(id);
        if (e == null) throw new KeyNotFoundException("Item not found");
        _uow.Items.Remove(e);
        await _uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<ItemDto>> GetAllAsync()
    {


        var items = await _uow.Items.Query() // IQueryable<Item>
         .Include(i => i.Category)
         .Include(i => i.Group)
         .Include(i => i.SubGroup)
         .Include(i => i.ItemType)
         .Select(i => new ItemDto
         {
             Id = i.Id,
             Name = i.Name,
             Barcode = i.Barcode,
             Rate = i.Rate,
             Cost = i.Cost,
             CategoryName = i.Category != null ? i.Category.Name : null,
             GroupName = i.Group != null ? i.Group.Name : null,
             SubGroupName = i.SubGroup != null ? i.SubGroup.Name : null,
             ItemTypeName = i.ItemType != null ? i.ItemType.Name : null
         })
         .ToListAsync();

        return _mapper.Map<IEnumerable<ItemDto>>(items);
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = await _uow.Items.Query()
        .Include(i => i.Category)
        .Include(i => i.Group)
        .Include(i => i.SubGroup)
        .Include(i => i.ItemType)
        .Where(i => i.Id == id)
        .Select(i => new ItemDto
        {
            Id = i.Id,
            Name = i.Name,
            Barcode = i.Barcode,
            Rate = i.Rate,
            Cost = i.Cost,
            CategoryName = i.Category != null ? i.Category.Name : null,
            GroupName = i.Group != null ? i.Group.Name : null,
            SubGroupName = i.SubGroup != null ? i.SubGroup.Name : null,
            ItemTypeName = i.ItemType != null ? i.ItemType.Name : null
        })
        .FirstOrDefaultAsync();


        return item is null ? new ItemDto() : _mapper.Map<ItemDto>(item);
    }

    public async Task UpdateAsync(int id, CreateItemDto dto)
    {
        var e = await _uow.Items.GetByIdAsync(id);
        if (e == null) throw new KeyNotFoundException("Item not found");
        _mapper.Map(dto, e);
        _uow.Items.Update(e);
        await _uow.SaveChangesAsync();
    }
}
