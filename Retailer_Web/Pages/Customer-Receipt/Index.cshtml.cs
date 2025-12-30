using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.CustomerPayment
{
    public class IndexModel : PageModel
    {
      

        private readonly IWebHostEnvironment _env;

        private readonly IApiClient _api;
        public IndexModel(IApiClient api )  {   _api = api;   }
        [BindProperty(SupportsGet = true)]
        public DateTime sdate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public DateTime edate { get; set; } = DateTime.Now; 
        [BindProperty]
        public IEnumerable<CustomerPaymentDto> PaymentDetail { get; set; } = new List<CustomerPaymentDto>();
        public async Task FillData() {

            #region payment
            var pd = await _api.GetAllCustomerPaymentDateWise(sdate, edate);

            foreach (var item in pd)
            {

                item.CustomerName = item.Customer.Name ?? "";
                if (item.Bank != null)
                    item.bankName = item.Bank.AccountName;
                if (item.PaymentMethod != null)
                {
                    item.PaymentMethodName = item.PaymentMethod.Name;
                }

            }

            PaymentDetail = pd;
        }
        public async Task OnGetAsync()
        {
           await FillData();
            #endregion
        }
        // ✅ AJAX Soft Delete Handler
        public async Task< IActionResult>   OnPostDeleteAsync(int id)
        {
            var payment = await _api.DeleteCustomerPaymentAsync(id);
            if (!payment) 
                return NotFound();
            return Page();
        }

    }
}
