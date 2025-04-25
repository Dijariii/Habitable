using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Habitable.Services;
using Habitable.ViewModels;
using Habitable.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

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
        // Remove any DataValidation plugins
        ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);

        ConfigureServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var authService = _serviceProvider!.GetRequiredService<IAuthService>();
            var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = authService.IsAuthenticated().Result
                    ? _serviceProvider.GetRequiredService<MainWindowViewModel>()
                    : loginViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configuration
        const string supabaseUrl = "YOUR_SUPABASE_URL"; // TODO: Move to configuration
        const string supabaseKey = "YOUR_SUPABASE_KEY"; // TODO: Move to configuration

        // Register services
        services.AddSingleton<IAuthService>(new AuthService(supabaseUrl, supabaseKey));
        services.AddSingleton<ISyncService, SyncService>();
        services.AddSingleton<IHabitService, HabitService>();
        services.AddSingleton<IAchievementService, AchievementService>();

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