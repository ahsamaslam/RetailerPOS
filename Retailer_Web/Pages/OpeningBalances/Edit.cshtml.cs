using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.OpeningBalances;
public class EditModel : PageModel
{
    private readonly ILogger<EditModel> _logger;
    private readonly IToastNotification _toastNotification;
    private readonly IApiClient _api;

    public EditModel(IApiClient api, ILogger<EditModel> logger, IToastNotification toastNotification)
    {
        _api = api;
        _logger = logger;
        _toastNotification = toastNotification;
    }

    [BindProperty]
    public UpdateOpeningBalanceDto Editing { get; set; } = new(0, DateTime.UtcNow.Year, 0, 0m);

    public List<ItemDto> Products { get; set; } = new();
    public SelectList ProductSelectList => new(Products, nameof(ItemDto.Id), nameof(ItemDto.Name));

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var vm = await _api.GetOpeningBalanceAsync(id);
        if (vm == null) return NotFound();

        // map to editing DTO (assumes OpeningBalanceViewModel has ProductId)
        Editing = new UpdateOpeningBalanceDto(vm.Id, vm.Year, vm.ProductId, vm.OpeningQuantity);

        Products = await _api.GetItemsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Products = await _api.GetItemsAsync();
            return Page();
        }

        var result = await _api.UpdateOpeningBalanceAsync(Editing.Id, Editing);
        if (result.Success)
        {
            _toastNotification.AddSuccessToastMessage("Opening balance updated.");
            return RedirectToPage("./Index");
        }

        _toastNotification.AddErrorToastMessage(result.ErrorMessage ?? "Error updating opening balance.");
        if (result.ErrorMessage?.Contains("exists") == true)
            ModelState.AddModelError(string.Empty, result.ErrorMessage);

        Products = await _api.GetItemsAsync();
        return Page();
    }
}
