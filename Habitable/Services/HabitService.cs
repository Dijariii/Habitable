using System;
using System.Collections.Generic;
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
                return await _supabaseClient.From<Habit>().Get();
            }
            return new List<Habit>(); // TODO: Implement local storage retrieval
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
                return await _supabaseClient.From<Habit>().Insert(habit).Single();
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
                return await _supabaseClient.From<Habit>()
                    .Update(habit)
                    .Match(new { Id = habit.Id })
                    .Single();
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
                    .Delete()
                    .Match(new { Id = habitId });
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
            // For demo purposes, just update the streak
            var habit = await _supabaseClient.From<Habit>()
                .Where(h => h.Id == habitId)
                .Single();

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
            var habit = await _supabaseClient.From<Habit>()
                .Where(h => h.Id == habitId)
                .Single();
            return habit.CurrentStreak;
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
            var streaks = new Dictionary<string, int>();
            foreach (var habit in habits)
            {
                streaks[habit.Id] = habit.CurrentStreak;
            }
            return streaks;
        }
        catch (Exception)
        {
            return new Dictionary<string, int>();
        }
    }
}