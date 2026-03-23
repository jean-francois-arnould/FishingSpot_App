using FishingSpot.PWA.Models.Auth;

namespace FishingSpot.PWA.Services
{
    public interface IAuthService
    {
        event Action<User?>? OnAuthStateChanged;

        User? CurrentUser { get; }
        string? AccessToken { get; }
        bool IsAuthenticated { get; }

        Task<(bool Success, string Message)> SignUpAsync(string email, string password);
        Task<(bool Success, string Message)> SignInAsync(string email, string password);
        Task SignOutAsync();
        Task InitializeAsync();
    }
}
