using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Supabase.Postgrest.Models;

namespace Habitable.Models;

[ObservableObject]
public partial class UserProfile : BaseModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public bool IsDarkMode { get; set; }
    public int StreakFreezeTokens { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
    public List<Achievement> Achievements { get; set; } = new();
}