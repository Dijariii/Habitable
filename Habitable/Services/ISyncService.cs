using System.Threading.Tasks;
using Habitable.Models;

namespace Habitable.Services;

public interface ISyncService
{
    Task InitializeLocalStorageAsync();
    Task<bool> IsSyncRequiredAsync();
    Task SyncHabitsAsync();
    Task SyncUserProfileAsync();
    Task SyncAchievementsAsync();
    Task<bool> IsOnlineAsync();
    Task QueueOfflineChangesAsync(OfflineChange change);
    Task ProcessOfflineQueueAsync();
}

public record OfflineChange(
    string EntityType,
    string EntityId,
    ChangeType Type,
    string SerializedData
);

public enum ChangeType
{
    Create,
    Update,
    Delete,
    CheckIn
}