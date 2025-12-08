using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.Web.Pages.Report
{
    public class StockReportModel : BasePageModel
    {
        private readonly IApiClient _api;

        public StockReportModel(IApiClient api) : base(api) => _api = api;

        [BindProperty]
        public CreateItemDto Input { get; set; } = new();

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Groups { get; set; } = new List<SelectListItem>();

        public async Task OnGetAsync()
        {
            // Fetch categories and convert to List to allow Insert
            var categories = (await _api.GetCategoriesAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();

            // Add "All" option at the beginning
            categories.Insert(0, new SelectListItem("All", "0"));
            Categories = categories;

            // Fetch groups and convert to List
            var groups = (await _api.GetGroupsAsync())
                .Select(g => new SelectListItem(g.Name, g.Id.ToString()))
                .ToList();

            // Add "All" option at the beginning
            groups.Insert(0, new SelectListItem("All", "0"));
            Groups = groups;
            await OnGetItemsAsync();
        }
        public async Task<IActionResult> OnGetItemsAsync(int categoryId = 0, int groupId = 0)
        {
            // Interpret 0 as "All"
            IEnumerable<ItemDto> items = await _api.GetStockItemsAsync(  categoryId  , groupId ); // implement this on API client
      
          
            // Map or select only the fields you want to return in the JSON
            var result = items.Select(i => new {
                i.Id,
                i.Name,
                Qty = i.QtyInHand, // adjust property name to your DTO (Qty / Quantity / Stock)
                CategoryName = i.CategoryName,
                GroupName = i.GroupName
            });

            return new JsonResult(result);
        }
    }
}
 
