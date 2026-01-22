using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailerMobileApp.Core.Constants;
using RetailerMobileApp.Core.Interfaces;
using RetailerMobileApp.ViewModels;

namespace RetailerMobileApp.Features.Dashboard.ViewModels;

public record DashboardShortcut(string Title, string Description, string Route, Color AccentColor);

public partial class DashboardViewModel : BaseViewModel
{
    private readonly ITokenStorageService _tokenStorageService;

    public ObservableCollection<DashboardShortcut> PrimaryActions { get; } = new();

    [ObservableProperty]
    private string _userDisplayName = "Retail Manager";

    [ObservableProperty]
    private string _userRole = "Store Admin";

    [ObservableProperty]
    private string _userInitials = "RM";

    [ObservableProperty]
    private bool _isProfileMenuVisible;

    public DashboardViewModel(ITokenStorageService tokenStorageService)
    {
        _tokenStorageService = tokenStorageService;
        Title = "Home";
        SeedActions();
    }

    private void SeedActions()
    {
        PrimaryActions.Clear();
        PrimaryActions.Add(new DashboardShortcut(
            "Sales",
            "Create invoices, view history, and monitor revenue.",
            RouteNames.SalesLanding,
            Color.FromArgb("#FF7E5F")));

        PrimaryActions.Add(new DashboardShortcut(
            "Purchases",
            "Track purchase orders and incoming stock.",
            RouteNames.PurchasesLanding,
            Color.FromArgb("#5F72FF")));

        PrimaryActions.Add(new DashboardShortcut(
            "Reports",
            "Analyze sales, inventory, and performance.",
            RouteNames.ReportsLanding,
            Color.FromArgb("#23C9B9")));
    }

    [RelayCommand]
    private Task OpenShortcutAsync(DashboardShortcut? shortcut)
    {
        if (shortcut is null)
        {
            return Task.CompletedTask;
        }

        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.GoToAsync(shortcut.Route).ConfigureAwait(false);
        });
    }

    [RelayCommand]
    private void ToggleProfileMenu()
    {
        IsProfileMenuVisible = !IsProfileMenuVisible;
    }

    [RelayCommand]
    private void HideProfileMenu()
    {
        IsProfileMenuVisible = false;
    }

    [RelayCommand]
    private Task OpenProfileAsync()
    {
        HideProfileMenu();
        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.GoToAsync(RouteNames.ProfileSettings).ConfigureAwait(false);
        });
    }

    [RelayCommand]
    private Task LogoutAsync()
    {
        HideProfileMenu();
        return ExecuteBusyActionAsync(async () =>
        {
            await _tokenStorageService.ClearAsync().ConfigureAwait(false);

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.GoToAsync($"//{RouteNames.Login}").ConfigureAwait(false);
            }).ConfigureAwait(false);
        });
    }
}
