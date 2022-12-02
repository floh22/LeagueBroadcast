using System.Windows;
using LeagueBroadcast.Update;
using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Client.MVVM.ViewModel.Startup;

namespace LeagueBroadcast.Client.MVVM.View.Startup
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

        private void UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            UpdateController.OnUpdateConfirmed();
        }

        private void UpdateSkip_Click(object sender, RoutedEventArgs e)
        {
            StringVersion skippedVersion = ((StartupViewModel)DataContext).UpdateVersion;
            ConfigController.Get<ClientConfig>().LastSkippedVersion = skippedVersion;
            ConfigController.WriteConfig<ClientConfig>();
            UpdateController.OnUpdateSkipped(skippedVersion);
        }

        private void UpdateContinue_Click(object sender, RoutedEventArgs e)
        {
            UpdateController.OnUpdateCanceled();
        }
    }
}
