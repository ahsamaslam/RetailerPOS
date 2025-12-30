using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.CustomerPayment
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;
        [BindProperty]
        public CustomerPaymentViewModel CustomerPayment { get; set; } = new CustomerPaymentViewModel
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
        public List<SelectListItem> CustomersList { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; }

        // ✅ AJAX HANDLER
        public async Task< IActionResult> OnGetCustomerBalance(int customerId)
        {

            var balance = await _api.GetCustomersBalanceAsync(DateTime.Now , customerId);
        
            return new JsonResult(balance);
        }

        public async Task OnGetAsync()
        {
            var customers = await _api.GetCustomersAsync();
            CustomersList = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            var paymentmethod = await _api.GetPaymentMethodAsync();
            PaymentMethods = paymentmethod.Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.name }).ToList();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.CreateCustomerPaymentAsync(CustomerPayment);
            return RedirectToPage("Index");
        }
    }
}
