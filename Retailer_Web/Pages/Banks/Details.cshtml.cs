using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Dtos;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.Banks
{
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _api;
        public DetailsModel(IApiClient api) { _api = api; }

        [BindProperty]
        public BanksViewModel Bank { get; set; } = new();
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
            var banks = await _api.GetBankByIdAsync(id);
            if (banks != null) Bank = banks;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.UpdateBankAsync(Bank);
            return RedirectToPage("Index");
        }
    }
}
