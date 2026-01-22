using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using RetailerMobileApp.Core.Constants;
using RetailerMobileApp.Core.Interfaces;
using RetailerMobileApp.Core.Models.Auth;
using RetailerMobileApp.ViewModels;

namespace RetailerMobileApp.Features.Authentication.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    private readonly IAuthApiClient _authApiClient;
    private readonly ITokenStorageService _tokenStorageService;

    public LoginViewModel(IAuthApiClient authApiClient, ITokenStorageService tokenStorageService)
    {
        _authApiClient = authApiClient;
        _tokenStorageService = tokenStorageService;
        Title = "Sign in";
    }

    [RelayCommand]
    private async Task SignInAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await ShowAlertAsync("Missing information", "Username and password are required.").ConfigureAwait(false);
            return;
        }

        await ExecuteBusyActionAsync(async () =>
        {
            try
            {
                var request = new LoginRequestDto(Username.Trim(), Password);
                var result = await _authApiClient.LoginAsync(request).ConfigureAwait(false);
                await _tokenStorageService.StoreTokensAsync(result).ConfigureAwait(false);

                Password = string.Empty;

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.GoToAsync($"//{RouteNames.Dashboard}").ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Sign in failed", ex.Message).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    private static Task ShowAlertAsync(string title, string message)
    {
        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Shell.Current is null)
            {
                return;
            }

            await Shell.Current.DisplayAlertAsync(title, message, "OK").ConfigureAwait(false);
        });
    }
}
