using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DesktopCalendarWidget.Models;

namespace DesktopCalendarWidget.Services
{
    public class SettingsService
    {
        private const string SettingsFileName = "calendars.json";
        private readonly string _settingsPath;

        public SettingsService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "DesktopCalendarWidget");
            Directory.CreateDirectory(appFolder);
            _settingsPath = Path.Combine(appFolder, SettingsFileName);
        }

        public async Task<List<CalendarConfiguration>> LoadCalendarsAsync()
        {
            try
            {
                if (!File.Exists(_settingsPath))
                {
                    return new List<CalendarConfiguration>();
                }

                var json = await File.ReadAllTextAsync(_settingsPath);
                return JsonConvert.DeserializeObject<List<CalendarConfiguration>>(json) ?? new List<CalendarConfiguration>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading calendars: {ex.Message}");
                return new List<CalendarConfiguration>();
            }
        }

        public async Task SaveCalendarsAsync(List<CalendarConfiguration> calendars)
        {
            try
            {
                var json = JsonConvert.SerializeObject(calendars, Formatting.Indented);
                await File.WriteAllTextAsync(_settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving calendars: {ex.Message}");
            }
        }
    }
}