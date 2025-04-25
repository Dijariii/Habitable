using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Habitable.Models;
using Habitable.Services;

namespace Habitable.ViewModels.Features;

public partial class AchievementsViewModel : ViewModelBase
{
    private readonly IAchievementService _achievementService;

    [ObservableProperty]
    private ObservableCollection<Achievement> unlockedAchievements = new();

    [ObservableProperty]
    private ObservableCollection<Achievement> availableAchievements = new();

    [ObservableProperty]
    private Achievement? selectedAchievement;

    [ObservableProperty]
    private bool isLoading;

    public AchievementsViewModel(IAchievementService achievementService)
    {
        _achievementService = achievementService;
        LoadAchievementsAsync().ConfigureAwait(false);
    }

    private async Task LoadAchievementsAsync()
    {
        IsLoading = true;

        var unlocked = await _achievementService.GetUnlockedAchievementsAsync();
        UnlockedAchievements.Clear();
        foreach (var achievement in unlocked)
        {
            UnlockedAchievements.Add(achievement);
        }

        var available = await _achievementService.GetAvailableAchievementsAsync();
        AvailableAchievements.Clear();
        foreach (var achievement in available)
        {
            AvailableAchievements.Add(achievement);
        }

        IsLoading = false;
    }
}