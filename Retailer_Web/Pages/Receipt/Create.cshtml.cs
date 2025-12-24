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
        public IActionResult OnGetCustomerBalance(int customerId)
        {
            // Example – replace with DB query
            decimal balance =9999;

            return new JsonResult(balance);
        }

        public async Task OnGetAsync()
        {
            var customers = await _api.GetCustomersAsync();
            CustomersList = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            PaymentMethods = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Cash" },
                new SelectListItem { Value = "2", Text = "Bank Transfer" },
                new SelectListItem { Value = "3", Text = "Credit Card" }
            };
          
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.CreateCustomerPaymentAsync(CustomerPayment);
            return RedirectToPage("Index");
        }
    }
}
