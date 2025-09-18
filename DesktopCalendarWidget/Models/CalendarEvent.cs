using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopCalendarWidget.Models
{
    public class CalendarEvent : INotifyPropertyChanged
    {
        private string _id;
        private string _title;
        private string _description;
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _isAllDay;
        private string _location;
        private string _calendarColor;
        private string _calendarName;

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public bool IsAllDay
        {
            get => _isAllDay;
            set => SetProperty(ref _isAllDay, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public string CalendarColor
        {
            get => _calendarColor;
            set => SetProperty(ref _calendarColor, value);
        }

        public string CalendarName
        {
            get => _calendarName;
            set => SetProperty(ref _calendarName, value);
        }

        public TimeSpan Duration => EndDate - StartDate;

        public bool IsToday => StartDate.Date == DateTime.Today;

        public bool IsThisWeek
        {
            get
            {
                var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7);
                return StartDate.Date >= startOfWeek && StartDate.Date < endOfWeek;
            }
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