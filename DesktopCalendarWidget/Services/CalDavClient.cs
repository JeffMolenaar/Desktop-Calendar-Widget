using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DesktopCalendarWidget.Models;

namespace DesktopCalendarWidget.Services
{
    public class CalDavClient
    {
        private readonly HttpClient _httpClient;

        public CalDavClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> TestConnectionAsync(CalendarConfiguration config)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Options, config.Url);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}")));

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CalendarEvent>> GetEventsAsync(CalendarConfiguration config, DateTime startDate, DateTime endDate)
        {
            var events = new List<CalendarEvent>();

            try
            {
                var calendarData = await GetCalendarDataAsync(config, startDate, endDate);
                events = ParseCalendarData(calendarData, config);
            }
            catch (Exception ex)
            {
                // Log error (in real implementation, use proper logging)
                System.Diagnostics.Debug.WriteLine($"Error getting events: {ex.Message}");
            }

            return events;
        }

        private async Task<string> GetCalendarDataAsync(CalendarConfiguration config, DateTime startDate, DateTime endDate)
        {
            var requestBody = CreateCalendarQuery(startDate, endDate);

            var request = new HttpRequestMessage(new HttpMethod("REPORT"), config.Url)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/xml")
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}")));
            request.Headers.Add("Depth", "1");

            var response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        private string CreateCalendarQuery(DateTime startDate, DateTime endDate)
        {
            return $@"<?xml version='1.0' encoding='utf-8' ?>
<c:calendar-query xmlns:d='DAV:' xmlns:c='urn:ietf:params:xml:ns:caldav'>
    <d:prop>
        <d:getetag />
        <c:calendar-data />
    </d:prop>
    <c:filter>
        <c:comp-filter name='VCALENDAR'>
            <c:comp-filter name='VEVENT'>
                <c:time-range start='{startDate:yyyyMMddTHHmmssZ}' end='{endDate:yyyyMMddTHHmmssZ}'/>
            </c:comp-filter>
        </c:comp-filter>
    </c:filter>
</c:calendar-query>";
        }

        private List<CalendarEvent> ParseCalendarData(string calendarData, CalendarConfiguration config)
        {
            var events = new List<CalendarEvent>();

            try
            {
                // Simple iCal parsing - in real implementation, use a proper iCal library
                var lines = calendarData.Split('\n');
                CalendarEvent currentEvent = null;

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();

                    if (line.StartsWith("BEGIN:VEVENT"))
                    {
                        currentEvent = new CalendarEvent
                        {
                            CalendarColor = config.Color,
                            CalendarName = config.Name
                        };
                    }
                    else if (line.StartsWith("END:VEVENT") && currentEvent != null)
                    {
                        events.Add(currentEvent);
                        currentEvent = null;
                    }
                    else if (currentEvent != null)
                    {
                        if (line.StartsWith("SUMMARY:"))
                            currentEvent.Title = line.Substring(8);
                        else if (line.StartsWith("DESCRIPTION:"))
                            currentEvent.Description = line.Substring(12);
                        else if (line.StartsWith("DTSTART"))
                            currentEvent.StartDate = ParseDateTime(line);
                        else if (line.StartsWith("DTEND"))
                            currentEvent.EndDate = ParseDateTime(line);
                        else if (line.StartsWith("LOCATION:"))
                            currentEvent.Location = line.Substring(9);
                        else if (line.StartsWith("UID:"))
                            currentEvent.Id = line.Substring(4);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing calendar data: {ex.Message}");
            }

            return events;
        }

        private DateTime ParseDateTime(string line)
        {
            try
            {
                var dateStr = line.Split(':')[1];
                if (dateStr.Contains("T"))
                {
                    return DateTime.ParseExact(dateStr.Replace("Z", ""), "yyyyMMddTHHmmss", null);
                }
                else
                {
                    return DateTime.ParseExact(dateStr, "yyyyMMdd", null);
                }
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}