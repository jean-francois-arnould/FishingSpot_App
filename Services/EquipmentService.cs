using FishingSpot.PWA.Models.Equipment;

namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Refactored Equipment Service using repository pattern to reduce code duplication
    /// </summary>
    public class EquipmentService : BaseSupabaseService, IEquipmentService
    {
        private readonly SupabaseRepository<Rod> _rodRepository;
        private readonly SupabaseRepository<Reel> _reelRepository;
        private readonly SupabaseRepository<Line> _lineRepository;
        private readonly SupabaseRepository<Lure> _lureRepository;
        private readonly SupabaseRepository<Leader> _leaderRepository;
        private readonly SupabaseRepository<Hook> _hookRepository;

        public EquipmentService(HttpClient httpClient, IConfiguration configuration, IAuthService authService)
            : base(httpClient, configuration, authService)
        {
            _rodRepository = new SupabaseRepository<Rod>(httpClient, "rods");
            _reelRepository = new SupabaseRepository<Reel>(httpClient, "reels");
            _lineRepository = new SupabaseRepository<Line>(httpClient, "lines");
            _lureRepository = new SupabaseRepository<Lure>(httpClient, "lures");
            _leaderRepository = new SupabaseRepository<Leader>(httpClient, "leaders");
            _hookRepository = new SupabaseRepository<Hook>(httpClient, "hooks");
        }

        private void SetUserIdAndCreatedAt<T>(T equipment) where T : BaseEquipment
        {
            if (_authService.CurrentUser != null)
                equipment.UserId = _authService.CurrentUser.Id;
            equipment.CreatedAt = DateTime.UtcNow;
        }

        // ============================================
        // CANNES
        // ============================================
        public async Task<List<Rod>> GetAllRodsAsync()
        {
            SetAuthHeaders();
            return await _rodRepository.GetAllAsync("created_at.desc");
        }

        public async Task<Rod?> GetRodByIdAsync(int id)
        {
            SetAuthHeaders();
            return await _rodRepository.GetByIdAsync(id);
        }

        public async Task<int> AddRodAsync(Rod rod)
        {
            SetAuthHeaders();
            SetUserIdAndCreatedAt(rod);
            return await _rodRepository.AddAsync(rod);
        }

        public async Task<bool> UpdateRodAsync(Rod rod)
        {
            SetAuthHeaders();
            return await _rodRepository.UpdateAsync(rod.Id, rod);
        }

        public async Task<bool> DeleteRodAsync(int id)
        {
            SetAuthHeaders();
            return await _rodRepository.DeleteAsync(id);
        }

        // ============================================
        // MOULINETS
        // ============================================
        public async Task<List<Reel>> GetAllReelsAsync()
        {
            SetAuthHeaders();
            return await _reelRepository.GetAllAsync("created_at.desc");
        }

        public async Task<Reel?> GetReelByIdAsync(int id)
        {
            SetAuthHeaders();
            return await _reelRepository.GetByIdAsync(id);
        }

        public async Task<int> AddReelAsync(Reel reel)
        {
            SetAuthHeaders();
            SetUserIdAndCreatedAt(reel);
            return await _reelRepository.AddAsync(reel);
        }

        public async Task<bool> UpdateReelAsync(Reel reel)
        {
            SetAuthHeaders();
            return await _reelRepository.UpdateAsync(reel.Id, reel);
        }

        public async Task<bool> DeleteReelAsync(int id)
        {
            SetAuthHeaders();
            return await _reelRepository.DeleteAsync(id);
        }

        // ============================================
        // FILS
        // ============================================
        public async Task<List<Line>> GetAllLinesAsync()
        {
            SetAuthHeaders();
            return await _lineRepository.GetAllAsync("created_at.desc");
        }

        public async Task<Line?> GetLineByIdAsync(int id)
        {
            SetAuthHeaders();
            return await _lineRepository.GetByIdAsync(id);
        }

        public async Task<int> AddLineAsync(Line line)
        {
            SetAuthHeaders();
            SetUserIdAndCreatedAt(line);
            return await _lineRepository.AddAsync(line);
        }

        public async Task<bool> UpdateLineAsync(Line line)
        {
            SetAuthHeaders();
            return await _lineRepository.UpdateAsync(line.Id, line);
        }

        public async Task<bool> DeleteLineAsync(int id)
        {
            SetAuthHeaders();
            return await _lineRepository.DeleteAsync(id);
        }

        // ============================================
        // LEURRES
        // ============================================
        public async Task<List<Lure>> GetAllLuresAsync()
        {
            SetAuthHeaders();
            return await _lureRepository.GetAllAsync("created_at.desc");
        }

        public async Task<Lure?> GetLureByIdAsync(int id)
        {
            SetAuthHeaders();
            return await _lureRepository.GetByIdAsync(id);
        }

        public async Task<int> AddLureAsync(Lure lure)
        {
            SetAuthHeaders();
            SetUserIdAndCreatedAt(lure);
            return await _lureRepository.AddAsync(lure);
        }

        public async Task<bool> UpdateLureAsync(Lure lure)
        {
            SetAuthHeaders();
            return await _lureRepository.UpdateAsync(lure.Id, lure);
        }

        public async Task<bool> DeleteLureAsync(int id)
        {
            SetAuthHeaders();
            return await _lureRepository.DeleteAsync(id);
        }

        // ============================================
        // BAS DE LIGNE
        // ============================================
        public async Task<List<Leader>> GetAllLeadersAsync()
        {
            SetAuthHeaders();
            return await _leaderRepository.GetAllAsync("created_at.desc");
        }

        public async Task<Leader?> GetLeaderByIdAsync(int id)
        {
            SetAuthHeaders();
            return await _leaderRepository.GetByIdAsync(id);
        }

        public async Task<int> AddLeaderAsync(Leader leader)
        {
            SetAuthHeaders();
            SetUserIdAndCreatedAt(leader);
            return await _leaderRepository.AddAsync(leader);
        }

        public async Task<bool> UpdateLeaderAsync(Leader leader)
        {
            SetAuthHeaders();
            return await _leaderRepository.UpdateAsync(leader.Id, leader);
        }

        public async Task<bool> DeleteLeaderAsync(int id)
        {
            SetAuthHeaders();
            return await _leaderRepository.DeleteAsync(id);
        }

        // ============================================
        // HAMEÇONS
        // ============================================
        public async Task<List<Hook>> GetAllHooksAsync()
        {
            SetAuthHeaders();
            return await _hookRepository.GetAllAsync("created_at.desc");
        }

        public async Task<Hook?> GetHookByIdAsync(int id)
        {
            SetAuthHeaders();
            return await _hookRepository.GetByIdAsync(id);
        }

        public async Task<int> AddHookAsync(Hook hook)
        {
            SetAuthHeaders();
            SetUserIdAndCreatedAt(hook);
            return await _hookRepository.AddAsync(hook);
        }

        public async Task<bool> UpdateHookAsync(Hook hook)
        {
            SetAuthHeaders();
            return await _hookRepository.UpdateAsync(hook.Id, hook);
        }

        public async Task<bool> DeleteHookAsync(int id)
        {
            SetAuthHeaders();
            return await _hookRepository.DeleteAsync(id);
        }
    }
}
