using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DesktopCalendarWidget.Models;

namespace DesktopCalendarWidget.ViewModels
{
    public class CalendarDayViewModel : INotifyPropertyChanged
    {
        private DateTime _date;
        private bool _isCurrentMonth;
        private bool _isToday;
        private List<CalendarEvent> _events;

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public int Day => Date.Day;

        public bool IsCurrentMonth
        {
            get => _isCurrentMonth;
            set => SetProperty(ref _isCurrentMonth, value);
        }

        public bool IsToday
        {
            get => _isToday;
            set => SetProperty(ref _isToday, value);
        }

        public List<CalendarEvent> Events
        {
            get => _events ?? new List<CalendarEvent>();
            set => SetProperty(ref _events, value);
        }

        public CalendarDayViewModel(DateTime date, bool isCurrentMonth)
        {
            Date = date;
            IsCurrentMonth = isCurrentMonth;
            IsToday = date.Date == DateTime.Today;
            Events = new List<CalendarEvent>();
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