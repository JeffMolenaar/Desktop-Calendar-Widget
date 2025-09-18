using System;
using System.Windows;
using System.Windows.Input;
using DesktopCalendarWidget.ViewModels;

namespace DesktopCalendarWidget
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            
            // Position window in bottom-right corner
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = SystemParameters.PrimaryScreenWidth - Width - 20;
            Top = SystemParameters.PrimaryScreenHeight - Height - 60;
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            base.OnStateChanged(e);
        }
    }
}