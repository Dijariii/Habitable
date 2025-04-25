using System;
using System.Collections.Generic;
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
                return await _supabaseClient.From<Achievement>().Get();
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
                return await _supabaseClient.From<Achievement>()
                    .Where(a => a.UnlockedAt != null)
                    .Get();
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
            var achievement = await _supabaseClient.From<Achievement>()
                .Where(a => a.Id == achievementId)
                .Single();

            achievement.UnlockedAt = DateTime.UtcNow;
            
            if (await _syncService.IsOnlineAsync())
            {
                return await _supabaseClient.From<Achievement>()
                    .Update(achievement)
                    .Match(new { Id = achievementId })
                    .Single();
            }

            await _syncService.QueueOfflineChangesAsync(new OfflineChange(
                "Achievement",
                achievementId,
                ChangeType.Update,
                System.Text.Json.JsonSerializer.Serialize(achievement)
            ));
            return achievement;
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
            var achievement = await _supabaseClient.From<Achievement>()
                .Where(a => a.Id == achievementId)
                .Single();
            return achievement.Progress;
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
            // Demo implementation - just check streak-based achievements
            var habit = await _supabaseClient.From<Habit>()
                .Where(h => h.Id == habitId)
                .Single();

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
                return await _supabaseClient.From<Achievement>()
                    .Where(a => a.UnlockedAt == null)
                    .Get();
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