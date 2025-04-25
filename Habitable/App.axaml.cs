using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Habitable.Services;
using Habitable.ViewModels;
using Habitable.ViewModels.Features;
using Habitable.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Supabase;

namespace Habitable;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Remove data validation plugins to avoid conflicts
        if (BindingPlugins.DataValidators.Any(x => x is DataAnnotationsValidationPlugin))
        {
            var pluginsToRemove = BindingPlugins.DataValidators
                .OfType<DataAnnotationsValidationPlugin>()
                .ToList();
            foreach (var plugin in pluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }

        ConfigureServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var authService = _serviceProvider!.GetRequiredService<IAuthService>();
            var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configuration - Using demo project for development
        const string supabaseUrl = "https://xyzcompany.supabase.co";  // Replace with your Supabase URL
        const string supabaseKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9..."; // Replace with your Supabase anon key

        // Create Supabase client
        var supabaseOptions = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };
        var supabaseClient = new Client(supabaseUrl, supabaseKey, supabaseOptions);

        // Register services
        services.AddSingleton(supabaseClient);
        services.AddSingleton<ISyncService, SyncService>();
        services.AddSingleton<IAuthService>(provider => new AuthService(supabaseUrl, supabaseKey));
        services.AddSingleton<IHabitService>(provider => 
            new HabitService(supabaseClient, provider.GetRequiredService<ISyncService>()));
        services.AddSingleton<IAchievementService>(provider => 
            new AchievementService(supabaseClient, provider.GetRequiredService<ISyncService>()));

        // Register ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<HabitsViewModel>();
        services.AddTransient<AchievementsViewModel>();
        services.AddTransient<SettingsViewModel>();

        _serviceProvider = services.BuildServiceProvider();
    }
}