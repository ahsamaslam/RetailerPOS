using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Customers
{
    public class DetailsModel : BasePageModel
    {
        private readonly IApiClient _api;
        public DetailsModel(IApiClient api) : base(api) { _api = api; }

        public CustomerViewModel Customer { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var customer = await _api.GetCustomerByIdAsync(id);
            if (customer != null) Customer = customer;
        }
    }
}
