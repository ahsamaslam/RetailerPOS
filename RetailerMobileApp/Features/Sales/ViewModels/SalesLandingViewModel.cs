using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailerMobileApp.Core.Constants;
using RetailerMobileApp.ViewModels;

namespace RetailerMobileApp.Features.Sales.ViewModels;

public partial class SalesLandingViewModel : BaseViewModel
{
    public ObservableCollection<string> RecentSales { get; } = new();

    public SalesLandingViewModel()
    {
        Title = "Sales";
        LoadPlaceholders();
    }

    private void LoadPlaceholders()
    {
        RecentSales.Clear();
        RecentSales.Add("INV-1001 · $540");
        RecentSales.Add("INV-1002 · $1,230");
        RecentSales.Add("INV-1003 · $320");
    }

    [RelayCommand]
    private Task StartNewSaleAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.GoToAsync(RouteNames.SalesCreate).ConfigureAwait(false);
        });
    }
}
