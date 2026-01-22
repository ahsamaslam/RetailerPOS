using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RetailerMobileApp.Core.Interfaces;
using RetailerMobileApp.Core.Options;
using RetailerMobileApp.Features.Authentication.ViewModels;
using RetailerMobileApp.Features.Dashboard.ViewModels;
using RetailerMobileApp.Features.Profile.ViewModels;
using RetailerMobileApp.Features.Purchases.ViewModels;
using RetailerMobileApp.Features.Reports.ViewModels;
using RetailerMobileApp.Features.Sales.ViewModels;
using RetailerMobileApp.Infrastructure.Http;
using RetailerMobileApp.Infrastructure.Storage;

namespace RetailerMobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var apiEndpoints = new ApiEndpointsOptions
            {
                AuthModuleBaseUrl = builder.Configuration[$"{ApiEndpointsOptions.SectionName}:{nameof(ApiEndpointsOptions.AuthModuleBaseUrl)}"] ?? string.Empty,
                RetailerApiBaseUrl = builder.Configuration[$"{ApiEndpointsOptions.SectionName}:{nameof(ApiEndpointsOptions.RetailerApiBaseUrl)}"] ?? string.Empty
            };

            apiEndpoints.Validate();
            builder.Services.AddSingleton(apiEndpoints);

            builder.Services.AddSingleton<ITokenStorageService, SecureTokenStorageService>();
            builder.Services.AddTransient<AuthenticatedHttpMessageHandler>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<SalesLandingViewModel>();
            builder.Services.AddTransient<SalesCreateViewModel>();
            builder.Services.AddTransient<PurchasesLandingViewModel>();
            builder.Services.AddTransient<ReportsLandingViewModel>();
            builder.Services.AddTransient<ProfileSettingsViewModel>();

            builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
            {
                client.BaseAddress = apiEndpoints.AuthModuleBaseAddress;
            });

            builder.Services.AddHttpClient<ISalesApiClient, SalesApiClient>(client =>
            {
                client.BaseAddress = apiEndpoints.RetailerApiBaseAddress;
            }).AddHttpMessageHandler<AuthenticatedHttpMessageHandler>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
