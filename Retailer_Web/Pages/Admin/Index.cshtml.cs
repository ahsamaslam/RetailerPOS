using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;

namespace Retailer.Web.Pages.Admin
{
    public class IndexModel : BasePageModel
    {
        public IndexModel(IApiClient api) : base(api)
        {
        }

        public void OnGet()
        {
            // Nothing needed here, static dashboard links
        }
    }
}
