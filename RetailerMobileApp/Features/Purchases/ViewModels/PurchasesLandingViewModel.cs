using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailerMobileApp.ViewModels;

namespace RetailerMobileApp.Features.Purchases.ViewModels;

public partial class PurchasesLandingViewModel : BaseViewModel
{
    public ObservableCollection<string> PendingPurchases { get; } = new();

    public PurchasesLandingViewModel()
    {
        Title = "Purchases";
        LoadPlaceholders();
    }

    private void LoadPlaceholders()
    {
        PendingPurchases.Clear();
        PendingPurchases.Add("PO-5001 · Draft");
        PendingPurchases.Add("PO-5002 · Awaiting GRN");
    }

    [RelayCommand]
    private Task CreatePurchaseAsync() => Task.CompletedTask;
}
