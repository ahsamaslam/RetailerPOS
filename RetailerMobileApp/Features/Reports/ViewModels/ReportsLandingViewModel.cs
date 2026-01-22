using System.Collections.ObjectModel;
using RetailerMobileApp.ViewModels;

namespace RetailerMobileApp.Features.Reports.ViewModels;

public partial class ReportsLandingViewModel : BaseViewModel
{
    public ObservableCollection<string> Reports { get; } = new();

    public ReportsLandingViewModel()
    {
        Title = "Reports";
        SeedReports();
    }

    private void SeedReports()
    {
        Reports.Clear();
        Reports.Add("Daily Sales Report");
        Reports.Add("Monthly Sales Summary");
        Reports.Add("Stock Position Report");
    }
}
