using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Supabase.Postgrest.Models;

namespace Habitable.Models;

[ObservableObject]
public partial class Achievement : BaseModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public AchievementType Type { get; set; }
    public int Threshold { get; set; }
    public DateTime? UnlockedAt { get; set; }
    public int Progress { get; set; }

    public enum AchievementType
    {
        Streak,
        TotalCheckIns,
        PerfectWeek,
        HabitMaster
    }
}