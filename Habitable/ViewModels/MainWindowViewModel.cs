using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Habitable.Services;

namespace Habitable.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    
    [ObservableProperty]
    private ViewModelBase? currentView;

    [ObservableProperty]
    private string selectedNavItem = "Dashboard";

    public MainWindowViewModel(IAuthService authService)
    {
        _authService = authService;
        UpdateCurrentView();
    }

    partial void OnSelectedNavItemChanged(string value)
    {
        UpdateCurrentView();
    }

    private void UpdateCurrentView()
    {
        CurrentView = SelectedNavItem switch
        {
            "Dashboard" => new DashboardViewModel(),
            "Habits" => new HabitsViewModel(),
            "Achievements" => new AchievementsViewModel(),
            "Settings" => new SettingsViewModel(),
            _ => throw new ArgumentException($"Unknown nav item: {SelectedNavItem}")
        };
    }

    [RelayCommand]
    private async Task SignOut()
    {
        await _authService.SignOutAsync();
        // TODO: Navigate to login view
    }
}
