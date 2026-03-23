using FishingSpot.PWA.Models;

namespace FishingSpot.PWA.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile?> GetProfileAsync();
        Task<bool> CreateOrUpdateProfileAsync(UserProfile profile);
        Task<bool> DeleteProfileAsync();
        Task<bool> DeleteAccountAsync();
    }
}
