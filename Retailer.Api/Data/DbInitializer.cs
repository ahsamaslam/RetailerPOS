using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.Data;
using System;

namespace Retailer.Api.Data
{
    public class DbInitializer : IDbInitializer
    {
        // Pages and actions used to create menus/submenus
        private static readonly List<MenuDto> Pages = new List<MenuDto>() { 
            new MenuDto() { UrlTitle="branches",Title = "Branches" , Icon="fa fa-code-branch" },
            new MenuDto() { UrlTitle="#",Title="Products", Icon="fa fa-boxes",SubMenus = new List<SubMenuDto>(){ 
                new SubMenuDto() { UrlTitle="categories", Title="Categories" },
                new SubMenuDto() { UrlTitle="groups", Title = "Groups" },
                new SubMenuDto() { UrlTitle="itemtype", Title= "Item Type" },
                new SubMenuDto() { UrlTitle="subgroups", Title="Sub Groups" },
                new SubMenuDto() { UrlTitle="items", Title="Items" },
            }},
            new MenuDto() { UrlTitle="customers", Icon="fa fa-users",Title="Customers" },
            new MenuDto() { UrlTitle="vendors", Icon="fa fa-truck", Title = "Vendors" },
            new MenuDto() { UrlTitle="Banks", Icon="fa fa-truck", Title = "Bank" },
            new MenuDto() { UrlTitle="openingbalances", Icon="fa fa-balance-scale", Title = "Opening Balances"  },
            new MenuDto() { UrlTitle="#",Title="Sales", Icon="fa fa-shopping-cart",SubMenus = new List<SubMenuDto>(){
                new SubMenuDto() { UrlTitle="sales", Title="Sales" },
                new SubMenuDto() { UrlTitle="saleReturn", Title="Sales Return" },
            }},
            new MenuDto() { UrlTitle="#",Title="Purchases", Icon="fa fa-shopping-bag",SubMenus = new List<SubMenuDto>(){
                new SubMenuDto() { UrlTitle="purchases", Title="Purchases" },
                new SubMenuDto() { UrlTitle="purchases-return", Title="Purchases Return" }
            }},
            new MenuDto() { UrlTitle="#",Title="Receipts", Icon="fa fa-receipt",SubMenus = new List<SubMenuDto>(){ 
                new SubMenuDto() { UrlTitle="customer-receipt", Title="Customer Receipt" },
                new SubMenuDto() { UrlTitle="vendor-receipt", Title="Vendor Receipt" }, 
            }},
             new MenuDto() { UrlTitle="#",Title="Ledger", Icon="fa fa-receipt",SubMenus = new List<SubMenuDto>(){
                new SubMenuDto() { UrlTitle="Ledger/Customer", Title="Customer Ledger" }, 
                new SubMenuDto() { UrlTitle="Ledger/Vendor", Title="Vendor Ledger" }, 
            }},
				 new MenuDto() { UrlTitle="#",Title="Report", Icon="fa fa-receipt",SubMenus = new List<SubMenuDto>(){
				new SubMenuDto() { UrlTitle="Report/StockReport", Title="Stock Report Ledger" }, 
			}},

            //new MenuDto() { UrlTitle="categories",Title="Categories", Icon="fa fa-list" },
            //new MenuDto() { UrlTitle="groups", Title = "Groups", Icon="fa fa-layer-group" },
            //new MenuDto() { UrlTitle="itemType", Icon="fa fa-tags" },
            //new MenuDto() { UrlTitle="subgroups", Icon="fa fa-object-group" },
            //new MenuDto() { UrlTitle="items", Icon="fa fa-boxes" },
            // new MenuDto() { UrlTitle="sales", Icon="fa fa-shopping-cart" },
            //new MenuDto() { UrlTitle="sales-return", Icon="fa fa-shopping-cart" },
            //new MenuDto() { UrlTitle="purchases", Icon="fa fa-shopping-bag" },
            //new MenuDto() { UrlTitle="purchases-return", Icon="fa fa-shopping-bag" },
            //new MenuDto() { UrlTitle="customer-receipt", Icon="fa fa-receipt" },
            //new MenuDto() { UrlTitle="vendor-receipt", Icon="fa fa-receipt" },
            //new MenuDto() { UrlTitle="bank-receipt", Icon="fa fa-receipt" },
        };

        //private static readonly string[] PageActions = new[] { "View", "Create", "Edit", "Delete" };

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
            //var actionOrder = PageActions.Select((a, idx) => (Action: a, Order: idx))
            //                             .ToDictionary(x => x.Action, x => x.Order, StringComparer.OrdinalIgnoreCase);

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
                        UrlTitle = pageName.UrlTitle,
                        Icon = pageName.Icon,      // optionally set default icon or map per-page
                        SortOrder = pageIdx,
                        IsActive = true
                    };

                    apiDb.Menus.Add(menu);
                    // Save so Menu.Id is populated (necessary for SubMenu.MenuId)
                    await apiDb.SaveChangesAsync(cancellationToken);
                }
                // For each configured action create a submenu if missing
                if (pageName.SubMenus.Count > 0)
                {
                    int x = 0;
                    foreach(var s in pageName.SubMenus)
                    {
                        // Submenu title — adjust if you prefer "View Items" instead of "View"
                        //var subTitleUpper = subTitle.ToUpper();

                        //// route mapping - adjust if your pages live elsewhere
                        //string? route = action.ToLowerInvariant() switch
                        //{
                        //    "view" => $"/{pageName.Title}",
                        //    "create" => $"/{pageName.Title}/Create",
                        //    "edit" => $"/{pageName.Title}/Edit",
                        //    "delete" => null, // typically delete is an action, not a page
                        //    _ => $"/{pageName.Title}/{action}"
                        //};

                        // Skip if submenu (by title) already exists for this menu
                        var exists = await apiDb.SubMenus
                            .AnyAsync(s => s.MenuId == menu.Id && s.Title != null && s.Title == s.Title, cancellationToken);

                        if (exists) continue;

                        var sub = new SubMenu
                        {
                            MenuId = menu.Id,
                            Title = s.Title,
                            UrlTitle = s.UrlTitle,
                            SortOrder = x,
                            IsActive = true
                        };

                        apiDb.SubMenus.Add(sub);
                        x++;
                    }
                    
                }

                // persist created submenus for this menu
                await apiDb.SaveChangesAsync(cancellationToken);
            } // end pages loop
        }
    }
}
