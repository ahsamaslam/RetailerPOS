using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.Payment
{
    public class EditModel : PageModel
    {
		private readonly IApiClient _api;
		public EditModel(IApiClient api) => _api = api;
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

		public async Task OnGetAsync(int id)
		{


			var vendors = await _api.GetVendorsAsync();
			vendorsList = vendors.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
			var paymentmethod = await _api.GetPaymentMethodAsync();
			PaymentMethods = paymentmethod.Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.name }).ToList();
			VendorPayment = await _api.GetVendorpaymentByIdAsync(id); // fetch DTO from API

		}
		public async Task<IActionResult> OnGetVendorBalance(int vendorId)
		{

			var balance = await _api.GetVendorBalanceAsync(DateTime.Now, vendorId);

			return new JsonResult(balance);
		}
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid) return Page();

			// recompute totals server-side

			var dto = new VendorPaymentDto
			{
				Id = VendorPayment.Id,
				VendorId = VendorPayment.VendorId,
				Type = VendorPayment.Type,
				Amount = VendorPayment.Amount,
				PaymentDate = VendorPayment.PaymentDate,
				PaymentMethodId = VendorPayment.PaymentMethodId,
				taxPercent = VendorPayment.taxPercent,
				taxAmount = VendorPayment.taxAmount,
				whtPercent = VendorPayment.whtPercent,
				whtAmount = VendorPayment.whtAmount,
				totalAmount = VendorPayment.totalAmount,
				companyId = VendorPayment.companyId,
				status = VendorPayment.status
				// Add other properties if needed
			};
			var success = await _api.UpdateVendorPaymentAsync(dto); // call Update API
			if (!success)
			{
				ModelState.AddModelError("", "Unable to update sale.");
				return Page();
			}

			return RedirectToPage("Index");
		}
	}
}
