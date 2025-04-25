using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Habitable.Models;
using Habitable.Services;

namespace Habitable.ViewModels.Features;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IHabitService _habitService;
    private readonly IAchievementService _achievementService;

    [ObservableProperty]
    private ObservableCollection<Habit> todaysHabits = new();

    [ObservableProperty]
    private int completedToday;

    [ObservableProperty]
    private int totalHabits;

    [ObservableProperty]
    private int currentStreak;

    [ObservableProperty]
    private ObservableCollection<Achievement> recentAchievements = new();

    public DashboardViewModel(IHabitService habitService, IAchievementService achievementService)
    {
        _habitService = habitService;
        _achievementService = achievementService;
        LoadDashboardDataAsync().ConfigureAwait(false);
    }

    private async Task LoadDashboardDataAsync()
    {
        var habits = await _habitService.GetHabitsAsync();
        var streaks = await _habitService.GetAllStreaksAsync();
        var achievements = await _achievementService.GetUnlockedAchievementsAsync();

        TodaysHabits.Clear();
        foreach (var habit in habits)
        {
            // TODO: Filter habits for today based on frequency
            TodaysHabits.Add(habit);
        }

        TotalHabits = habits.Count;
        CurrentStreak = streaks.Count > 0 ? streaks.Values.Max() : 0;

        RecentAchievements.Clear();
        foreach (var achievement in achievements.Take(5))
        {
            RecentAchievements.Add(achievement);
        }
    }
}