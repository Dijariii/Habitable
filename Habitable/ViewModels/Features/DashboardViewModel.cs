using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Habitable.Models;
using Habitable.Services;

namespace Habitable.ViewModels.Features;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IHabitService _habitService;
    private readonly IAchievementService _achievementService;

    [ObservableProperty]
    private ObservableCollection<HabitViewModel> todaysHabits = new();

    [ObservableProperty]
    private int completedToday;

    [ObservableProperty]
    private int totalHabits;

    [ObservableProperty]
    private int currentStreak;

    [ObservableProperty]
    private ObservableCollection<Achievement> recentAchievements = new();

    [ObservableProperty]
    private bool isLoading;

    public DashboardViewModel(IHabitService habitService, IAchievementService achievementService)
    {
        _habitService = habitService;
        _achievementService = achievementService;
        LoadDashboardDataAsync().ConfigureAwait(false);
    }

    private async Task LoadDashboardDataAsync()
    {
        IsLoading = true;

        var habits = await _habitService.GetHabitsAsync();
        var streaks = await _habitService.GetAllStreaksAsync();
        var achievements = await _achievementService.GetUnlockedAchievementsAsync();

        TodaysHabits.Clear();
        foreach (var habit in habits.Where(h => ShouldShowHabitToday(h)))
        {
            var habitVm = new HabitViewModel(habit, _habitService, _achievementService);
            habitVm.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(HabitViewModel.IsCompleted))
                {
                    UpdateStats();
                }
            };
            TodaysHabits.Add(habitVm);
        }

        UpdateStats();
        CurrentStreak = streaks.Count > 0 ? streaks.Values.Max() : 0;

        RecentAchievements.Clear();
        foreach (var achievement in achievements.OrderByDescending(a => a.UnlockedAt).Take(5))
        {
            RecentAchievements.Add(achievement);
        }

        IsLoading = false;
    }

    private void UpdateStats()
    {
        TotalHabits = TodaysHabits.Count;
        CompletedToday = TodaysHabits.Count(h => h.IsCompleted);
    }

    private bool ShouldShowHabitToday(Habit habit)
    {
        var today = DateTime.Now.DayOfWeek;
        return habit.Frequency switch
        {
            Habit.FrequencyType.Daily => true,
            Habit.FrequencyType.Weekly => today == DayOfWeek.Monday,
            Habit.FrequencyType.Custom => habit.CustomDays.Contains(today),
            _ => false
        };
    }
}