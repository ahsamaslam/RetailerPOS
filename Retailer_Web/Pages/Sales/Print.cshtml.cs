using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Sales;

[Authorize]
public class PrintModel : BasePageModel
    {
        private readonly IApiClient _api;
        public PrintModel(IApiClient api) => _api = api;
        [BindProperty]
        public SalesMasterDto Sale { get; set; } = new();
    public SaleInvoiceSettingDto Settings { get; set; }
    public CompanyDto company { get; set; }
    public async Task<IActionResult> OnGetAsync(int id)
        {
        company = await _api.GetUserCompanyAsync(); // fetch DTO from API
        Settings = await _api.GetSalePrintSetting(1); // fetch DTO from API
           var sale = await _api.GetSaleByIdAsync(id); // fetch DTO from API
        foreach (var item in sale.Details)
        {
            item.Amount =  Math.Round(item.Rate  ) * item.Qty;

        }
        Sale = sale;
        if (Sale == null) return NotFound();
            return Page();
        }
    
}
