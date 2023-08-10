using LeagueBroadcast.MVVM.ViewModel;
using System;
using System.Windows;

namespace LeagueBroadcast.MVVM.View
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        public StartupViewModel GETDataContext()
        {
            return (StartupViewModel)DataContext;
        }

        private void UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            StartupViewModel ctx = (StartupViewModel)this.DataContext;
            ctx.Update?.Invoke(null, EventArgs.Empty);

        }

        private void UpdateSkip_Click(object sender, RoutedEventArgs e)
        {
            StartupViewModel ctx = (StartupViewModel)this.DataContext;
            ctx.SkipUpdate?.Invoke(null, EventArgs.Empty);
        }

        private void Button_Open_Discord_Click(object sender, RoutedEventArgs e)
        {
            //TODO update when https is available
            string destinationUrl = "http://discord.lolfar.site";

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = destinationUrl,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
    }
}
