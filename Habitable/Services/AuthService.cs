using System;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using Habitable.Models;

namespace Habitable.Services;

public class AuthService : IAuthService
{
    private readonly Client _supabaseClient;
    private UserProfile? _currentUser;

    public AuthService(string supabaseUrl, string supabaseKey)
    {
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };
        
        _supabaseClient = new Client(supabaseUrl, supabaseKey, options);
    }

    public async Task<bool> SignInAsync(string email, string password)
    {
        try
        {
            var response = await _supabaseClient.Auth.SignIn(email, password);
            if (response != null)
            {
                await LoadUserProfile();
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SignInWithGoogleAsync()
    {
        try
        {
            var response = await _supabaseClient.Auth.SignIn(Supabase.Gotrue.Constants.Provider.Google);
            if (response != null)
            {
                await LoadUserProfile();
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task SignOutAsync()
    {
        await _supabaseClient.Auth.SignOut();
        _currentUser = null;
    }

    public async Task<bool> SignUpAsync(string email, string password, string displayName)
    {
        try
        {
            var response = await _supabaseClient.Auth.SignUp(email, password);
            if (response?.User != null)
            {
                var profile = new UserProfile
                {
                    Id = response.User.Id,
                    Email = email,
                    DisplayName = displayName,
                    CreatedAt = DateTime.UtcNow
                };

                var insertResponse = await _supabaseClient.From<UserProfile>().Insert(profile);
                _currentUser = insertResponse.Models[0];
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<UserProfile?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        await LoadUserProfile();
        return _currentUser;
    }

    public async Task<bool> UpdateUserProfileAsync(UserProfile profile)
    {
        try
        {
            var response = await _supabaseClient.From<UserProfile>()
                .Where(p => p.Id == profile.Id)
                .Update(profile);
                
            _currentUser = response.Models[0];
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> IsAuthenticated()
    {
        var session = await _supabaseClient.Auth.RetrieveSessionAsync();
        return session != null;
    }

    public async Task<string?> GetCurrentSessionTokenAsync()
    {
        var session = await _supabaseClient.Auth.RetrieveSessionAsync();
        return session?.AccessToken;
    }

    private async Task LoadUserProfile()
    {
        var session = await _supabaseClient.Auth.RetrieveSessionAsync();
        if (session?.User != null)
        {
            var response = await _supabaseClient.From<UserProfile>()
                .Where(p => p.Id == session.User.Id)
                .Get();
            _currentUser = response.Models.FirstOrDefault();
        }
    }
}