using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Habitable.Models;

namespace Habitable.Services;

public interface IHabitService
{
    Task<List<Habit>> GetHabitsAsync();
    Task<Habit> CreateHabitAsync(Habit habit);
    Task<Habit> UpdateHabitAsync(Habit habit);
    Task DeleteHabitAsync(string habitId);
    Task<bool> CheckInHabitAsync(string habitId, DateTime date);
    Task<List<DateTime>> GetCheckInsForHabitAsync(string habitId, DateTime startDate, DateTime endDate);
    Task<int> GetCurrentStreakAsync(string habitId);
    Task<Dictionary<string, int>> GetAllStreaksAsync();
}