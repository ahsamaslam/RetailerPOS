using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;

namespace Retailer.Web.ViewComponents
{
    [ViewComponent(Name = "sideBarVc")]
    public class sideBarVcModel : ViewComponent
    {
        private readonly IApiClient _api;
        public sideBarVcModel(IApiClient api) => _api = api;


        public IEnumerable<MenuDto> Menus { get; set; } = Enumerable.Empty<MenuDto>();

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // if not authenticated redirect to login (safe-guard)
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return View(Enumerable.Empty<MenuDto>()); // empty menu
                                                          //   return RedirectToPage("/Login", new { ReturnUrl = Url.Content("~/") });
            }

            try
            {
                Menus = await _api.GetMenusForCurrentUserAsync();
              //  return Page();
            }
           
            catch (Exception ex)
            {
                // log if you have logger; for now show empty menu and page
                // _logger?.LogError(ex, "Failed to load menus");
                Menus = Enumerable.Empty<MenuDto>();
      
            }
            return View(Menus);
        }
    }
}
