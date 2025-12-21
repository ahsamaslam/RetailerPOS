using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Retailer.Web.Pages
{
    [Authorize]
    public class DebugClaimsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
