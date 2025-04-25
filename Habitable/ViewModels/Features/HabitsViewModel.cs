using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Habitable.Models;
using Habitable.Services;

namespace Habitable.ViewModels.Features;

public partial class HabitsViewModel : ViewModelBase
{
    private readonly IHabitService _habitService;
    private readonly IAchievementService _achievementService;

    [ObservableProperty]
    private ObservableCollection<Habit> habits = new();

    [ObservableProperty]
    private Habit? selectedHabit;

    [ObservableProperty]
    private bool isAddingHabit;

    [ObservableProperty]
    private string newHabitName = string.Empty;

    [ObservableProperty]
    private string newHabitDescription = string.Empty;

    public HabitsViewModel(IHabitService habitService, IAchievementService achievementService)
    {
        _habitService = habitService;
        _achievementService = achievementService;
        LoadHabitsAsync().ConfigureAwait(false);
    }

    private async Task LoadHabitsAsync()
    {
        var habitsList = await _habitService.GetHabitsAsync();
        Habits.Clear();
        foreach (var habit in habitsList)
        {
            Habits.Add(habit);
        }
    }

    [RelayCommand]
    private void StartAddHabit()
    {
        IsAddingHabit = true;
        NewHabitName = string.Empty;
        NewHabitDescription = string.Empty;
    }

    [RelayCommand]
    private async Task SaveNewHabit()
    {
        if (string.IsNullOrWhiteSpace(NewHabitName)) return;

        var habit = new Habit
        {
            Name = NewHabitName,
            Description = NewHabitDescription,
            CreatedAt = DateTime.UtcNow
        };

        await _habitService.CreateHabitAsync(habit);
        await LoadHabitsAsync();
        IsAddingHabit = false;
    }

    [RelayCommand]
    private async Task CheckInHabit(string habitId)
    {
        await _habitService.CheckInHabitAsync(habitId, DateTime.UtcNow);
        await _achievementService.UpdateAchievementProgressAsync(habitId);
        await LoadHabitsAsync();
    }

    [RelayCommand]
    private async Task DeleteHabit(string habitId)
    {
        await _habitService.DeleteHabitAsync(habitId);
        await LoadHabitsAsync();
    }
}