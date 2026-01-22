using Microsoft.Maui.Controls;
using RetailerMobileApp.Features.Sales.ViewModels;

namespace RetailerMobileApp.Features.Sales.Views;

public partial class SalesLandingPage : ContentPage
{
    public SalesLandingPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetRequiredService<SalesLandingViewModel>();
    }
}
