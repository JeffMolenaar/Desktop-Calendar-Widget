using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopCalendarWidget.Models;
using DesktopCalendarWidget.Services;

namespace DesktopCalendarWidget.Views
{
    public partial class SettingsWindow : Window
    {
        public List<CalendarConfiguration> Calendars { get; private set; }

        public SettingsWindow(List<CalendarConfiguration> calendars)
        {
            InitializeComponent();
            Calendars = calendars.ToList(); // Create a copy
            DataContext = new SettingsViewModel(Calendars, this);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm && vm.SelectedCalendar != null)
            {
                vm.SelectedCalendar.Password = ((PasswordBox)sender).Password;
            }
        }
    }

    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly SettingsWindow _window;
        private readonly CalDavClient _calDavClient;
        private CalendarConfiguration _selectedCalendar;

        public ObservableCollection<CalendarConfiguration> Calendars { get; }

        public CalendarConfiguration SelectedCalendar
        {
            get => _selectedCalendar;
            set => SetProperty(ref _selectedCalendar, value);
        }

        public ICommand AddCalendarCommand { get; }
        public ICommand RemoveCalendarCommand { get; }
        public ICommand TestConnectionCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public SettingsViewModel(List<CalendarConfiguration> calendars, SettingsWindow window)
        {
            _window = window;
            _calDavClient = new CalDavClient();
            
            Calendars = new ObservableCollection<CalendarConfiguration>(calendars);
            
            AddCalendarCommand = new RelayCommand(AddCalendar);
            RemoveCalendarCommand = new RelayCommand(RemoveCalendar, CanRemoveCalendar);
            TestConnectionCommand = new RelayCommand(async () => await TestConnectionAsync(), CanTestConnection);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void AddCalendar()
        {
            var newCalendar = new CalendarConfiguration
            {
                Name = $"Calendar {Calendars.Count + 1}",
                Color = GetNextColor()
            };
            
            Calendars.Add(newCalendar);
            SelectedCalendar = newCalendar;
        }

        private string GetNextColor()
        {
            var colors = new[]
            {
                "#2196F3", "#4CAF50", "#FF9800", "#9C27B0", "#F44336",
                "#00BCD4", "#FFEB3B", "#E91E63", "#8BC34A", "#3F51B5"
            };
            
            var usedColors = Calendars.Select(c => c.Color).ToHashSet();
            return colors.FirstOrDefault(c => !usedColors.Contains(c)) ?? colors[0];
        }

        private void RemoveCalendar()
        {
            if (SelectedCalendar != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to remove the calendar '{SelectedCalendar.Name}'?",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Calendars.Remove(SelectedCalendar);
                    SelectedCalendar = Calendars.FirstOrDefault();
                }
            }
        }

        private bool CanRemoveCalendar() => SelectedCalendar != null;

        private async System.Threading.Tasks.Task TestConnectionAsync()
        {
            if (SelectedCalendar == null) return;

            if (string.IsNullOrEmpty(SelectedCalendar.Url) || 
                string.IsNullOrEmpty(SelectedCalendar.Username) || 
                string.IsNullOrEmpty(SelectedCalendar.Password))
            {
                MessageBox.Show("Please fill in URL, username, and password.", "Missing Information", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var success = await _calDavClient.TestConnectionAsync(SelectedCalendar);
                
                if (success)
                {
                    MessageBox.Show("Connection successful!", "Test Connection", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Connection failed. Please check your settings.", "Test Connection", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection test failed: {ex.Message}", "Test Connection", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanTestConnection() => SelectedCalendar != null;

        private void Save()
        {
            _window.Calendars = Calendars.ToList();
            _window.DialogResult = true;
            _window.Close();
        }

        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();
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