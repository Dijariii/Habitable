using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Habitable.Models;

public partial class Achievement : ObservableObject
{
    [ObservableProperty]
    private string id;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private string icon;

    [ObservableProperty]
    private AchievementType type;

    [ObservableProperty]
    private int threshold;

    [ObservableProperty]
    private DateTime? unlockedAt;

    [ObservableProperty]
    private int progress;

    public enum AchievementType
    {
        Streak,
        TotalCheckIns,
        PerfectWeek,
        HabitMaster
    }
}