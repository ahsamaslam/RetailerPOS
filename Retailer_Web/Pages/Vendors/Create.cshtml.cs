using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Dtos;
using Retailer.Web.Models;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Vendors
{
    [Authorize]
    public class CreateModel : BasePageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public VendorViewModel Vendor { get; set; } = new();
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
        public async Task OnGetAsync()
        {
            var items = await _api.GetCitiesAsync();
            Cities = items.Select(i => new ItemSelectListItem { Value = i.Id.ToString(), Text = i.Name }).ToList();
            var prov = await _api.GetProvienceAsync();
            Provience = prov.Select(i => new ItemSelectListItem { Value = i.id.ToString(), Text = i.name }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.CreateVendorAsync(Vendor);
            return RedirectToPage("Index");
        }
    }
}
