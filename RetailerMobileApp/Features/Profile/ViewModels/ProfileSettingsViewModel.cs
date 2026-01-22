using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using RetailerMobileApp.ViewModels;

namespace RetailerMobileApp.Features.Profile.ViewModels;

public partial class ProfileSettingsViewModel : BaseViewModel
{
    private byte[]? _profileImageBytes;

    [ObservableProperty]
    private ImageSource? _profileImage = ImageSource.FromFile("dotnet_bot.png");

    [ObservableProperty]
    private string _fullName = "Retail Manager";

    [ObservableProperty]
    private string _email = "manager@retailerpos.com";

    public ProfileSettingsViewModel()
    {
        Title = "Profile settings";
    }

    [RelayCommand]
    private async Task ChangePhotoAsync()
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select a profile photo"
            }).ConfigureAwait(false);

            if (result is null)
            {
                return;
            }

            await using var stream = await result.OpenReadAsync().ConfigureAwait(false);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms).ConfigureAwait(false);
            _profileImageBytes = ms.ToArray();

            ProfileImage = ImageSource.FromStream(() => new MemoryStream(_profileImageBytes));
        }
        catch (FeatureNotSupportedException)
        {
            await ShowAlertAsync("Not supported", "Photo picking is not supported on this device.").ConfigureAwait(false);
        }
        catch (PermissionException)
        {
            await ShowAlertAsync("Permission denied", "Please allow photo permissions to change your profile picture.").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Unable to update photo", ex.Message).ConfigureAwait(false);
        }
    }

    [RelayCommand]
    private Task SaveProfileAsync()
    {
        return ShowAlertAsync("Saved", "Your profile changes were saved.");
    }

    private static Task ShowAlertAsync(string title, string message)
    {
        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Shell.Current is null)
            {
                return;
            }

            await Shell.Current.DisplayAlert(title, message, "OK").ConfigureAwait(false);
        });
    }
}
