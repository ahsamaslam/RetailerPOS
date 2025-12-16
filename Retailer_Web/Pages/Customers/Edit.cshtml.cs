using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Dtos;
using Retailer.Web.Models;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Customers
{
    public class EditModel : BasePageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) : base(api) { _api = api; }

        [BindProperty]
        public CustomerViewModel Customer { get; set; } = new();
        public List<ItemSelectListItem> Cities { get; set; } = new();
        public List<ItemSelectListItem> PaymentType { get; set; } = Enum.GetValues(typeof(PaymentType))
                   .Cast<PaymentType>()
                   .Select(pt => new ItemSelectListItem
                   {
                       Text = pt.ToString(),           // or use a description attribute
                       Value = ((int)pt).ToString()
                   })
                   .ToList();
        public List<ItemSelectListItem> Provience { get; set; } = new();
        public async Task OnGetAsync(int id)
        {
            var items = await _api.GetCitiesAsync();
            Cities = items.Select(i => new ItemSelectListItem { Value = i.Id.ToString(), Text = i.Name }).ToList();
            var prov = await _api.GetProvienceAsync();
            Provience = prov.Select(i => new ItemSelectListItem { Value = i.id.ToString(), Text = i.name }).ToList();
            var customer = await _api.GetCustomerByIdAsync(id);
            if (customer != null) Customer = customer;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.UpdateCustomerAsync(Customer);
            return RedirectToPage("Index");
        }
    }
}
