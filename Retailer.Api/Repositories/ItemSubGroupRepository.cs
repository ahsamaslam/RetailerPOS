using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Repositories
{
    public class ItemSubGroupRepository : GenericRepository<ItemSubGroup>
    {
        private readonly RetailerDbContext _context;

        public ItemSubGroupRepository(RetailerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ItemSubGroupDto>> GetAllWithGroupAsync()
        {
            return await _context.Set<ItemSubGroup>()
                .Include(sg => sg.Group) // Include navigation property
                .Select(sg => new ItemSubGroupDto
                {
                    Id = sg.Id,
                    Name = sg.Name,
                    GroupId = sg.GroupId,
                    GroupName = sg.Group != null ? sg.Group.Name : null
                })
                .ToListAsync();
        }
        public async Task<ItemSubGroupDto?> GetByIdWithGroupAsync(int id)
        {
            return await _context.Set<ItemSubGroup>()
                .Include(sg => sg.Group) // Include related Group
                .Where(sg => sg.Id == id)
                .Select(sg => new ItemSubGroupDto
                {
                    Id = sg.Id,
                    Name = sg.Name,
                    GroupId = sg.GroupId,
                    GroupName = sg.Group != null ? sg.Group.Name : null
                })
                .FirstOrDefaultAsync();
        }
    }
}
