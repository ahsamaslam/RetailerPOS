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
        var list = await _uow.Items.Query()
            .Include(i => i.Category)
            .Include(i => i.Group)
            .Include(i => i.SubGroup)
            .Include(i => i.ItemType)
            .Include(i => i.UnitOfMeasure)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ItemDto>>(list);
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var e = await _uow.Items.Query()
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == id);
        return e is null ? null : _mapper.Map<ItemDto>(e);
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
