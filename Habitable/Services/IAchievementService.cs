using System.Collections.Generic;
using System.Threading.Tasks;
using Habitable.Models;

namespace Habitable.Services;

public interface IAchievementService
{
    Task<List<Achievement>> GetAllAchievementsAsync();
    Task<List<Achievement>> GetUnlockedAchievementsAsync();
    Task<Achievement> UnlockAchievementAsync(string achievementId);
    Task<int> GetProgressForAchievementAsync(string achievementId);
    Task UpdateAchievementProgressAsync(string habitId);
    Task<List<Achievement>> GetAvailableAchievementsAsync();
    Task<bool> CheckAndUnlockAchievementsAsync(string habitId);
}