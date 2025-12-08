using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.SubGroups;

public class DetailsModel : BasePageModel
{
    private readonly IApiClient _api;
    public DetailsModel(IApiClient api) : base(api) => _api = api;

    public ItemSubGroupViewModel SubGroup { get; set; } = new();

    public async Task OnGetAsync(int id)
    {
        var sg = await _api.GetSubGroupAsync(id);
        if (sg != null) SubGroup = sg;
    }
}
