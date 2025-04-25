using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Supabase.Postgrest.Models;

namespace Habitable.Models;

[ObservableObject]
public partial class Habit : BaseModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public FrequencyType Frequency { get; set; }
    public List<DayOfWeek> CustomDays { get; set; } = new();
    public TimeSpan? ReminderTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public bool IsCompleted { get; set; }

    public enum FrequencyType
    {
        Daily,
        Weekly,
        Custom
    }
}