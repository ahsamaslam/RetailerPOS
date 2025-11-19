using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.Data;
using System.Net.Http;

namespace Retailer.Api.Services
{
    public class MenuService : IMenuService
    {
        private readonly RetailerDbContext _db;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public MenuService(RetailerDbContext db, HttpClient httpClient, IMemoryCache cache)
        {
            _db = db;
            _httpClient = httpClient;
            _cache = cache;
        }
        // Admin: full list
        public async Task<IEnumerable<MenuDto>> GetAllMenusAsync()
        {
            var menus = await _db.Menus
                .Include(m => m.SubMenus)
                  .ThenInclude(s => s.SubMenuPermissions)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            return menus.Select(MapToDto);
        }

        public async Task<MenuDto?> GetMenuByIdAsync(int id)
        {
            var m = await _db.Menus
                .Include(x => x.SubMenus)
                  .ThenInclude(s => s.SubMenuPermissions)
                .FirstOrDefaultAsync(x => x.Id == id);

            return m == null ? null : MapToDto(m);
        }

        public async Task<MenuDto> CreateMenuAsync(MenuDto dto)
        {
            var m = new Menu { Title = dto.Title, Icon = dto.Icon, SortOrder = dto.SortOrder, IsActive = dto.IsActive };
            _db.Menus.Add(m);
            await _db.SaveChangesAsync();

            // add submenus if provided
            foreach (var sm in dto.SubMenus)
            {
                var s = new SubMenu
                {
                    MenuId = m.Id,
                    Title = sm.Title,
                    Route = sm.Route,
                    SortOrder = sm.SortOrder,
                    IsActive = sm.IsActive
                };
                _db.SubMenus.Add(s);
                await _db.SaveChangesAsync();

                if (sm.PermissionNames?.Any() == true)
                {
                    foreach (var pid in sm.PermissionNames)
                    {
                        _db.SubMenuPermissions.Add(new SubMenuPermission { SubMenuId = s.Id, PermissionName = pid });
                    }
                    await _db.SaveChangesAsync();
                }
            }

            return await GetMenuByIdAsync(m.Id) ?? throw new InvalidOperationException("Failed to load created menu");
        }

        public async Task<bool> UpdateMenuAsync(int id, MenuDto dto)
        {
            var m = await _db.Menus
                .Include(x => x.SubMenus)
                    .ThenInclude(s => s.SubMenuPermissions)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (m == null) return false;

            m.Title = dto.Title;
            m.Icon = dto.Icon;
            m.SortOrder = dto.SortOrder;
            m.IsActive = dto.IsActive;

            // sync submenus (add new, update existing, remove missing)
            foreach (var smDto in dto.SubMenus)
            {
                var existing = m.SubMenus.FirstOrDefault(s => s.Id == smDto.Id && smDto.Id > 0);
                if (existing == null)
                {
                    var newSm = new SubMenu
                    {
                        MenuId = m.Id,
                        Title = smDto.Title,
                        Route = smDto.Route,
                        SortOrder = smDto.SortOrder,
                        IsActive = smDto.IsActive
                    };
                    _db.SubMenus.Add(newSm);
                    await _db.SaveChangesAsync();

                    if (smDto.PermissionNames?.Any() == true)
                    {
                        foreach (var pid in smDto.PermissionNames)
                            _db.SubMenuPermissions.Add(new SubMenuPermission { SubMenuId = newSm.Id, PermissionName = pid });
                    }
                }
                else
                {
                    existing.Title = smDto.Title;
                    existing.Route = smDto.Route;
                    existing.SortOrder = smDto.SortOrder;
                    existing.IsActive = smDto.IsActive;

                    // Replace permissions list simply
                    var existingPerms = existing.SubMenuPermissions.ToList();
                    if (existingPerms.Any())
                        _db.SubMenuPermissions.RemoveRange(existingPerms);

                    if (smDto.PermissionNames?.Any() == true)
                        foreach (var pid in smDto.PermissionNames)
                            _db.SubMenuPermissions.Add(new SubMenuPermission { SubMenuId = existing.Id, PermissionName = pid });
                }
            }

            // remove submenus not in dto
            var dtoIds = dto.SubMenus.Where(s => s.Id > 0).Select(s => s.Id).ToHashSet();
            var toRemove = m.SubMenus.Where(s => !dtoIds.Contains(s.Id)).ToList();
            if (toRemove.Any())
            {
                foreach (var r in toRemove) _db.SubMenus.Remove(r);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMenuAsync(int id)
        {
            var m = await _db.Menus.FindAsync(id);
            if (m == null) return false;
            _db.Menus.Remove(m);
            await _db.SaveChangesAsync();
            return true;
        }

        // ----- User-facing: return only menus/submenus the user has permission for -----
        public async Task<IEnumerable<MenuDto>> GetMenusForUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            // 1. Fetch user's effective permissions from AuthModule
            if (!_cache.TryGetValue<HashSet<string>>(userId, out var effectivePermissions))
            {
                var response = await _httpClient.GetAsync($"api/auth/users/{userId}/permissions");
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Failed to fetch user permissions from AuthModule");

                var userPermissionsDto = await response.Content.ReadFromJsonAsync<UserPermissionDto>();
                effectivePermissions = userPermissionsDto?.Permissions?.ToHashSet() ?? new HashSet<string>();

                _cache.Set(userId, effectivePermissions, TimeSpan.FromMinutes(15));
            }

            // 2. Fetch all menus/submenus
            var menus = await _db.Menus
                .Include(m => m.SubMenus)
                    .ThenInclude(sm => sm.SubMenuPermissions)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            // 3. Filter submenus based on user's effective permissions
            var result = new List<MenuDto>();
            foreach (var menu in menus)
            {
                var allowedSubs = menu.SubMenus
                    .Where(sm => sm.IsActive)
                    .Where(sm => sm.SubMenuPermissions.Any()
                        ? sm.SubMenuPermissions.Any(sp => effectivePermissions.Contains(sp.PermissionName))
                        : true) // Show submenu if no permissions assigned
                    .OrderBy(sm => sm.SortOrder)
                    .ToList();

                if (!allowedSubs.Any()) continue;

                result.Add(new MenuDto
                {
                    Id = menu.Id,
                    Title = menu.Title,
                    Icon = menu.Icon,
                    SortOrder = menu.SortOrder,
                    IsActive = menu.IsActive,
                    SubMenus = allowedSubs.Select(sm => new SubMenuDto
                    {
                        Id = sm.Id,
                        MenuId = sm.MenuId,
                        Title = sm.Title,
                        Route = sm.Route,
                        SortOrder = sm.SortOrder,
                        IsActive = sm.IsActive,
                        PermissionNames = sm.SubMenuPermissions.Select(sp => sp.PermissionName).ToList()
                    }).ToList()
                });
            }

            return result;
        }

        // helper mapping
        private static MenuDto MapToDto(Menu m)
        {
            return new MenuDto
            {
                Id = m.Id,
                Title = m.Title,
                Icon = m.Icon,
                SortOrder = m.SortOrder,
                IsActive = m.IsActive,
                SubMenus = m.SubMenus.OrderBy(s => s.SortOrder).Select(s => new SubMenuDto
                {
                    Id = s.Id,
                    MenuId = s.MenuId,
                    Title = s.Title,
                    Route = s.Route,
                    SortOrder = s.SortOrder,
                    IsActive = s.IsActive,
                    PermissionNames = s.SubMenuPermissions.Select(sp => sp.PermissionName).ToList(),
                    PermissionIds = null // cannot resolve here without AuthModule call
                }).ToList()
            };
        }
    }
}
