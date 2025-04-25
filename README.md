# Habitable - Cross-Platform Habit Tracking App

Habitable is a modern, cross-platform habit tracking application built with Avalonia UI and .NET. It helps users build and maintain good habits through daily tracking, insightful analytics, and motivational achievements.

## Features

- 📱 Cross-platform support (Windows, macOS, Linux, Android, iOS)
- 🔐 Secure user authentication with email/password and Google OAuth
- 📊 Habit tracking with daily, weekly, and custom frequencies
- 🏆 Achievement system with unlockable badges
- 📈 Analytics dashboard with progress visualization
- 🌐 Offline-first with sync capabilities
- 🎨 Dark/light theme support
- 🔔 Built-in notification system

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) or newer
- [Supabase Account](https://supabase.com) for backend services

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/Dijariii/Habitable.git
   cd Habitable
   ```

2. Configure Supabase:
   - Create a new Supabase project
   - Update the Supabase URL and API key in `App.axaml.cs`

3. Build and run:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project Habitable/Habitable.csproj
   ```

## Project Structure

- `Models/` - Data models for habits, achievements, and user profiles
- `Services/` - Business logic and data access services
- `ViewModels/` - MVVM view models
- `Views/` - Avalonia UI views
- `Assets/` - Application resources

## Tech Stack

- [Avalonia UI](https://avaloniaui.net/) - Cross-platform UI framework
- [.NET 9](https://dotnet.microsoft.com/) - Development platform
- [Supabase](https://supabase.com) - Backend services
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM toolkit

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
