using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopCalendarWidget.Models
{
    public class CalendarConfiguration : INotifyPropertyChanged
    {
        private string _name;
        private string _url;
        private string _username;
        private string _password;
        private string _color;
        private int _syncIntervalMinutes;
        private bool _isEnabled;
        private DateTime _lastSync;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Url
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public int SyncIntervalMinutes
        {
            get => _syncIntervalMinutes;
            set => SetProperty(ref _syncIntervalMinutes, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public DateTime LastSync
        {
            get => _lastSync;
            set => SetProperty(ref _lastSync, value);
        }

        public CalendarConfiguration()
        {
            Name = "New Calendar";
            Color = "#2196F3"; // Default blue
            SyncIntervalMinutes = 15;
            IsEnabled = true;
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
}