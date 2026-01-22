using Microsoft.Maui.Controls;
using RetailerMobileApp.Features.Dashboard.ViewModels;

namespace RetailerMobileApp.Features.Dashboard.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetRequiredService<DashboardViewModel>();
    }
}
