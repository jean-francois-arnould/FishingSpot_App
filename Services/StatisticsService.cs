using FishingSpot.PWA.Models;
using System.Globalization;

namespace FishingSpot.PWA.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ISupabaseService _supabaseService;
        private readonly ILoggerService _logger;
        private readonly IWeatherService? _weatherService;

        public StatisticsService(ISupabaseService supabaseService, ILoggerService logger, IWeatherService? weatherService = null)
        {
            _supabaseService = supabaseService;
            _logger = logger;
            _weatherService = weatherService;
        }

        public async Task<FishingStatistics> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Calculating fishing statistics");

                var allCatches = await _supabaseService.GetAllCatchesAsync();

                if (startDate.HasValue)
                    allCatches = allCatches.Where(c => c.CatchDate.Date >= startDate.Value.Date).ToList();
                if (endDate.HasValue)
                    allCatches = allCatches.Where(c => c.CatchDate.Date <= endDate.Value.Date).ToList();

                var stats = BuildStatistics(allCatches);

                _logger.LogInformation("Statistics calculated successfully", new Dictionary<string, object>
                {
                    { "TotalCatches", stats.TotalCatches },
                    { "TotalSpecies", stats.TotalSpecies }
                });

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error calculating statistics", ex);
                throw;
            }
        }

        public async Task<StatisticsDashboard> GetDashboardAsync(StatisticsPeriod period = StatisticsPeriod.Last90Days)
        {
            try
            {
                _logger.LogInformation("Calculating statistics dashboard", new Dictionary<string, object>
                {
                    { "Period", period.ToString() }
                });

                var allCatches = await _supabaseService.GetAllCatchesAsync();
                var totalAvailable = allCatches.Count;
                var endDate = DateTime.Today;
                var startDate = GetStartDate(period, endDate);
                var filteredCatches = allCatches
                    .Where(c => !startDate.HasValue || c.CatchDate.Date >= startDate.Value.Date)
                    .Where(c => c.CatchDate.Date <= endDate.Date)
                    .OrderByDescending(c => c.CatchDate)
                    .ThenByDescending(c => c.CatchTime ?? TimeSpan.Zero)
                    .ToList();

                var summary = BuildStatistics(filteredCatches);
                var setups = await GetSetupsSafelyAsync();
                var advancedInsights = await BuildAdvancedInsightsAsync(filteredCatches, setups);

                return new StatisticsDashboard
                {
                    Period = period,
                    PeriodLabel = GetPeriodLabel(period),
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalAvailableCatches = totalAvailable,
                    Summary = summary,
                    Conditions = BuildConditions(filteredCatches, summary),
                    Catches = filteredCatches,
                    SpeciesBreakdown = BuildSpeciesBreakdown(filteredCatches),
                    WeatherBreakdown = BuildWeatherBreakdown(filteredCatches),
                    HourlyBreakdown = BuildHourlyBreakdown(filteredCatches),
                    TopLocations = summary.TopLocations,
                    MonthlyTrends = summary.MonthlyTrends,
                    PersonalBests = BuildPersonalBests(filteredCatches),
                    AdvancedInsights = advancedInsights
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error calculating statistics dashboard", ex);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetCatchesByHourAsync()
        {
            try
            {
                var catches = await _supabaseService.GetAllCatchesAsync();

                return catches
                    .Where(c => c.CatchTime.HasValue)
                    .GroupBy(c => $"{c.CatchTime!.Value.Hours:00}:00")
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting catches by hour", ex);
                return new Dictionary<string, int>();
            }
        }

        public async Task<List<FishCatch>> GetPersonalBestsAsync()
        {
            try
            {
                var catches = await _supabaseService.GetAllCatchesAsync();
                return BuildPersonalBests(catches);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting personal bests", ex);
                return new List<FishCatch>();
            }
        }

        private async Task<List<FishingSetup>> GetSetupsSafelyAsync()
        {
            try
            {
                return await _supabaseService.GetAllSetupsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Unable to load setups for statistics", ex);
                return new List<FishingSetup>();
            }
        }

        private async Task<AdvancedFishingInsights> BuildAdvancedInsightsAsync(List<FishCatch> catches, List<FishingSetup> setups)
        {
            var sessionScores = BuildSessionScores(catches);
            var scoreByCatch = sessionScores.ToDictionary(s => s.Catch);
            var productiveSpots = BuildSpotProductivity(catches, scoreByCatch);

            return new AdvancedFishingInsights
            {
                ProductiveSpots = productiveSpots,
                SetupEfficiency = BuildSetupEfficiency(catches, setups, scoreByCatch),
                SessionScores = sessionScores,
                ReturnSuggestions = await BuildReturnSuggestionsAsync(catches, productiveSpots),
                SpeciesRecords = BuildSpeciesRecords(catches)
            };
        }

        private static List<SessionScore> BuildSessionScores(List<FishCatch> catches)
        {
            if (!catches.Any())
            {
                return new List<SessionScore>();
            }

            var speciesCounts = catches
                .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                .GroupBy(c => c.FishName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);
            var personalRecords = BuildPersonalBests(catches);
            var totalCatches = catches.Count;

            return catches
                .Select(fishCatch =>
                {
                    var speciesPeers = catches
                        .Where(c => string.Equals(c.FishName, fishCatch.FishName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    var lengthPeers = speciesPeers.Any(c => c.Length > 0) ? speciesPeers : catches;
                    var weightPeers = speciesPeers.Any(c => c.Weight > 0) ? speciesPeers : catches;
                    var speciesCount = !string.IsNullOrWhiteSpace(fishCatch.FishName) && speciesCounts.TryGetValue(fishCatch.FishName, out var count)
                        ? count
                        : totalCatches;
                    var rarityScore = totalCatches <= 1
                        ? 80
                        : 100 - ((speciesCount - 1) * 100.0 / Math.Max(1, totalCatches - 1));
                    var isRecord = personalRecords.Contains(fishCatch);
                    var lengthScore = PercentileRank(lengthPeers.Where(c => c.Length > 0).Select(c => c.Length), fishCatch.Length);
                    var weightScore = PercentileRank(weightPeers.Where(c => c.Weight > 0).Select(c => c.Weight), fishCatch.Weight);
                    var weatherScore = BuildWeatherCompletenessScore(fishCatch);
                    var score = Math.Clamp(
                        (lengthScore * 0.30) +
                        (weightScore * 0.25) +
                        (rarityScore * 0.18) +
                        (weatherScore * 0.15) +
                        (isRecord ? 12 : 0),
                        0,
                        100);

                    return new SessionScore
                    {
                        CatchId = fishCatch.Id,
                        Catch = fishCatch,
                        Score = Math.Round(score, 1),
                        Grade = GetScoreGrade(score),
                        PrimaryReason = GetScoreReason(lengthScore, weightScore, rarityScore, weatherScore, isRecord),
                        LengthScore = Math.Round(lengthScore, 1),
                        WeightScore = Math.Round(weightScore, 1),
                        RarityScore = Math.Round(rarityScore, 1),
                        WeatherScore = Math.Round(weatherScore, 1),
                        IsPersonalRecord = isRecord
                    };
                })
                .OrderByDescending(s => s.Score)
                .ThenByDescending(s => s.Catch.CatchDate)
                .Take(30)
                .ToList();
        }

        private static List<SpotProductivity> BuildSpotProductivity(List<FishCatch> catches, Dictionary<FishCatch, SessionScore> scoreByCatch)
        {
            var spots = catches
                .Select(c => new { Catch = c, Key = BuildSpotKey(c) })
                .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                .GroupBy(x => x.Key)
                .Select(g =>
                {
                    var groupCatches = g.Select(x => x.Catch).ToList();
                    var first = groupCatches.First();
                    TryGetCoordinates(first, out var latitude, out var longitude);
                    var bestCatch = groupCatches
                        .OrderByDescending(c => scoreByCatch.TryGetValue(c, out var score) ? score.Score : 0)
                        .ThenByDescending(c => c.Length)
                        .FirstOrDefault();
                    var bestScore = bestCatch != null && scoreByCatch.TryGetValue(bestCatch, out var sessionScore)
                        ? sessionScore.Score
                        : 0;

                    return new SpotProductivity
                    {
                        SpotKey = g.Key,
                        LocationName = GetDisplayLocation(first),
                        Latitude = latitude,
                        Longitude = longitude,
                        Count = groupCatches.Count,
                        SpeciesCount = groupCatches
                            .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                            .Select(c => c.FishName)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .Count(),
                        TotalWeight = groupCatches.Sum(c => c.Weight),
                        AverageLength = groupCatches.Average(c => c.Length),
                        AverageWeight = groupCatches.Average(c => c.Weight),
                        BestSessionScore = bestScore,
                        LastCatchDate = groupCatches.Max(c => c.CatchDate),
                        BestCatch = bestCatch,
                        Species = groupCatches
                            .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                            .GroupBy(c => c.FishName, StringComparer.OrdinalIgnoreCase)
                            .OrderByDescending(s => s.Count())
                            .ThenBy(s => s.Key)
                            .Select(s => s.Key)
                            .Take(5)
                            .ToList()
                    };
                })
                .ToList();

            foreach (var spot in spots)
            {
                spot.ProductivityScore =
                    (spot.Count * 14) +
                    (spot.SpeciesCount * 8) +
                    ((spot.TotalWeight / 1000.0) * 1.5) +
                    (spot.AverageLength * 0.20) +
                    (spot.BestSessionScore * 0.25);
            }

            NormalizeScores(spots, s => s.ProductivityScore, (s, value) => s.ProductivityScore = value);

            return spots
                .OrderByDescending(s => s.ProductivityScore)
                .ThenByDescending(s => s.Count)
                .ThenBy(s => s.LocationName)
                .Take(20)
                .ToList();
        }

        private static List<SetupEfficiency> BuildSetupEfficiency(
            List<FishCatch> catches,
            List<FishingSetup> setups,
            Dictionary<FishCatch, SessionScore> scoreByCatch)
        {
            var setupsById = setups
                .GroupBy(s => s.Id)
                .ToDictionary(g => g.Key, g => g.First());
            var efficiencies = catches
                .Where(c => c.SetupId.HasValue)
                .GroupBy(c => c.SetupId!.Value)
                .Select(g =>
                {
                    var groupCatches = g.ToList();
                    setupsById.TryGetValue(g.Key, out var setup);
                    var bestCatch = groupCatches
                        .OrderByDescending(c => scoreByCatch.TryGetValue(c, out var score) ? score.Score : 0)
                        .ThenByDescending(c => c.Length)
                        .FirstOrDefault();
                    var bestScore = bestCatch != null && scoreByCatch.TryGetValue(bestCatch, out var sessionScore)
                        ? sessionScore.Score
                        : 0;

                    return new SetupEfficiency
                    {
                        SetupId = g.Key,
                        SetupName = string.IsNullOrWhiteSpace(setup?.Name) ? $"Montage #{g.Key}" : setup!.Name,
                        Count = groupCatches.Count,
                        SpeciesCount = groupCatches
                            .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                            .Select(c => c.FishName)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .Count(),
                        TotalWeight = groupCatches.Sum(c => c.Weight),
                        AverageLength = groupCatches.Average(c => c.Length),
                        AverageWeight = groupCatches.Average(c => c.Weight),
                        BestSessionScore = bestScore,
                        LastCatchDate = groupCatches.Max(c => c.CatchDate),
                        BestCatch = bestCatch,
                        EfficiencyScore =
                            (groupCatches.Count * 16) +
                            (groupCatches.Sum(c => c.Weight) / 1000.0 * 1.4) +
                            (groupCatches.Average(c => c.Length) * 0.22) +
                            (bestScore * 0.30)
                    };
                })
                .ToList();

            NormalizeScores(efficiencies, s => s.EfficiencyScore, (s, value) => s.EfficiencyScore = value);

            return efficiencies
                .OrderByDescending(s => s.EfficiencyScore)
                .ThenByDescending(s => s.Count)
                .ThenBy(s => s.SetupName)
                .ToList();
        }

        private async Task<List<ReturnSuggestion>> BuildReturnSuggestionsAsync(List<FishCatch> catches, List<SpotProductivity> spots)
        {
            if (!catches.Any())
            {
                return new List<ReturnSuggestion>();
            }

            var suggestions = new List<ReturnSuggestion>();
            var candidateSpots = spots.Any()
                ? spots.Take(3).ToList()
                : new List<SpotProductivity>
                {
                    new()
                    {
                        SpotKey = "all",
                        LocationName = "Tous les spots",
                        Count = catches.Count,
                        SpeciesCount = catches.Select(c => c.FishName).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                        LastCatchDate = catches.Max(c => c.CatchDate)
                    }
                };

            var suggestionTasks = candidateSpots.Select(async spot =>
            {
                var spotCatches = spot.SpotKey == "all"
                    ? catches
                    : catches.Where(c => IsSameSpot(spot, c)).ToList();

                ReturnSuggestion? suggestion = null;
                if (_weatherService != null && spot.HasCoordinates)
                {
                    suggestion = await BuildForecastSuggestionAsync(spot, spotCatches);
                }

                return suggestion ?? BuildHistoricalSuggestion(spot, spotCatches);
            });

            suggestions.AddRange(await Task.WhenAll(suggestionTasks));

            return suggestions
                .OrderByDescending(s => s.ConfidenceScore)
                .ThenBy(s => s.SuggestedAt)
                .Take(3)
                .ToList();
        }

        private async Task<ReturnSuggestion?> BuildForecastSuggestionAsync(SpotProductivity spot, List<FishCatch> spotCatches)
        {
            if (_weatherService == null || !spot.Latitude.HasValue || !spot.Longitude.HasValue || !spotCatches.Any())
            {
                return null;
            }

            var forecast = await _weatherService.GetHourlyForecastAsync(spot.Latitude.Value, spot.Longitude.Value, 7);
            if (forecast?.Hours == null || !forecast.Hours.Any())
            {
                return null;
            }

            var bestSlot = GetBestSlot(spotCatches);
            var bestWeatherFamily = spotCatches
                .Where(c => c.WeatherCode.HasValue)
                .GroupBy(c => WeatherFamily(c.WeatherCode!.Value))
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .Select(g => (int?)g.Key)
                .FirstOrDefault();
            var historicalMatches = spotCatches.Count(c => c.CatchTime.HasValue && c.CatchTime.Value.Hours / 3 == bestSlot);

            var bestHour = forecast.Hours
                .Where(h => h.Time >= DateTime.Now.AddHours(1))
                .Select(h => new
                {
                    Hour = h,
                    Score = BuildForecastSuggestionScore(h, bestSlot, bestWeatherFamily)
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Hour.Time)
                .FirstOrDefault();

            if (bestHour == null)
            {
                return null;
            }

            return new ReturnSuggestion
            {
                SpotName = spot.LocationName,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                SuggestedAt = bestHour.Hour.Time,
                TimeSlot = FormatTimeSlot(bestSlot),
                Reason = $"{historicalMatches} prise(s) historiques sur ce creneau",
                WeatherSummary = FormatForecastSummary(bestHour.Hour),
                ConfidenceScore = Math.Round(bestHour.Score, 1),
                HistoricalMatches = historicalMatches,
                UsesForecast = true,
                WeatherCode = bestHour.Hour.WeatherCode,
                Temperature = bestHour.Hour.Temperature,
                WindSpeed = bestHour.Hour.WindSpeed,
                Humidity = bestHour.Hour.Humidity,
                PrecipitationProbability = bestHour.Hour.PrecipitationProbability
            };
        }

        private static ReturnSuggestion BuildHistoricalSuggestion(SpotProductivity spot, List<FishCatch> spotCatches)
        {
            var bestSlot = GetBestSlot(spotCatches);
            var bestDay = GetBestDayOfWeek(spotCatches);
            var suggestedAt = NextOccurrence(bestDay, bestSlot * 3);
            var historicalMatches = spotCatches.Count(c => c.CatchTime.HasValue && c.CatchTime.Value.Hours / 3 == bestSlot);

            return new ReturnSuggestion
            {
                SpotName = spot.LocationName,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                SuggestedAt = suggestedAt,
                TimeSlot = FormatTimeSlot(bestSlot),
                Reason = $"{historicalMatches} prise(s) historiques sur ce creneau",
                WeatherSummary = "Prevision indisponible",
                ConfidenceScore = Math.Round(Math.Clamp(45 + (historicalMatches * 8.0), 35, 82), 1),
                HistoricalMatches = historicalMatches,
                UsesForecast = false
            };
        }

        private static List<SpeciesRecord> BuildSpeciesRecords(List<FishCatch> catches)
        {
            return catches
                .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                .GroupBy(c => c.FishName, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    var record = g
                        .OrderByDescending(c => c.Length)
                        .ThenByDescending(c => c.Weight)
                        .ThenByDescending(c => c.CatchDate)
                        .First();

                    return new SpeciesRecord
                    {
                        FishName = record.FishName,
                        Catch = record,
                        Length = record.Length,
                        Weight = record.Weight,
                        PhotoUrl = record.PhotoUrl,
                        CatchDate = record.CatchDate,
                        LocationName = record.LocationName
                    };
                })
                .OrderByDescending(r => r.Length)
                .ThenByDescending(r => r.Weight)
                .ToList();
        }

        private static FishingStatistics BuildStatistics(List<FishCatch> catches)
        {
            return new FishingStatistics
            {
                TotalCatches = catches.Count,
                TotalSpecies = catches
                    .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                    .Select(c => c.FishName)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count(),
                TotalWeight = catches.Sum(c => c.Weight),
                AverageWeight = catches.Any() ? catches.Average(c => c.Weight) : 0,
                AverageLength = catches.Any() ? catches.Average(c => c.Length) : 0,
                BiggestCatch = catches.OrderByDescending(c => c.Length).FirstOrDefault(),
                HeaviestCatch = catches.OrderByDescending(c => c.Weight).FirstOrDefault(),

                CatchesBySpecies = catches
                    .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                    .GroupBy(c => c.FishName)
                    .OrderByDescending(g => g.Count())
                    .ToDictionary(g => g.Key, g => g.Count()),

                AverageSizeBySpecies = catches
                    .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                    .GroupBy(c => c.FishName)
                    .ToDictionary(g => g.Key, g => g.Average(c => c.Length)),

                CatchesByMonth = catches
                    .GroupBy(c => c.CatchDate.ToString("yyyy-MM", CultureInfo.InvariantCulture))
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Count()),

                TopLocations = catches
                    .Where(c => !string.IsNullOrWhiteSpace(c.LocationName))
                    .GroupBy(c => c.LocationName)
                    .Select(g => new LocationStats
                    {
                        LocationName = g.Key,
                        Count = g.Count(),
                        AverageSize = g.Average(c => c.Length)
                    })
                    .OrderByDescending(l => l.Count)
                    .ThenBy(l => l.LocationName)
                    .Take(5)
                    .ToList(),

                BestTimeOfDay = catches
                    .Where(c => c.CatchTime.HasValue)
                    .GroupBy(c => c.CatchTime!.Value.Hours)
                    .OrderByDescending(g => g.Count())
                    .ThenBy(g => g.Key)
                    .Select(g => TimeSpan.FromHours(g.Key))
                    .FirstOrDefault(),

                CatchesByWeather = catches
                    .Where(c => !string.IsNullOrWhiteSpace(c.WeatherCondition))
                    .GroupBy(c => c.WeatherCondition!)
                    .OrderByDescending(g => g.Count())
                    .ToDictionary(g => g.Key, g => g.Count()),

                MonthlyTrends = catches
                    .GroupBy(c => new DateTime(c.CatchDate.Year, c.CatchDate.Month, 1))
                    .OrderBy(g => g.Key)
                    .Select(g => new MonthlyTrend
                    {
                        MonthDate = g.Key,
                        Month = g.Key.ToString("MMM yyyy", CultureInfo.CurrentCulture),
                        Count = g.Count(),
                        TotalWeight = g.Sum(c => c.Weight)
                    })
                    .ToList()
            };
        }

        private static ConditionsSummary BuildConditions(List<FishCatch> catches, FishingStatistics summary)
        {
            var weatherCatches = catches
                .Where(c => c.WeatherTemperature.HasValue || !string.IsNullOrWhiteSpace(c.WeatherCondition) || c.WindSpeed.HasValue || c.Humidity.HasValue)
                .ToList();

            var bestWeather = catches
                .Where(c => !string.IsNullOrWhiteSpace(c.WeatherCondition))
                .GroupBy(c => c.WeatherCondition!)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .FirstOrDefault();

            var bestSlot = catches
                .Where(c => c.CatchTime.HasValue)
                .GroupBy(c => c.CatchTime!.Value.Hours / 3)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .FirstOrDefault();

            return new ConditionsSummary
            {
                BestTimeSlot = bestSlot == null ? "Non renseignée" : FormatTimeSlot(bestSlot.Key),
                BestTimeSlotCount = bestSlot?.Count() ?? 0,
                BestWeatherCondition = bestWeather?.Key ?? "Non renseignée",
                BestWeatherCount = bestWeather?.Count() ?? 0,
                AverageTemperature = AverageOrNull(weatherCatches.Where(c => c.WeatherTemperature.HasValue).Select(c => c.WeatherTemperature!.Value)),
                AverageWindSpeed = AverageOrNull(weatherCatches.Where(c => c.WindSpeed.HasValue).Select(c => c.WindSpeed!.Value)),
                AverageHumidity = AverageOrNull(weatherCatches.Where(c => c.Humidity.HasValue).Select(c => (double)c.Humidity!.Value)),
                WeatherSampleCount = weatherCatches.Count,
                BestLocation = summary.TopLocations.FirstOrDefault()
            };
        }

        private static List<SpeciesBreakdown> BuildSpeciesBreakdown(List<FishCatch> catches)
        {
            return catches
                .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                .GroupBy(c => c.FishName)
                .Select(g => new SpeciesBreakdown
                {
                    FishName = g.Key,
                    Count = g.Count(),
                    AverageLength = g.Average(c => c.Length),
                    TotalWeight = g.Sum(c => c.Weight),
                    BiggestLength = g.Max(c => c.Length),
                    HeaviestWeight = g.Max(c => c.Weight)
                })
                .OrderByDescending(s => s.Count)
                .ThenBy(s => s.FishName)
                .ToList();
        }

        private static List<WeatherBreakdown> BuildWeatherBreakdown(List<FishCatch> catches)
        {
            return catches
                .Where(c => !string.IsNullOrWhiteSpace(c.WeatherCondition))
                .GroupBy(c => new { c.WeatherCondition, c.WeatherCode })
                .Select(g => new WeatherBreakdown
                {
                    Condition = g.Key.WeatherCondition!,
                    WeatherCode = g.Key.WeatherCode,
                    Count = g.Count(),
                    AverageTemperature = AverageOrNull(g.Where(c => c.WeatherTemperature.HasValue).Select(c => c.WeatherTemperature!.Value)),
                    AverageWindSpeed = AverageOrNull(g.Where(c => c.WindSpeed.HasValue).Select(c => c.WindSpeed!.Value))
                })
                .OrderByDescending(w => w.Count)
                .ThenBy(w => w.Condition)
                .ToList();
        }

        private static List<HourlyBreakdown> BuildHourlyBreakdown(List<FishCatch> catches)
        {
            var countsByHour = catches
                .Where(c => c.CatchTime.HasValue)
                .GroupBy(c => c.CatchTime!.Value.Hours)
                .ToDictionary(g => g.Key, g => g.Count());

            return Enumerable.Range(0, 24)
                .Select(hour => new HourlyBreakdown
                {
                    Hour = hour,
                    Label = $"{hour:00}h",
                    Count = countsByHour.TryGetValue(hour, out var count) ? count : 0
                })
                .ToList();
        }

        private static List<FishCatch> BuildPersonalBests(List<FishCatch> catches)
        {
            return catches
                .Where(c => !string.IsNullOrWhiteSpace(c.FishName))
                .GroupBy(c => c.FishName)
                .Select(g => g.OrderByDescending(c => c.Length).ThenByDescending(c => c.Weight).First())
                .OrderByDescending(c => c.Length)
                .ThenByDescending(c => c.Weight)
                .Take(10)
                .ToList();
        }

        private static double PercentileRank(IEnumerable<double> values, double value)
        {
            var materialized = values
                .Where(v => v > 0)
                .OrderBy(v => v)
                .ToList();

            if (value <= 0 || !materialized.Any())
            {
                return 0;
            }

            var lowerOrEqual = materialized.Count(v => v <= value);
            return Math.Clamp(lowerOrEqual * 100.0 / materialized.Count, 0, 100);
        }

        private static double BuildWeatherCompletenessScore(FishCatch fishCatch)
        {
            var score = 40.0;
            if (fishCatch.WeatherTemperature.HasValue)
                score += 15;
            if (!string.IsNullOrWhiteSpace(fishCatch.WeatherCondition) || fishCatch.WeatherCode.HasValue)
                score += 20;
            if (fishCatch.WindSpeed.HasValue)
                score += 15;
            if (fishCatch.Humidity.HasValue)
                score += 10;

            return Math.Clamp(score, 0, 100);
        }

        private static string GetScoreGrade(double score)
        {
            return score switch
            {
                >= 85 => "A+",
                >= 75 => "A",
                >= 65 => "B",
                >= 50 => "C",
                _ => "D"
            };
        }

        private static string GetScoreReason(double lengthScore, double weightScore, double rarityScore, double weatherScore, bool isRecord)
        {
            if (isRecord)
                return "Record personnel";
            if (rarityScore >= 82)
                return "Espece rare";
            if (lengthScore >= weightScore && lengthScore >= 70)
                return "Belle longueur";
            if (weightScore >= 70)
                return "Poids solide";
            if (weatherScore >= 85)
                return "Conditions completes";

            return "Session reguliere";
        }

        private static void NormalizeScores<T>(List<T> items, Func<T, double> getScore, Action<T, double> setScore)
        {
            var maxScore = items.Any() ? items.Max(getScore) : 0;
            if (maxScore <= 0)
            {
                return;
            }

            foreach (var item in items)
            {
                setScore(item, Math.Round(Math.Clamp(getScore(item) * 100.0 / maxScore, 0, 100), 1));
            }
        }

        private static string BuildSpotKey(FishCatch fishCatch)
        {
            if (TryGetCoordinates(fishCatch, out var latitude, out var longitude))
            {
                return $"geo:{Math.Round(latitude!.Value, 4).ToString("0.0000", CultureInfo.InvariantCulture)}:{Math.Round(longitude!.Value, 4).ToString("0.0000", CultureInfo.InvariantCulture)}";
            }

            return string.IsNullOrWhiteSpace(fishCatch.LocationName)
                ? string.Empty
                : $"loc:{fishCatch.LocationName.Trim().ToLowerInvariant()}";
        }

        private static bool TryGetCoordinates(FishCatch fishCatch, out double? latitude, out double? longitude)
        {
            latitude = null;
            longitude = null;

            if (!TryParseCoordinate(fishCatch.Latitude, out var parsedLatitude) ||
                !TryParseCoordinate(fishCatch.Longitude, out var parsedLongitude))
            {
                return false;
            }

            latitude = parsedLatitude;
            longitude = parsedLongitude;
            return true;
        }

        private static bool TryParseCoordinate(string? value, out double coordinate)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out coordinate))
            {
                return true;
            }

            return double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out coordinate);
        }

        private static string GetDisplayLocation(FishCatch fishCatch)
        {
            if (!string.IsNullOrWhiteSpace(fishCatch.LocationName))
            {
                return fishCatch.LocationName.Trim();
            }

            return TryGetCoordinates(fishCatch, out var latitude, out var longitude)
                ? $"{latitude!.Value.ToString("0.0000", CultureInfo.InvariantCulture)}, {longitude!.Value.ToString("0.0000", CultureInfo.InvariantCulture)}"
                : "Lieu non renseigne";
        }

        private static bool IsSameSpot(SpotProductivity spot, FishCatch fishCatch)
        {
            if (spot.HasCoordinates && TryGetCoordinates(fishCatch, out var latitude, out var longitude))
            {
                return Math.Round(spot.Latitude!.Value, 4) == Math.Round(latitude!.Value, 4) &&
                       Math.Round(spot.Longitude!.Value, 4) == Math.Round(longitude!.Value, 4);
            }

            return string.Equals(spot.LocationName, GetDisplayLocation(fishCatch), StringComparison.OrdinalIgnoreCase);
        }

        private static int GetBestSlot(List<FishCatch> catches)
        {
            return catches
                .Where(c => c.CatchTime.HasValue)
                .GroupBy(c => c.CatchTime!.Value.Hours / 3)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .Select(g => g.Key)
                .FirstOrDefault(2);
        }

        private static DayOfWeek GetBestDayOfWeek(List<FishCatch> catches)
        {
            return catches
                .GroupBy(c => c.CatchDate.DayOfWeek)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .Select(g => g.Key)
                .FirstOrDefault(DateTime.Today.AddDays(1).DayOfWeek);
        }

        private static DateTime NextOccurrence(DayOfWeek dayOfWeek, int hour)
        {
            var today = DateTime.Today;
            var daysUntil = ((int)dayOfWeek - (int)today.DayOfWeek + 7) % 7;
            var candidate = today.AddDays(daysUntil).AddHours(hour);

            if (candidate <= DateTime.Now.AddHours(1))
            {
                candidate = candidate.AddDays(7);
            }

            return candidate;
        }

        private static double BuildForecastSuggestionScore(HourlyForecast forecast, int bestSlot, int? bestWeatherFamily)
        {
            var slot = forecast.Time.Hour / 3;
            var slotScore = slot == bestSlot
                ? 45
                : IsNeighborSlot(slot, bestSlot) ? 26 : 8;
            var weatherMatchScore = bestWeatherFamily.HasValue && forecast.WeatherCode.HasValue
                ? WeatherFamily(forecast.WeatherCode.Value) == bestWeatherFamily.Value ? 25 : 10
                : 14;

            return Math.Clamp(slotScore + weatherMatchScore + BuildForecastSuitabilityScore(forecast), 0, 100);
        }

        private static bool IsNeighborSlot(int slot, int bestSlot)
        {
            return Math.Abs(slot - bestSlot) == 1 || Math.Abs(slot - bestSlot) == 7;
        }

        private static double BuildForecastSuitabilityScore(HourlyForecast forecast)
        {
            var score = 30.0;

            if (forecast.PrecipitationProbability.HasValue)
            {
                score -= Math.Max(0, forecast.PrecipitationProbability.Value - 35) * 0.28;
            }

            if (forecast.WindSpeed.HasValue)
            {
                score -= Math.Max(0, forecast.WindSpeed.Value - 22) * 0.75;
            }

            if (forecast.Temperature.HasValue)
            {
                if (forecast.Temperature.Value < 4)
                    score -= 6;
                if (forecast.Temperature.Value > 30)
                    score -= 6;
            }

            return Math.Clamp(score, 0, 30);
        }

        private static int WeatherFamily(int code)
        {
            return code switch
            {
                0 or 1 => 0,
                2 or 3 => 1,
                45 or 48 => 2,
                51 or 53 or 55 or 61 or 63 or 65 or 80 or 81 or 82 => 3,
                71 or 73 or 75 or 77 or 85 or 86 => 4,
                95 or 96 or 99 => 5,
                _ => 6
            };
        }

        private static string FormatForecastSummary(HourlyForecast forecast)
        {
            var parts = new List<string>();

            if (forecast.WeatherCode.HasValue)
                parts.Add(WeatherData.GetWeatherDescription(forecast.WeatherCode.Value));
            if (forecast.Temperature.HasValue)
                parts.Add($"{forecast.Temperature.Value.ToString("0.#", CultureInfo.CurrentCulture)} C");
            if (forecast.WindSpeed.HasValue)
                parts.Add($"vent {forecast.WindSpeed.Value.ToString("0.#", CultureInfo.CurrentCulture)} km/h");
            if (forecast.PrecipitationProbability.HasValue)
                parts.Add($"pluie {forecast.PrecipitationProbability.Value}%");

            return parts.Any() ? string.Join(", ", parts) : "Prevision disponible";
        }

        private static DateTime? GetStartDate(StatisticsPeriod period, DateTime endDate)
        {
            return period switch
            {
                StatisticsPeriod.Last30Days => endDate.AddDays(-29),
                StatisticsPeriod.Last90Days => endDate.AddDays(-89),
                StatisticsPeriod.CurrentYear => new DateTime(endDate.Year, 1, 1),
                _ => null
            };
        }

        private static string GetPeriodLabel(StatisticsPeriod period)
        {
            return period switch
            {
                StatisticsPeriod.Last30Days => "30 derniers jours",
                StatisticsPeriod.Last90Days => "90 derniers jours",
                StatisticsPeriod.CurrentYear => "Cette année",
                _ => "Tout l'historique"
            };
        }

        private static string FormatTimeSlot(int slotIndex)
        {
            var startHour = slotIndex * 3;
            var endHour = (startHour + 3) % 24;
            return $"{startHour:00}:00 - {endHour:00}:00";
        }

        private static double? AverageOrNull(IEnumerable<double> values)
        {
            var materialized = values.ToList();
            return materialized.Any() ? materialized.Average() : null;
        }
    }
}
