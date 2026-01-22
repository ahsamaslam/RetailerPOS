using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace RetailerMobileApp;

public static class ServiceHelper
{
    public static IServiceProvider Services =>
        Application.Current?.Handler?.MauiContext?.Services
        ?? throw new InvalidOperationException("The application service provider has not been initialized yet.");

    public static T GetRequiredService<T>() where T : notnull =>
        Services.GetRequiredService<T>();
}
