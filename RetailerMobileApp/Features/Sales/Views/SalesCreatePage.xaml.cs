using Microsoft.Maui.Controls;
using RetailerMobileApp.Features.Sales.ViewModels;

namespace RetailerMobileApp.Features.Sales.Views;

public partial class SalesCreatePage : ContentPage
{
    public SalesCreatePage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetRequiredService<SalesCreateViewModel>();
    }
}
