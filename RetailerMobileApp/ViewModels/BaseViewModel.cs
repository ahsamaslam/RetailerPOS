using CommunityToolkit.Mvvm.ComponentModel;

namespace RetailerMobileApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private bool _isInitialized;

    protected async Task ExecuteBusyActionAsync(Func<Task> action)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await action().ConfigureAwait(false);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
