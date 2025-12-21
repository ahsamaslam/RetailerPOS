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

    public async Task<ItemDto> CreateAsync(CreateItemDto dto,Guid CompanyId)
    {

        var entity = _mapper.Map<Item>(dto);
        entity.CompanyId = CompanyId;
        await _uow.Items.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return _mapper.Map<ItemDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _uow.Items.GetAsync(b => b.Id == id);
        if (e == null) throw new KeyNotFoundException("Item not found");
        _uow.Items.Remove(e);
        await _uow.SaveChangesAsync();
    }
    public async Task<IEnumerable<ItemDto>> GetStockItemsAsync(int categoryId = 0, int groupId = 0)
    {

        var items = await _uow.Items.Query()
            .Include(x=>x.Category)
            .Include(x=>x.Group)
            .Include(x=>x.SubGroup)
    .Where(r => r.QtyInHand > 0
             && (categoryId == 0 || r.CategoryId == categoryId)
             && (groupId == 0 || r.GroupId == groupId))
    .ToListAsync();
        return _mapper.Map<IEnumerable<ItemDto>>(items);
    }
        public async Task<IEnumerable<ItemDto>> GetAllAsync(Guid CompanyId)
    {


        var items = await _uow.Items.Query() // IQueryable<Item>
         .Include(i => i.Category)
         .Include(i => i.Group)
         .Include(i => i.SubGroup)
         .Include(i => i.ItemType)
         .Where(i => i.CompanyId == CompanyId)
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
        var e = await _uow.Items.GetAsync(b => b.Id == id);
        if (e == null) throw new KeyNotFoundException("Item not found");
        _mapper.Map(dto, e);
        _uow.Items.Update(e);
        await _uow.SaveChangesAsync();
    }
}
