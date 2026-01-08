using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Services.Layout;

namespace Retailer.POS.Web.Pages.Reports;

public class ReportsModel : PageModel
{
    private const string LayoutMenusKey = "Menus";
    private const string LayoutUserInfoKey = "_LayoutUserInfo";
    private readonly ILayoutContext _layout;
    private List<MenuDto> _reportMenus = new();

    public ReportsModel(ILayoutContext layout) => _layout = layout;

    public IReadOnlyList<MenuDto> Menus { get; private set; } = Array.Empty<MenuDto>();
    public List<string> TopTabs { get; private set; } = new();
    public List<ReportItem> Reports { get; private set; } = new();
    public string SelectedTab { get; private set; } = "Sales";

    public async Task OnGetAsync(string? tab)
    {
        if (!string.IsNullOrWhiteSpace(tab))
        {
            SelectedTab = tab;
        }

        await LoadLayoutDataAsync();
        LoadTabs();
        LoadReports();
    }

    public IActionResult OnPostToggleFavorite(string code)
    {
        var key = $"fav_{code}";
        TempData[key] = !(TempData[key] as bool? ?? false);
        return new JsonResult(true);
    }

    public IActionResult OnGetOpen(string code)
        => RedirectToPage("/Reports/View", new { code });

    private async Task LoadLayoutDataAsync()
    {
        var menus = (await _layout.GetMenusAsync())
            ?.Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .ToList() ?? new List<MenuDto>();

        Menus = menus;
        HttpContext.Items[LayoutMenusKey] = menus;

        var userInfo = await _layout.GetUserInfoAsync();
        if (userInfo != null)
        {
            HttpContext.Items[LayoutUserInfoKey] = userInfo;
        }
    }

    private void LoadTabs()
    {
        _reportMenus = Menus
            .Where(IsReportMenu)
            .ToList();

        TopTabs = _reportMenus
            .Select(m => m.Title)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (TopTabs.Count == 0)
        {
            TopTabs.Add(SelectedTab);
        }

        var matchedTab = TopTabs
            .FirstOrDefault(tab => tab.Equals(SelectedTab, StringComparison.OrdinalIgnoreCase));

        SelectedTab = matchedTab ?? TopTabs[0];
    }

    private void LoadReports()
    {
        Reports = new List<ReportItem>();
        if (_reportMenus.Count == 0)
        {
            return;
        }

        var selectedMenu = _reportMenus
            .FirstOrDefault(m => m.Title.Equals(SelectedTab, StringComparison.OrdinalIgnoreCase))
            ?? _reportMenus.FirstOrDefault();

        if (selectedMenu?.SubMenus == null)
        {
            return;
        }

        var subMenus = selectedMenu.SubMenus
            .Where(sm => sm.IsActive)
            .OrderBy(sm => sm.SortOrder)
            .ToList();

        Reports = subMenus
            .Select(sm =>
            {
                var code = sm.Id > 0 ? sm.Id.ToString("D3") : sm.Title;
                return new ReportItem(
                    code,
                    sm.Title,
                    IsFav(code),
                    ResolveRoute(sm.UrlTitle));
            })
            .ToList();
    }

    private static bool IsReportMenu(MenuDto menu) =>
        menu.SubMenus?.Any(sm => sm.IsActive && (sm.UrlTitle?.Contains("report", StringComparison.OrdinalIgnoreCase) ?? false)) ?? false;

    private string ResolveRoute(string? route)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            return "#";
        }

        if (route.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return route;
        }

        if (route.StartsWith("/", StringComparison.Ordinal))
        {
            return route;
        }

        return Url.Page(route) ?? "/" + route.TrimStart('/');
    }

    private bool IsFav(string code) => TempData[$"fav_{code}"] as bool? ?? false;
}

public record ReportItem(string Code, string Name, bool IsFavorite, string Url);
