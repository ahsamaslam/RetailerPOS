using RetailerMobileApp.Core.Constants;
using RetailerMobileApp.Features.Profile.Views;
using RetailerMobileApp.Features.Purchases.Views;
using RetailerMobileApp.Features.Reports.Views;
using RetailerMobileApp.Features.Sales.Views;

namespace RetailerMobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private static void RegisterRoutes()
        {
            Routing.RegisterRoute(RouteNames.SalesLanding, typeof(SalesLandingPage));
            Routing.RegisterRoute(RouteNames.SalesCreate, typeof(SalesCreatePage));
            Routing.RegisterRoute(RouteNames.PurchasesLanding, typeof(PurchasesLandingPage));
            Routing.RegisterRoute(RouteNames.ReportsLanding, typeof(ReportsLandingPage));
            Routing.RegisterRoute(RouteNames.ProfileSettings, typeof(ProfileSettingsPage));
        }
    }
}
