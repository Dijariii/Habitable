using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Habitable.Services;
using Habitable.ViewModels.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Habitable.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _serviceProvider;
    
    [ObservableProperty]
    private ViewModelBase? currentView;

    [ObservableProperty]
    private string selectedNavItem = "Dashboard";

    public MainWindowViewModel(IAuthService authService, IServiceProvider serviceProvider)
    {
        _authService = authService;
        _serviceProvider = serviceProvider;
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
            "Dashboard" => _serviceProvider.GetRequiredService<DashboardViewModel>(),
            "Habits" => _serviceProvider.GetRequiredService<HabitsViewModel>(),
            "Achievements" => _serviceProvider.GetRequiredService<AchievementsViewModel>(),
            "Settings" => _serviceProvider.GetRequiredService<SettingsViewModel>(),
            _ => throw new ArgumentException($"Unknown nav item: {SelectedNavItem}")
        };
    }

    [RelayCommand]
    private async Task SignOut()
    {
        await _authService.SignOutAsync();
        // Navigate to login view will be handled by AuthService state change
    }
}
