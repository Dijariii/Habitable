using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Habitable.Services;

namespace Habitable.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string displayName = string.Empty;

    [ObservableProperty]
    private bool isSigningUp;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public string ActionButtonText => IsSigningUp ? "Sign Up" : "Sign In";
    public string ToggleActionText => IsSigningUp ? "Already have an account?" : "Don't have an account?";
    public string ToggleLinkText => IsSigningUp ? "Sign In" : "Sign Up";

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task Submit()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please fill in all required fields";
            return;
        }

        if (IsSigningUp && string.IsNullOrWhiteSpace(DisplayName))
        {
            ErrorMessage = "Display name is required";
            return;
        }

        bool success = IsSigningUp 
            ? await _authService.SignUpAsync(Email, Password, DisplayName)
            : await _authService.SignInAsync(Email, Password);

        if (!success)
        {
            ErrorMessage = IsSigningUp 
                ? "Failed to create account. Please try again." 
                : "Invalid email or password";
            return;
        }

        // TODO: Navigate to main window
    }

    [RelayCommand]
    private async Task GoogleSignIn()
    {
        ErrorMessage = string.Empty;
        var success = await _authService.SignInWithGoogleAsync();
        if (!success)
        {
            ErrorMessage = "Failed to sign in with Google";
            return;
        }

        // TODO: Navigate to main window
    }

    public void ToggleAuthMode()
    {
        IsSigningUp = !IsSigningUp;
        ErrorMessage = string.Empty;
    }
}