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
        private readonly HttpClient _httpClient;
        private readonly RetailerDbContext _db;
        private readonly IMemoryCache _cache;

        public MenuService(
            RetailerDbContext db,
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _cache = cache;

            // get the named client
            _httpClient = httpClientFactory.CreateClient("AuthModule");
        }
        // Admin: full list
        public async Task<IEnumerable<MenuDto>> GetAllMenusAsync()
        {
            var menus = await _db.Menus
                .Include(m => m.SubMenus)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            return menus.Select(MapToDto);
        }

        public async Task<MenuDto?> GetMenuByIdAsync(int id)
        {
            var m = await _db.Menus
                .Include(x => x.SubMenus)
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
                    UrlTitle = sm.UrlTitle,
                    SortOrder = sm.SortOrder,
                    IsActive = sm.IsActive
                };
                _db.SubMenus.Add(s);
                await _db.SaveChangesAsync();
            }

            return await GetMenuByIdAsync(m.Id) ?? throw new InvalidOperationException("Failed to load created menu");
        }

        public async Task<bool> UpdateMenuAsync(int id, MenuDto dto)
        {
            var m = await _db.Menus
                .Include(x => x.SubMenus)
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
                        UrlTitle = smDto.UrlTitle,
                        SortOrder = smDto.SortOrder,
                        IsActive = smDto.IsActive
                    };
                    _db.SubMenus.Add(newSm);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    existing.Title = smDto.Title;
                    existing.UrlTitle = smDto.UrlTitle;
                    existing.SortOrder = smDto.SortOrder;
                    existing.IsActive = smDto.IsActive;

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
                var response = await _httpClient.GetAsync($"api/authuser/permissions");
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Failed to fetch user permissions from AuthModule");

                var userPermissionsDto = await response.Content.ReadFromJsonAsync<UserPermissionDto>();
                effectivePermissions = userPermissionsDto?.Permissions?.ToHashSet() ?? new HashSet<string>();

                _cache.Set(userId, effectivePermissions, TimeSpan.FromMinutes(15));
            }

            // 2. Fetch all menus/submenus
            var menus = await _db.Menus
                .Include(m => m.SubMenus)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            // 3. Filter submenus based on user's effective permissions
            var result = new List<MenuDto>();
            foreach (var menu in menus)
            {


                if(menu.SubMenus.Count > 0)
                {
                    result.Add(new MenuDto
                    {
                        Id = menu.Id,
                        Title = menu.Title,
                        UrlTitle = menu.UrlTitle,
                        Icon = menu.Icon,
                        SortOrder = menu.SortOrder,
                        IsActive = menu.IsActive,
                        SubMenus = menu.SubMenus.Select(sm => new SubMenuDto
                        {
                            Id = sm.Id,
                            MenuId = sm.MenuId,
                            Title = sm.Title,
                            UrlTitle = sm.UrlTitle,
                            SortOrder = sm.SortOrder,
                            IsActive = sm.IsActive,
                            PermissionNames = effectivePermissions.Select(x => x).Where(x => x.Contains(sm.UrlTitle)).ToList()
                        }).ToList()
                    });
                }
                else
                {
                    result.Add(new MenuDto
                    {
                        Id = menu.Id,
                        Title = menu.Title,
                        UrlTitle = menu.UrlTitle,
                        Icon = menu.Icon,
                        SortOrder = menu.SortOrder,
                        IsActive = menu.IsActive,
                        PermissionNames = effectivePermissions.Select(x => x).Where(x => x.Contains(menu.UrlTitle)).ToList()
                    });
                }
                
            }

            return result;
        }
        public async Task<SubMenuDto?> CreateSubMenuAsync(int menuId, SubMenuDto dto)
        {
            // Validate parent menu exists
            var parentMenu = await _db.Menus
                .FirstOrDefaultAsync(m => m.Id == menuId);

            if (parentMenu == null)
                return null;

            // Create entity
            var sub = new SubMenu
            {
                MenuId = menuId,
                Title = dto.Title,
                UrlTitle = dto.UrlTitle,
                SortOrder = dto.SortOrder,
                IsActive = dto.IsActive
            };

            _db.SubMenus.Add(sub);
            await _db.SaveChangesAsync();

            // Map back to DTO
            return new SubMenuDto
            {
                Id = sub.Id,
                MenuId = sub.MenuId,
                Title = sub.Title,
                UrlTitle = sub.UrlTitle,
                SortOrder = sub.SortOrder,
                IsActive = sub.IsActive
            };
        }
        public async Task<bool> DeleteSubMenuAsync(int menuId, int subMenuId)
        {
            // Ensure submenu belongs to this menu
            var sub = await _db.SubMenus
                .FirstOrDefaultAsync(s => s.Id == subMenuId && s.MenuId == menuId);

            if (sub == null)
                return false;

            _db.SubMenus.Remove(sub);
            await _db.SaveChangesAsync();

            return true;
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
                    UrlTitle = s.UrlTitle,
                    SortOrder = s.SortOrder,
                    IsActive = s.IsActive
                }).ToList()
            };
        }
    }
}
