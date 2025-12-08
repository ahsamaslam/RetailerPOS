using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Sales
{
    public class IndexModel : BasePageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) : base(api) => _api = api;
        [BindProperty(SupportsGet = true)]
        public DateTime sdate { get; set; } = DateTime.Now; 

        [BindProperty(SupportsGet = true)]
        public DateTime edate { get; set; } = DateTime.Now;
        public List<SalesMasterDto> Sales { get; set; } = new();

        public async Task OnGetAsync()
        {

            Sales = (await _api.GetAllSaleDateWise(sdate, edate)).ToList();
            // Sales = (await _api.GetAllSaleDateWise(sdate,edate)).ToList();
        }
    }
}
