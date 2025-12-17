using Microsoft.AspNetCore.Http;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Helpers;
using Retailer.Web.Models;
using System.IdentityModel.Claims;

namespace Retailer.Web.Services.Layout
{
    public class LayoutContext : ILayoutContext
    {
        private readonly IApiClient _api;
        private readonly IHttpContextAccessor _http;

        private LayoutUserInfo? _userInfo;
        private IEnumerable<MenuDto>? _menus;

        public LayoutContext(
            IApiClient api,
            IHttpContextAccessor http)
        {
            _api = api;
            _http = http;
        }

        public async Task<LayoutUserInfo?> GetUserInfoAsync()
        {
            if (_userInfo != null)
                return _userInfo;

            var httpContext = _http.HttpContext;

            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return null;

            try
            {
                var user = httpContext.User;

                var info = new LayoutUserInfo
                {
                    UserName =
                        user.FindFirst("name")?.Value ??
                        user.Identity?.Name ??
                        "User",
                    AvatarUrl = user.FindFirst("picture")?.Value,
                    IsAdmin = user.IsInRole("admin")
                };

                var company = await _api.GetUserCompanyAsync();
                info.companyName = company?.Name;

                _userInfo = info;
                return info;
            }
            catch (ApiUnauthorizedException)
            {
                // token expired / invalid → force sign-out
                httpContext.Response.Redirect("/Login");
                return null;
            }
        }
        public async Task<IEnumerable<MenuDto>> GetMenusAsync()
        {

            if (_menus != null)
                return _menus;

            var httpContext = _http.HttpContext;

            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return null;
            try
            {

                _menus = await _api.GetMenusForCurrentUserAsync()
                         ?? Enumerable.Empty<MenuDto>();

                return _menus;
            }
            catch (ApiUnauthorizedException)
            {
                // token expired / invalid → force sign-out
                httpContext.Response.Redirect("/Login");
                return null;
            }

        }
    }
}
