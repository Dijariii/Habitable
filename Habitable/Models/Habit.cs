using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Habitable.Models;

public partial class Habit : ObservableObject
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
    private string color;

    [ObservableProperty]
    private FrequencyType frequency;

    [ObservableProperty]
    private List<DayOfWeek> customDays;

    [ObservableProperty]
    private TimeSpan? reminderTime;

    [ObservableProperty]
    private DateTime createdAt;

    [ObservableProperty]
    private int currentStreak;

    [ObservableProperty]
    private int longestStreak;

    public enum FrequencyType
    {
        Daily,
        Weekly,
        Custom
    }
}