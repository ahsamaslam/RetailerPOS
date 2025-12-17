using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Helpers;
using Retailer.Web.Models;
using Retailer.Web.Services.Layout;

namespace Retailer.Web.ViewComponents
{
    public class SidebarViewModel
    {
        public IEnumerable<MenuDto> Menus { get; set; }
            = Enumerable.Empty<MenuDto>();
        public LayoutUserInfo? User { get; set; }
    }
    public class SidebarViewComponent : ViewComponent
    {
        private readonly ILayoutContext _layout;

        public SidebarViewComponent(ILayoutContext layout)
        {
            _layout = layout;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menus = await _layout.GetMenusAsync();
            var userInfo = await _layout.GetUserInfoAsync();

            if (userInfo == null || menus == null)
                return Content(string.Empty); // or minimal topbar


            return View(new SidebarViewModel
            {
                Menus = menus,
                User = userInfo
            });

        }
    }
}
