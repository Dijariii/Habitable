using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using Habitable.Models;

namespace Habitable.Services;

public class HabitService : IHabitService
{
    private readonly Client _supabaseClient;
    private readonly ISyncService _syncService;

    public HabitService(Client supabaseClient, ISyncService syncService)
    {
        _supabaseClient = supabaseClient;
        _syncService = syncService;
    }

    public async Task<List<Habit>> GetHabitsAsync()
    {
        try
        {
            if (await _syncService.IsOnlineAsync())
            {
                var response = await _supabaseClient.From<Habit>().Get();
                return response.Models;
            }
            return new List<Habit>();
        }
        catch (Exception)
        {
            return new List<Habit>();
        }
    }

    public async Task<Habit> CreateHabitAsync(Habit habit)
    {
        try
        {
            if (await _syncService.IsOnlineAsync())
            {
                var response = await _supabaseClient.From<Habit>().Insert(habit);
                return response.Models.FirstOrDefault() ?? habit;
            }
            
            await _syncService.QueueOfflineChangesAsync(new OfflineChange(
                "Habit",
                habit.Id,
                ChangeType.Create,
                System.Text.Json.JsonSerializer.Serialize(habit)
            ));
            return habit;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Habit> UpdateHabitAsync(Habit habit)
    {
        try
        {
            if (await _syncService.IsOnlineAsync())
            {
                var response = await _supabaseClient.From<Habit>()
                    .Where(h => h.Id == habit.Id)
                    .Update(habit);
                return response.Models.FirstOrDefault() ?? habit;
            }

            await _syncService.QueueOfflineChangesAsync(new OfflineChange(
                "Habit",
                habit.Id,
                ChangeType.Update,
                System.Text.Json.JsonSerializer.Serialize(habit)
            ));
            return habit;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task DeleteHabitAsync(string habitId)
    {
        try
        {
            if (await _syncService.IsOnlineAsync())
            {
                await _supabaseClient.From<Habit>()
                    .Where(h => h.Id == habitId)
                    .Delete();
                return;
            }

            await _syncService.QueueOfflineChangesAsync(new OfflineChange(
                "Habit",
                habitId,
                ChangeType.Delete,
                string.Empty
            ));
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> CheckInHabitAsync(string habitId, DateTime date)
    {
        try
        {
            var habits = await _supabaseClient.From<Habit>()
                .Where(h => h.Id == habitId)
                .Get();
            
            var habit = habits.Models.FirstOrDefault();
            if (habit == null) return false;

            habit.CurrentStreak++;
            if (habit.CurrentStreak > habit.LongestStreak)
            {
                habit.LongestStreak = habit.CurrentStreak;
            }

            await UpdateHabitAsync(habit);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<DateTime>> GetCheckInsForHabitAsync(string habitId, DateTime startDate, DateTime endDate)
    {
        // TODO: Implement check-in history
        return new List<DateTime>();
    }

    public async Task<int> GetCurrentStreakAsync(string habitId)
    {
        try
        {
            var habits = await _supabaseClient.From<Habit>()
                .Where(h => h.Id == habitId)
                .Get();
            return habits.Models.FirstOrDefault()?.CurrentStreak ?? 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<Dictionary<string, int>> GetAllStreaksAsync()
    {
        try
        {
            var habits = await GetHabitsAsync();
            return habits.ToDictionary(h => h.Id, h => h.CurrentStreak);
        }
        catch (Exception)
        {
            return new Dictionary<string, int>();
        }
    }
}