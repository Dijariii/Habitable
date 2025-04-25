using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Supabase.Postgrest.Models;

namespace Habitable.Models;

public partial class UserProfile : BaseModel
{
    [ObservableProperty]
    private string id = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string displayName = string.Empty;

    [ObservableProperty]
    private string avatar = string.Empty;

    [ObservableProperty]
    private bool isDarkMode;

    [ObservableProperty]
    private int streakFreezeTokens;

    [ObservableProperty]
    private DateTime lastLoginAt = DateTime.UtcNow;

    [ObservableProperty]
    private List<Achievement> achievements = new();

    public UserProfile()
    {
        achievements = new List<Achievement>();
    }
}