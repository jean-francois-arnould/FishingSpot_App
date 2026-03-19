using FishingSpot.PWA.Models.Equipment;

namespace FishingSpot.PWA.Services
{
    public interface IEquipmentService
    {
        // Cannes
        Task<List<Rod>> GetAllRodsAsync();
        Task<Rod?> GetRodByIdAsync(int id);
        Task<int> AddRodAsync(Rod rod);
        Task<bool> UpdateRodAsync(Rod rod);
        Task<bool> DeleteRodAsync(int id);

        // Moulinets
        Task<List<Reel>> GetAllReelsAsync();
        Task<Reel?> GetReelByIdAsync(int id);
        Task<int> AddReelAsync(Reel reel);
        Task<bool> UpdateReelAsync(Reel reel);
        Task<bool> DeleteReelAsync(int id);

        // Fils
        Task<List<Line>> GetAllLinesAsync();
        Task<Line?> GetLineByIdAsync(int id);
        Task<int> AddLineAsync(Line line);
        Task<bool> UpdateLineAsync(Line line);
        Task<bool> DeleteLineAsync(int id);

        // Leurres
        Task<List<Lure>> GetAllLuresAsync();
        Task<Lure?> GetLureByIdAsync(int id);
        Task<int> AddLureAsync(Lure lure);
        Task<bool> UpdateLureAsync(Lure lure);
        Task<bool> DeleteLureAsync(int id);

        // Bas de ligne
        Task<List<Leader>> GetAllLeadersAsync();
        Task<Leader?> GetLeaderByIdAsync(int id);
        Task<int> AddLeaderAsync(Leader leader);
        Task<bool> UpdateLeaderAsync(Leader leader);
        Task<bool> DeleteLeaderAsync(int id);

        // Hameçons
        Task<List<Hook>> GetAllHooksAsync();
        Task<Hook?> GetHookByIdAsync(int id);
        Task<int> AddHookAsync(Hook hook);
        Task<bool> UpdateHookAsync(Hook hook);
        Task<bool> DeleteHookAsync(int id);
    }
}
