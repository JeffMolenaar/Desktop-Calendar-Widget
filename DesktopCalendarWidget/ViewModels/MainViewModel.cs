using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using DesktopCalendarWidget.Models;
using DesktopCalendarWidget.Services;

namespace DesktopCalendarWidget.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CalDavClient _calDavClient;
        private readonly SettingsService _settingsService;
        private readonly Timer _syncTimer;

        private DateTime _currentDate;
        private DateTime _lastSyncTime;
        private List<CalendarConfiguration> _calendars;
        private ObservableCollection<CalendarDayViewModel> _calendarDays;
        private ObservableCollection<CalendarEvent> _todaysEvents;

        public DateTime CurrentDate
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }

        public DateTime LastSyncTime
        {
            get => _lastSyncTime;
            set => SetProperty(ref _lastSyncTime, value);
        }

        public ObservableCollection<CalendarDayViewModel> CalendarDays
        {
            get => _calendarDays;
            set => SetProperty(ref _calendarDays, value);
        }

        public ObservableCollection<CalendarEvent> TodaysEvents
        {
            get => _todaysEvents;
            set => SetProperty(ref _todaysEvents, value);
        }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand ToggleVisibilityCommand { get; }
        public ICommand ExitCommand { get; }

        public MainViewModel()
        {
            _calDavClient = new CalDavClient();
            _settingsService = new SettingsService();
            _calendars = new List<CalendarConfiguration>();

            CurrentDate = DateTime.Today;
            CalendarDays = new ObservableCollection<CalendarDayViewModel>();
            TodaysEvents = new ObservableCollection<CalendarEvent>();

            // Commands
            PreviousMonthCommand = new RelayCommand(() => NavigateMonth(-1));
            NextMonthCommand = new RelayCommand(() => NavigateMonth(1));
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            RefreshCommand = new RelayCommand(async () => await RefreshCalendarsAsync());
            MinimizeCommand = new RelayCommand(MinimizeToTray);
            ToggleVisibilityCommand = new RelayCommand(ToggleVisibility);
            ExitCommand = new RelayCommand(ExitApplication);

            // Setup sync timer
            _syncTimer = new Timer();
            _syncTimer.Elapsed += async (s, e) => await RefreshCalendarsAsync();

            // Initialize
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadCalendarsAsync();
            GenerateCalendarDays();
            await RefreshCalendarsAsync();
            StartSyncTimer();
        }

        private async Task LoadCalendarsAsync()
        {
            _calendars = await _settingsService.LoadCalendarsAsync();
        }

        private void NavigateMonth(int direction)
        {
            CurrentDate = CurrentDate.AddMonths(direction);
            GenerateCalendarDays();
            _ = RefreshCalendarsAsync();
        }

        private void GenerateCalendarDays()
        {
            CalendarDays.Clear();

            var firstDayOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var startDate = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);

            for (int i = 0; i < 42; i++) // 6 weeks * 7 days
            {
                var date = startDate.AddDays(i);
                var isCurrentMonth = date.Month == CurrentDate.Month && date.Year == CurrentDate.Year;
                CalendarDays.Add(new CalendarDayViewModel(date, isCurrentMonth));
            }
        }

        private async Task RefreshCalendarsAsync()
        {
            try
            {
                var allEvents = new List<CalendarEvent>();

                foreach (var calendar in _calendars.Where(c => c.IsEnabled))
                {
                    var startDate = CalendarDays.First().Date;
                    var endDate = CalendarDays.Last().Date;
                    var events = await _calDavClient.GetEventsAsync(calendar, startDate, endDate);
                    allEvents.AddRange(events);
                }

                // Update calendar days with events
                foreach (var day in CalendarDays)
                {
                    day.Events = allEvents.Where(e => e.StartDate.Date == day.Date.Date).ToList();
                }

                // Update today's events
                TodaysEvents.Clear();
                var todaysEventsList = allEvents.Where(e => e.IsToday).OrderBy(e => e.StartDate).ToList();
                foreach (var evt in todaysEventsList)
                {
                    TodaysEvents.Add(evt);
                }

                LastSyncTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing calendars: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void StartSyncTimer()
        {
            if (_calendars.Any(c => c.IsEnabled))
            {
                var minInterval = _calendars.Where(c => c.IsEnabled).Min(c => c.SyncIntervalMinutes);
                _syncTimer.Interval = TimeSpan.FromMinutes(minInterval).TotalMilliseconds;
                _syncTimer.Start();
            }
        }

        private void ShowSettings()
        {
            var settingsWindow = new Views.SettingsWindow(_calendars);
            if (settingsWindow.ShowDialog() == true)
            {
                _calendars = settingsWindow.Calendars;
                _ = _settingsService.SaveCalendarsAsync(_calendars);
                _ = RefreshCalendarsAsync();
                StartSyncTimer();
            }
        }

        private void MinimizeToTray()
        {
            Application.Current.MainWindow.Hide();
        }

        private void ToggleVisibility()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow.IsVisible)
            {
                mainWindow.Hide();
            }
            else
            {
                mainWindow.Show();
                mainWindow.Activate();
            }
        }

        private void ExitApplication()
        {
            _syncTimer?.Stop();
            _calDavClient?.Dispose();
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();
    }
}