using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.Receipt
{
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;
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
            decimal balance = 9999;

            return new JsonResult(balance);
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customers = await _api.GetCustomersAsync();
            CustomersList = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            PaymentMethods = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Cash" },
                new SelectListItem { Value = "2", Text = "Bank Transfer" },
                new SelectListItem { Value = "3", Text = "Credit Card" }
            };
            CustomerPayment = await _api.GetCustomerpaymentByIdAsync(id); // fetch DTO from API
            if (CustomerPayment == null) return NotFound();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // recompute totals server-side

            var dto = new CustomerPaymentDto
            {
                Id = CustomerPayment.Id,
                CustomerId = CustomerPayment.CustomerId,
                Type = CustomerPayment.Type,
                Amount = CustomerPayment.Amount,
                PaymentDate = CustomerPayment.PaymentDate,
                PaymentMethodId = CustomerPayment.PaymentMethodId,
                taxPercent = CustomerPayment.taxPercent,
                taxAmount = CustomerPayment.taxAmount,
                whtPercent = CustomerPayment.whtPercent,
                whtAmount = CustomerPayment.whtAmount,
                totalAmount = CustomerPayment.totalAmount,
                companyId = CustomerPayment.companyId,
                status = CustomerPayment.status
                // Add other properties if needed
            };
            var success = await _api.UpdateCustomerPaymentAsync(dto); // call Update API
            if (!success)
            {
                ModelState.AddModelError("", "Unable to update sale.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
