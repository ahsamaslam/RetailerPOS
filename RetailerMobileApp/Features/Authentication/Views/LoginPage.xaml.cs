using RetailerMobileApp.Features.Authentication.ViewModels;

namespace RetailerMobileApp.Features.Authentication.Views;

public class LoginPage : ContentPage
{
    public LoginPage()
    {
        BindingContext = ServiceHelper.GetRequiredService<LoginViewModel>();
        this.SetBinding(TitleProperty, nameof(LoginViewModel.Title));
        Content = BuildContent();
    }

    private View BuildContent()
    {
        var titleLabel = new Label
        {
            Text = "Retailer POS",
            FontSize = 28,
            HorizontalTextAlignment = TextAlignment.Center
        };
        SemanticProperties.SetHeadingLevel(titleLabel, SemanticHeadingLevel.Level1);

        var subtitleLabel = new Label
        {
            Text = "Sign in to continue",
            FontSize = 18,
            TextColor = Colors.Gray,
            HorizontalTextAlignment = TextAlignment.Center
        };

        var usernameEntry = new Entry
        {
            Placeholder = "Username",
            Keyboard = Keyboard.Email,
            ClearButtonVisibility = ClearButtonVisibility.WhileEditing
        };
        usernameEntry.SetBinding(Entry.TextProperty, nameof(LoginViewModel.Username));

        var passwordEntry = new Entry
        {
            Placeholder = "Password",
            IsPassword = true,
            ClearButtonVisibility = ClearButtonVisibility.WhileEditing
        };
        passwordEntry.SetBinding(Entry.TextProperty, nameof(LoginViewModel.Password));

        var signInButton = new Button
        {
            Text = "Sign in",
            HeightRequest = 48,
            HorizontalOptions = LayoutOptions.Fill
        };
        signInButton.SetBinding(Button.CommandProperty, nameof(LoginViewModel.SignInCommand));

        var activityIndicator = new ActivityIndicator
        {
            HorizontalOptions = LayoutOptions.Center
        };
        activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(LoginViewModel.IsBusy));
        activityIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, nameof(LoginViewModel.IsBusy));

        var stack = new VerticalStackLayout
        {
            Padding = new Thickness(32, 48),
            Spacing = 20,
            Children =
            {
                titleLabel,
                subtitleLabel,
                usernameEntry,
                passwordEntry,
                signInButton,
                activityIndicator
            }
        };

        return new ScrollView
        {
            Content = stack
        };
    }
}
