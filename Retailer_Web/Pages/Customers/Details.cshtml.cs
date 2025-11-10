using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.Customers
{
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _api;
        public DetailsModel(IApiClient api) => _api = api;

        public CustomerViewModel Customer { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var customer = await _api.GetCustomerByIdAsync(id);
            if (customer != null) Customer = customer;
        }
    }
}
