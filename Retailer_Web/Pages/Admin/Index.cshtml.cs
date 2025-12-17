using Microsoft.AspNetCore.Authorization;

namespace Retailer.Web.Pages.Admin
{
    [Authorize]
    public class IndexModel : BasePageModel
    {
        public IndexModel()
        {
        }

        public void OnGet()
        {
            // Nothing needed here, static dashboard links
        }
    }
}
