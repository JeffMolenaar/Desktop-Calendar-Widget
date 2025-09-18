# Desktop Calendar Widget

A modern, always-on-top desktop calendar widget with CalDAV support for multiple calendars, specifically designed to work with Nextcloud and other CalDAV servers.

## Features

- **Modern Material Design UI** - Clean, modern interface that stays on top of other applications
- **Multiple CalDAV Calendar Support** - Add multiple CalDAV calendars from different sources
- **Color-Coded Calendars** - Each calendar has its own color for easy identification
- **Nextcloud Integration** - Specifically designed to work seamlessly with Nextcloud CalDAV
- **Configurable Sync Intervals** - Set different sync intervals for each calendar (5-120 minutes)
- **System Tray Integration** - Minimizes to system tray when not needed
- **Background Widget Mode** - Always stays in the background, never interferes with other applications
- **Today's Events View** - Special section showing today's events with time and location
- **Drag & Drop Positioning** - Move the widget anywhere on your desktop

## Screenshots

![Calendar Widget](screenshot-widget.png)
*Main calendar widget showing events from multiple calendars*

![Settings Window](screenshot-settings.png)
*Settings window for configuring CalDAV calendars*

## Requirements

- **Windows 10/11** (64-bit)
- **.NET 6.0 Runtime** (Windows Desktop Apps)
- **Internet Connection** for CalDAV synchronization
- **Visual Studio 2022** (for compilation)

## Installation

### Pre-built Release (Recommended)
1. Download the latest release from the [Releases](../../releases) page
2. Extract the ZIP file to your desired location
3. Run `DesktopCalendarWidget.exe`

### Building from Source
1. **Clone the repository:**
   ```bash
   git clone https://github.com/JeffMolenaar/Desktop-Calendar-Widget.git
   cd Desktop-Calendar-Widget
   ```

2. **Open in Visual Studio 2022:**
   - Open `DesktopCalendarWidget.sln`
   - Ensure you have the .NET 6.0 Windows Desktop workload installed

3. **Build the solution:**
   - Select `Release` configuration
   - Build → Build Solution (Ctrl+Shift+B)

4. **Run the application:**
   - Set `DesktopCalendarWidget` as startup project
   - Press F5 or Debug → Start Debugging

## Configuration

### Adding a CalDAV Calendar

1. **Right-click the widget** or click the settings icon
2. **Select "Settings"** from the context menu
3. **Click "Add"** to create a new calendar
4. **Configure the calendar:**
   - **Name:** Give your calendar a meaningful name
   - **CalDAV URL:** Enter your CalDAV server URL
   - **Username:** Your CalDAV username
   - **Password:** Your CalDAV password
   - **Color:** Choose a color for this calendar's events
   - **Sync Interval:** How often to sync (5-120 minutes)
   - **Enabled:** Check to enable syncing

### Nextcloud Setup

For **Nextcloud** calendars, use this URL format:
```
https://your-nextcloud-domain/remote.php/dav/calendars/USERNAME/CALENDAR_NAME/
```

**Example:**
```
https://cloud.example.com/remote.php/dav/calendars/john/personal/
```

**To find your calendar URL in Nextcloud:**
1. Go to Calendar app in Nextcloud
2. Click on the three dots (⋯) next to your calendar name
3. Select "Edit"
4. Copy the CalDAV URL shown

### Other CalDAV Servers

The widget supports any CalDAV-compliant server:
- **Google Calendar:** Use Google's CalDAV URL with app-specific password
- **Apple iCloud:** Use iCloud CalDAV URL with app-specific password
- **Outlook/Office 365:** Use Outlook's CalDAV endpoint
- **Custom Servers:** Any RFC 4791 compliant CalDAV server

## Usage

### Widget Controls
- **Drag the header** to move the widget around your desktop
- **Minimize button** (−) hides the widget to system tray
- **Settings button** (⚙) opens the configuration window
- **Navigation arrows** (◀ ▶) change months
- **Refresh button** (↻) manually syncs all calendars

### System Tray
- **Left-click tray icon** to show/hide the widget
- **Right-click tray icon** for context menu:
  - Settings
  - Refresh
  - Exit

### Calendar View
- **Month View** showing 6 weeks with color-coded events
- **Today's Events** expandable section with detailed event info
- **Event Tooltips** hover over events for full title
- **Current Day Highlighting** today's date is highlighted

## Troubleshooting

### Connection Issues
1. **Test Connection** button in settings to verify CalDAV setup
2. **Check URLs** - ensure CalDAV URL is correct and accessible
3. **Verify Credentials** - username and password must be correct
4. **Network Connectivity** - ensure you can reach the CalDAV server
5. **Firewall/Proxy** - check if firewall or proxy is blocking connections

### Sync Issues
1. **Manual Refresh** - use the refresh button to force sync
2. **Check Sync Intervals** - very short intervals may cause issues
3. **Server Logs** - check your CalDAV server logs for errors
4. **Event Logs** - check Windows Event Viewer for application errors

### Performance
- **Reduce Sync Frequency** if the widget feels slow
- **Limit Number of Calendars** for better performance
- **Check Available Memory** if syncing large calendars

## Development

### Project Structure
```
DesktopCalendarWidget/
├── Models/                 # Data models (CalendarConfiguration, CalendarEvent)
├── Services/              # Business logic (CalDavClient, SettingsService)
├── ViewModels/           # MVVM view models
├── Views/                # Additional windows (SettingsWindow)
├── Converters/           # WPF value converters
├── MainWindow.xaml       # Main widget UI
└── App.xaml              # Application entry point
```

### Key Components
- **CalDavClient** - Handles CalDAV protocol communication
- **SettingsService** - Manages calendar configuration persistence
- **MainViewModel** - Main widget logic and data binding
- **MaterialDesign** - Modern UI framework for WPF

### Contributing
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly on Windows
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Credits

- **Material Design in XAML** - Modern UI components
- **Hardcodet NotifyIcon** - System tray functionality
- **Newtonsoft.Json** - Configuration serialization

## Support

For issues, questions, or feature requests, please:
1. Check the [Issues](../../issues) page
2. Create a new issue with detailed information
3. Include Windows version, .NET version, and error messages

---

**Note:** This application is designed for Windows desktop environments and requires .NET 6.0 Windows Desktop Runtime.