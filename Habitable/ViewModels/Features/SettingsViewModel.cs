using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Habitable.Models;
using Habitable.Services;

namespace Habitable.ViewModels.Features;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly ISyncService _syncService;

    [ObservableProperty]
    private UserProfile userProfile;

    [ObservableProperty]
    private bool isDarkMode;

    [ObservableProperty]
    private bool isNotificationsEnabled;

    [ObservableProperty]
    private bool isSyncing;

    public SettingsViewModel(IAuthService authService, ISyncService syncService)
    {
        _authService = authService;
        _syncService = syncService;
        userProfile = new UserProfile();
        LoadSettingsAsync().ConfigureAwait(false);
    }

    private async Task LoadSettingsAsync()
    {
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser != null)
        {
            UserProfile = currentUser;
            IsDarkMode = currentUser.IsDarkMode;
        }
    }

    [RelayCommand]
    private async Task SyncData()
    {
        IsSyncing = true;
        await _syncService.ProcessOfflineQueueAsync();
        await _syncService.SyncHabitsAsync();
        await _syncService.SyncAchievementsAsync();
        await _syncService.SyncUserProfileAsync();
        IsSyncing = false;
    }

    [RelayCommand]
    private async Task UpdateProfile()
    {
        await _authService.UpdateUserProfileAsync(UserProfile);
    }

    [RelayCommand]
    private async Task ToggleTheme()
    {
        UserProfile.IsDarkMode = IsDarkMode;
        await UpdateProfile();
        // TODO: Apply theme change
    }
}