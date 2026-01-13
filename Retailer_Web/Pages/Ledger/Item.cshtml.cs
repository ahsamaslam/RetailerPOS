using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models.Ledger;

namespace Retailer.Web.Pages.Ledger
{
    public class ItemModel : PageModel
    {

        private readonly IWebHostEnvironment _env;

        private readonly IApiClient _api;
        public ItemModel(IApiClient api) { _api = api; }

        public int ItemCode { get; set; }
        public ItemDto Item { get; set; }
        public List<CustomerLedgerDto> ledgers { get; set; } = new();
        public DateTime sdate { get; set; } = DateTime.Now;
        public DateTime edate { get; set; } = DateTime.Now;
        public List<SelectListItem> ItemList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(DateTime? sdate, DateTime? edate, int status)
        {


            var items = await _api.GetItemsAsync();

            ItemList = items.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            return Page();
        }
        //      public async Task<IActionResult> OnGetLoadDataAsync(DateTime? sdate, DateTime? edate, int status)
        //      {
        //           var data = await _api.GetCustomerLedgerAsync(sdate??DateTime.Now, edate ?? DateTime.Now, status);
        //          ledgers = (List<CustomerLedgerDto>)data;
        //          return new JsonResult(data);
        //         //     return new JsonResult("");
        //      }
        //// Rename handler to LoadData
        public async Task<IActionResult> OnGetLoadDataAsync(
            int customerCode,
            DateTime sdate,
            DateTime edate)
        {
            var data = await _api.GetItemLedgerAsync(sdate, edate, customerCode);
            return new JsonResult(data);
        }

    }
}
