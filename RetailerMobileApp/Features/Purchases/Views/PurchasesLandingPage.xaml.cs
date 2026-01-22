using Microsoft.Maui.Controls;
using RetailerMobileApp.Features.Purchases.ViewModels;

namespace RetailerMobileApp.Features.Purchases.Views;

public partial class PurchasesLandingPage : ContentPage
{
    public PurchasesLandingPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetRequiredService<PurchasesLandingViewModel>();
    }
}
