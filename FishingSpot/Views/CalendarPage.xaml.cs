using FishingSpot.Models;
using FishingSpot.Services;

namespace FishingSpot.Views
{
    public partial class CalendarPage : ContentPage
    {
        private readonly StatisticsService _statisticsService;
        private readonly SQLiteDatabaseService _databaseService;
        private DateTime _currentMonth;
        private Dictionary<int, List<FishCatch>> _catchesByDay = new();

        public CalendarPage(StatisticsService statisticsService, SQLiteDatabaseService databaseService)
        {
            InitializeComponent();
            _statisticsService = statisticsService;
            _databaseService = databaseService;
            _currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCalendarAsync();
        }

        private async Task LoadCalendarAsync()
        {
            MonthYearLabel.Text = _currentMonth.ToString("MMMM yyyy");

            var startDate = _currentMonth;
            var endDate = _currentMonth.AddMonths(1).AddDays(-1);

            var catches = await _statisticsService.GetCatchesByDateRangeAsync(startDate, endDate);

            _catchesByDay = catches
                .GroupBy(c => c.CatchDate.Day)
                .ToDictionary(g => g.Key, g => g.ToList());

            BuildCalendar();
            UpdateMonthStatistics();
        }

        private void BuildCalendar()
        {
            CalendarGrid.Children.Clear();

            var firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);

            var startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            if (startDayOfWeek == 0) startDayOfWeek = 7;
            startDayOfWeek--;

            int currentRow = 0;
            int currentCol = startDayOfWeek;

            for (int day = 1; day <= daysInMonth; day++)
            {
                var dayFrame = CreateDayFrame(day);
                Grid.SetRow(dayFrame, currentRow);
                Grid.SetColumn(dayFrame, currentCol);
                CalendarGrid.Children.Add(dayFrame);

                currentCol++;
                if (currentCol > 6)
                {
                    currentCol = 0;
                    currentRow++;
                }
            }
        }

        private Frame CreateDayFrame(int day)
        {
            var hasCatches = _catchesByDay.ContainsKey(day);
            var catchCount = hasCatches ? _catchesByDay[day].Count : 0;

            var frame = new Frame
            {
                Padding = 5,
                CornerRadius = 5,
                BackgroundColor = hasCatches ? Colors.LightGreen : Colors.White,
                BorderColor = Colors.Gray
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnDayTapped(day);
            frame.GestureRecognizers.Add(tapGesture);

            var stackLayout = new VerticalStackLayout
            {
                Spacing = 2
            };

            var dayLabel = new Label
            {
                Text = day.ToString(),
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            stackLayout.Children.Add(dayLabel);

            if (hasCatches)
            {
                var catchLabel = new Label
                {
                    Text = $"🐟 {catchCount}",
                    FontSize = 10,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                stackLayout.Children.Add(catchLabel);
            }

            frame.Content = stackLayout;

            return frame;
        }

        private void OnDayTapped(int day)
        {
            if (_catchesByDay.ContainsKey(day))
            {
                var selectedDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
                SelectedDateLabel.Text = $"Captures du {selectedDate:dd MMMM yyyy}";
                DailyCatchesCollectionView.ItemsSource = _catchesByDay[day];
                CatchesFrame.IsVisible = true;
            }
            else
            {
                CatchesFrame.IsVisible = false;
            }
        }

        private void UpdateMonthStatistics()
        {
            var totalCatches = _catchesByDay.Values.Sum(list => list.Count);
            var fishingDays = _catchesByDay.Count;
            var averagePerDay = fishingDays > 0 ? (double)totalCatches / fishingDays : 0;

            MonthTotalLabel.Text = totalCatches.ToString();
            FishingDaysLabel.Text = fishingDays.ToString();
            AveragePerDayLabel.Text = $"{averagePerDay:F1}";
        }

        private async void OnPreviousMonthClicked(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            CatchesFrame.IsVisible = false;
            await LoadCalendarAsync();
        }

        private async void OnNextMonthClicked(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            CatchesFrame.IsVisible = false;
            await LoadCalendarAsync();
        }
    }
}
