using LeagueBroadcastHub.Log;
using LeagueBroadcastHub.Pages;
using LeagueIngameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeagueBroadcastHub
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {

        public SettingsPage()
        {
            InitializeComponent();
            this.DataContext = this;

            switch (ActiveSettings.current.AppMode)
            {
                case 0:
                    modeLocal.IsChecked = true;
                    break;
                case 1:
                    modeWeb.IsChecked = true;
                    break;
                case 2:
                    modeWeb.IsChecked = true;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Invalid app mode in settings. Falling back to local");
                    Properties.Settings.Default.appMode = 0;
                    modeLocal.IsChecked = true;
                    break;
            }

            LoggingSelection.SelectedIndex = (int)((Logging.LogLevel)Enum.Parse(typeof(Logging.LogLevel), Properties.Settings.Default.LogLevel));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RemoteConnect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void LoggingLevelChanged(object sender, SelectionChangedEventArgs e)
        {
            Logging.LogLevel level = (Logging.LogLevel)Enum.Parse(typeof(Logging.LogLevel), ((ComboBoxItem)LoggingSelection.SelectedItem).Tag.ToString());
            Logging.SetLogLevel(level);
            if(level != (Logging.LogLevel)Enum.Parse(typeof(Logging.LogLevel), Properties.Settings.Default.LogLevel))
            {
                Logging.Write($"Log Level set to {level.ToString()}");
                Properties.Settings.Default.LogLevel = level.ToString();
            }
        }

    }
}
