using Microsoft.Maui.Controls;
using RetailerMobileApp.Features.Profile.ViewModels;

namespace RetailerMobileApp.Features.Profile.Views;

public partial class ProfileSettingsPage : ContentPage
{
    public ProfileSettingsPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetRequiredService<ProfileSettingsViewModel>();
    }
}
