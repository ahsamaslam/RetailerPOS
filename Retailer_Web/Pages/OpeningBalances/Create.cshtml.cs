// Pages/OpeningBalances/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.OpeningBalances;
public class CreateModel : BasePageModel
{
    private readonly ILogger<CreateModel> _logger;
    private readonly IToastNotification _toastNotification;
    private readonly IApiClient _api;

    public CreateModel(IApiClient api, ILogger<CreateModel> logger, IToastNotification toastNotification):base(api)
    {
        _api = api;
        _logger = logger;
        _toastNotification = toastNotification;
    }

    [BindProperty]
    public CreateOpeningBalanceDto NewOpening { get; set; } = new(DateTime.UtcNow.Year, 0, 0m);
    public List<ItemDto> Products { get; set; } = new();
    public async Task OnGetAsync()
    {
        Products = await _api.GetItemsAsync();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Products = await _api.GetItemsAsync();
            return Page();
        }
        var resp = await _api.CreateOpeningBalanceAsync(NewOpening);
        if (resp.Success)
        {
            _toastNotification.AddSuccessToastMessage("Opening balance created.");
            return RedirectToPage("./Index");
        }

        _toastNotification.AddErrorToastMessage(resp.ErrorMessage ?? "Error creating opening balance.");
        // If conflict, show model error too
        if (resp.ErrorMessage?.Contains("exists") == true)
            ModelState.AddModelError(string.Empty, resp.ErrorMessage);

        return Page();
    }
}
