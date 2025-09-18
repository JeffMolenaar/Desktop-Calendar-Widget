using System;
using System.Windows;
using System.Windows.Threading;

namespace DesktopCalendarWidget
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up global exception handling
            Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // Create and show the main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An error occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"A critical error occurred: {((Exception)e.ExceptionObject).Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}