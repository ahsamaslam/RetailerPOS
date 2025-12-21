using Microsoft.AspNetCore.Http;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Helpers;
using Retailer.Web.Models;

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
                var session = httpContext.Session;

                var companyNameClaim = user.FindFirst("companyName")?.Value;
                var impersonatedName = session?.GetString("ImpersonatedCompanyName");
                var impersonatedId = session?.GetString("ImpersonatedCompanyId");
                var isSuperAdmin = user.IsInRole("superadmin");

                var info = new LayoutUserInfo
                {
                    UserName = user.FindFirst("name")?.Value ??
                               user.Identity?.Name ??
                               "User",
                    AvatarUrl = user.FindFirst("picture")?.Value ?? "/assets/img/user2-160x160.jpg",
                    companyName = isSuperAdmin
                        ? (string.IsNullOrWhiteSpace(impersonatedName) ? companyNameClaim : impersonatedName)
                        : companyNameClaim,
                    IsAdmin = user.IsInRole("admin"),
                    IsSuperAdmin = isSuperAdmin,
                    HasCompanyContext = !isSuperAdmin || !string.IsNullOrEmpty(impersonatedId),
                    picture = user.FindFirst("picture")?.Value
                };

                _userInfo = info;
                return info;
            }
            catch (ApiUnauthorizedException)
            {
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
            {
                _menus = Enumerable.Empty<MenuDto>();
                return _menus;
            }

            var sessionCompanyId = httpContext.Session?.GetString("ImpersonatedCompanyId");
            var isSuperAdmin = httpContext.User.IsInRole("superadmin");

            if (isSuperAdmin && string.IsNullOrEmpty(sessionCompanyId))
            {
                _menus = Enumerable.Empty<MenuDto>();
                return _menus;
            }

            try
            {
                _menus = await _api.GetMenusForCurrentUserAsync()
                         ?? Enumerable.Empty<MenuDto>();

                return _menus;
            }
            catch (ApiUnauthorizedException)
            {
                httpContext.Response.Redirect("/Login");
                _menus = Enumerable.Empty<MenuDto>();
                return _menus;
            }
        }
    }
}