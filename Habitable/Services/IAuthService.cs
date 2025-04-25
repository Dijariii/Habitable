using System.Threading.Tasks;
using Habitable.Models;

namespace Habitable.Services;

public interface IAuthService
{
    Task<bool> SignInAsync(string email, string password);
    Task<bool> SignInWithGoogleAsync();
    Task SignOutAsync();
    Task<bool> SignUpAsync(string email, string password, string displayName);
    Task<UserProfile?> GetCurrentUserAsync();
    Task<bool> UpdateUserProfileAsync(UserProfile profile);
    Task<bool> IsAuthenticated();
    Task<string?> GetCurrentSessionTokenAsync();
}