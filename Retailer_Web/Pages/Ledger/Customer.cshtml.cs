using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Dtos;
using Retailer.Web.Models.Ledger;

namespace Retailer.Web.Pages.Ledger
{
    public class CustomerModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        private readonly IApiClient _api;
        public CustomerModel(IApiClient api) { _api = api; }
         
        public int CustomerCode { get; set; }
        public CustomerDto Customer { get; set; }
        public List<CustomerLedgerDto> ledgers { get; set; }= new();    
        public DateTime sdate { get; set; } = DateTime.Now;
        public DateTime edate { get; set; } = DateTime.Now;
        public List<SelectListItem> CustomersList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(DateTime? sdate , DateTime? edate  ,int status)
        {
       
             
           var customers = await _api.GetCustomersAsync();

            CustomersList = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

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
			var data = await _api.GetCustomerLedgerAsync( sdate, edate, customerCode);
			return new JsonResult(data);
		}

	}
}
