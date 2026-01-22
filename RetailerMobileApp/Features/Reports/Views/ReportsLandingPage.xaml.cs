using Microsoft.Maui.Controls;
using RetailerMobileApp.Features.Reports.ViewModels;

namespace RetailerMobileApp.Features.Reports.Views;

public partial class ReportsLandingPage : ContentPage
{
    public ReportsLandingPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetRequiredService<ReportsLandingViewModel>();
    }
}
