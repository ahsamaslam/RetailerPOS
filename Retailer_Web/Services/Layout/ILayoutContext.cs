using Retailer.Web.ApiDTOs;
using Retailer.Web.Models;

namespace Retailer.Web.Services.Layout
{
    public interface ILayoutContext
    {
        Task<LayoutUserInfo?> GetUserInfoAsync();
        Task<IEnumerable<MenuDto>> GetMenusAsync();
    }
}
