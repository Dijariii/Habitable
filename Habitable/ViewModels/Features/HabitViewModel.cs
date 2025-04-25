using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Habitable.Models;
using Habitable.Services;

namespace Habitable.ViewModels.Features;

public partial class HabitViewModel : ViewModelBase
{
    private readonly Habit _habit;
    private readonly IHabitService _habitService;
    private readonly IAchievementService _achievementService;

    [ObservableProperty]
    private bool isCompleted;

    public string Name => _habit.Name;
    public string Description => _habit.Description;
    public string Icon => _habit.Icon;
    public string Id => _habit.Id;

    public HabitViewModel(Habit habit, IHabitService habitService, IAchievementService achievementService)
    {
        _habit = habit;
        _habitService = habitService;
        _achievementService = achievementService;
        IsCompleted = habit.IsCompleted;
    }

    [RelayCommand]
    private async Task ToggleComplete()
    {
        IsCompleted = !IsCompleted;
        _habit.IsCompleted = IsCompleted;
        
        await _habitService.UpdateHabitAsync(_habit);
        
        if (IsCompleted)
        {
            await _habitService.CheckInHabitAsync(_habit.Id, System.DateTime.Now);
            await _achievementService.UpdateAchievementProgressAsync(_habit.Id);
        }
    }
}