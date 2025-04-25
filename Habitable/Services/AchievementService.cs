using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using Habitable.Models;

namespace Habitable.Services;

public class AchievementService : IAchievementService
{
    private readonly Client _supabaseClient;
    private readonly ISyncService _syncService;

    public AchievementService(Client supabaseClient, ISyncService syncService)
    {
        _supabaseClient = supabaseClient;
        _syncService = syncService;
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        try
        {
            if (await _syncService.IsOnlineAsync())
            {
                var response = await _supabaseClient.From<Achievement>().Get();
                return response.Models;
            }
            return new List<Achievement>();
        }
        catch (Exception)
        {
            return new List<Achievement>();
        }
    }

    public async Task<List<Achievement>> GetUnlockedAchievementsAsync()
    {
        try
        {
            if (await _syncService.IsOnlineAsync())
            {
                var response = await _supabaseClient.From<Achievement>()
                    .Where(a => a.UnlockedAt != null)
                    .Get();
                return response.Models;
            }
            return new List<Achievement>();
        }
        catch (Exception)
        {
            return new List<Achievement>();
        }
    }

    public async Task<Achievement> UnlockAchievementAsync(string achievementId)
    {
        try
        {
            var achievements = await _supabaseClient.From<Achievement>()
                .Where(a => a.Id == achievementId)
                .Get();

            var achievement = achievements.Models.FirstOrDefault();
            if (achievement != null)
            {
                achievement.UnlockedAt = DateTime.UtcNow;
                
                if (await _syncService.IsOnlineAsync())
                {
                    var response = await _supabaseClient.From<Achievement>()
                        .Where(a => a.Id == achievementId)
                        .Update(achievement);
                    return response.Models.FirstOrDefault() ?? achievement;
                }

                await _syncService.QueueOfflineChangesAsync(new OfflineChange(
                    "Achievement",
                    achievementId,
                    ChangeType.Update,
                    System.Text.Json.JsonSerializer.Serialize(achievement)
                ));
            }
            return achievement ?? new Achievement { Id = achievementId };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> GetProgressForAchievementAsync(string achievementId)
    {
        try
        {
            var achievements = await _supabaseClient.From<Achievement>()
                .Where(a => a.Id == achievementId)
                .Get();
            return achievements.Models.FirstOrDefault()?.Progress ?? 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task UpdateAchievementProgressAsync(string habitId)
    {
        try
        {
            var habits = await _supabaseClient.From<Habit>()
                .Where(h => h.Id == habitId)
                .Get();
            var habit = habits.Models.FirstOrDefault();
            if (habit == null) return;

            var achievements = await GetAllAchievementsAsync();
            foreach (var achievement in achievements)
            {
                if (achievement.Type == Achievement.AchievementType.Streak)
                {
                    achievement.Progress = habit.CurrentStreak;
                    if (achievement.Progress >= achievement.Threshold && achievement.UnlockedAt == null)
                    {
                        await UnlockAchievementAsync(achievement.Id);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Log error
        }
    }

    public async Task<List<Achievement>> GetAvailableAchievementsAsync()
    {
        try
        {
            if (await _syncService.IsOnlineAsync())
            {
                var response = await _supabaseClient.From<Achievement>()
                    .Where(a => a.UnlockedAt == null)
                    .Get();
                return response.Models;
            }
            return new List<Achievement>();
        }
        catch (Exception)
        {
            return new List<Achievement>();
        }
    }

    public async Task<bool> CheckAndUnlockAchievementsAsync(string habitId)
    {
        try
        {
            await UpdateAchievementProgressAsync(habitId);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}