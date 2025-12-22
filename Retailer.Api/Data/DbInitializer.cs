using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.Data;

namespace Retailer.Api.Data
{
    public class DbInitializer : IDbInitializer
    {
        // Pages and actions used to create menus/submenus
        private static readonly List<MenuDto> Pages = new List<MenuDto>() { 
            new MenuDto() { Title="Branches", Icon="fa fa-code-branch" },
            new MenuDto() { Title="Categories", Icon="fa fa-list" },
            new MenuDto() { Title="Customers", Icon="fa fa-users" },
            new MenuDto() { Title="Groups", Icon="fa fa-layer-group" },
            new MenuDto() { Title="Items", Icon="fa fa-boxes" },
            new MenuDto() { Title="ItemType", Icon="fa fa-tags" },
            new MenuDto() { Title="OpeningBalances", Icon="fa fa-balance-scale" },
            new MenuDto() { Title="Sales", Icon="fa fa-shopping-cart" },
            new MenuDto() { Title="Purchases", Icon="fa fa-shopping-bag" },
            new MenuDto() { Title="SubGroups", Icon="fa fa-object-group" },
            new MenuDto() { Title="Vendors", Icon="fa fa-truck" }


        };

        private static readonly string[] PageActions = new[] { "View", "Create", "Edit", "Delete" };

        /// <summary>
        /// Initialize Menu and SubMenu tables based on hard-coded Pages & PageActions.
        /// Idempotent: will not create duplicate menus/submenus.
        /// </summary>
        private readonly RetailerDbContext apiDb;
        public DbInitializer(RetailerDbContext db)
        {
            apiDb = db;
        }
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
           

            // build action ordering map so submenus get a predictable SortOrder
            var actionOrder = PageActions.Select((a, idx) => (Action: a, Order: idx))
                                         .ToDictionary(x => x.Action, x => x.Order, StringComparer.OrdinalIgnoreCase);

            for (int pageIdx = 0; pageIdx < Pages.Count; pageIdx++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var pageName = Pages[pageIdx];
                var pageNameUpper = pageName.Title.ToUpper(); // normalize once

                // find existing menu (case-insensitive) - avoid StringComparison overload
                var menu = await apiDb.Menus
                   .FirstOrDefaultAsync(m => m.Title != null && m.Title.ToUpper() == pageNameUpper, cancellationToken);

                if (menu == null)
                {
                    menu = new Menu
                    {
                        Title = pageName.Title,
                        Icon = pageName.Icon,      // optionally set default icon or map per-page
                        SortOrder = pageIdx,
                        IsActive = true
                    };

                    apiDb.Menus.Add(menu);
                    // Save so Menu.Id is populated (necessary for SubMenu.MenuId)
                    await apiDb.SaveChangesAsync(cancellationToken);
                }

                // For each configured action create a submenu if missing
                foreach (var action in PageActions)
                {
                    // Submenu title — adjust if you prefer "View Items" instead of "View"
                    var subTitle = action;
                    var subTitleUpper = subTitle.ToUpper();

                    // route mapping - adjust if your pages live elsewhere
                    string? route = action.ToLowerInvariant() switch
                    {
                        "view" => $"/{pageName.Title}",
                        "create" => $"/{pageName.Title}/Create",
                        "edit" => $"/{pageName.Title}/Edit",
                        "delete" => null, // typically delete is an action, not a page
                        _ => $"/{pageName.Title}/{action}"
                    };

                    // Skip if submenu (by title) already exists for this menu
                    var exists = await apiDb.SubMenus
                        .AnyAsync(s => s.MenuId == menu.Id && s.Title != null && s.Title.ToUpper() == subTitleUpper, cancellationToken);

                    if (exists) continue;

                    var sub = new SubMenu
                    {
                        MenuId = menu.Id,
                        Title = subTitle,
                        Route = route,
                        SortOrder = actionOrder.ContainsKey(action) ? actionOrder[action] : 999,
                        IsActive = true
                    };

                    apiDb.SubMenus.Add(sub);
                }

                // persist created submenus for this menu
                await apiDb.SaveChangesAsync(cancellationToken);
            } // end pages loop
        }
    }
}
