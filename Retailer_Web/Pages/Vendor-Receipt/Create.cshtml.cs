using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.VendorPayment
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;
        [BindProperty]
        public VendorPaymentViewModel VendorPayment { get; set; } = new VendorPaymentViewModel
        {
            CreatedDate = DateTime.Now,
            PaymentDate = DateTime.Now,
            Amount = 0,
            taxAmount = 0,
            taxPercent = 0,
            whtAmount = 0,
            whtPercent = 0,
            totalAmount = 0,
        };
        public List<SelectListItem> vendorsList { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; }

        // ✅ AJAX HANDLER
        public IActionResult OnGetCustomerBalance(int customerId)
        {
            // Example – replace with DB query
            decimal balance = 9999;

            return new JsonResult(balance);
        }

        public async Task OnGetAsync()
        {
            var vendors = await _api.GetVendorsAsync();
            vendorsList = vendors.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
			var paymentmethod = await _api.GetPaymentMethodAsync();
			PaymentMethods = paymentmethod.Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.name }).ToList();
			 

        }
		public async Task<IActionResult> OnGetVendorBalance(int vendorId)
		{

			var balance = await _api.GetVendorBalanceAsync(DateTime.Now, vendorId);

			return new JsonResult(balance);
		}
		public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.CreateVendorPaymentAsync(VendorPayment);
            return RedirectToPage("Index");
        }
    }
}
