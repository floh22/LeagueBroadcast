using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace LeagueBroadcast.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LocationChanged += new EventHandler(Window_LocationChanged);
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            foreach (Window win in this.OwnedWindows)
            {
                win.Top = (this.Top + this.Height / 2) - win.Height / 2;
                win.Left = (this.Left + this.Width / 2) - win.Width / 2;
            }
        }

        public void SetHomeSelected()
        {
            HomeButton.IsChecked = true;
        }

        public void SetPickBanSelected()
        {
            PBButton.IsChecked = true;
        }

        public void SetIngameSelected()
        {
            IGButton.IsChecked = true;
        }

        public void SetPostGameSelected()
        {
            PGButton.IsChecked = true;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (Application.Current.MainWindow == null)
                return;
            Application.Current.MainWindow.Height = 720;
            Application.Current.MainWindow.Width = 1280;
        }
    }
}
