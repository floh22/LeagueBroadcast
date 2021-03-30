using LeagueBroadcastHub.Data.Provider;
using LeagueBroadcastHub.Log;
using LeagueBroadcastHub.Pages;
using LeagueBroadcastHub.Session;
using LeagueIngameServer;
using ModernWpf.Controls;
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
    public partial class SettingsPage : System.Windows.Controls.Page
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
            delayValue.Value = ActiveSettings._delayPickBanValue;
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = Keyboard.FocusedElement as TextBox;
            NumberBox numberBox = Keyboard.FocusedElement as NumberBox;
            if (textBox != null || numberBox != null)
            {
                Keyboard.ClearFocus();
            }
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

        private void PickBanToggled(object sender, RoutedEventArgs e)
        {
            if (ActiveSettings._usePickBan == StateController.CurrentSettings.PickBan)
                return;
            if (ActiveSettings._usePickBan)
                StateController.EnableChampSelect();
            else 
                StateController.DisableChampSelect();
        }

        private void IngameToggled(object sender, RoutedEventArgs e)
        {
            if (ActiveSettings._useIngame == StateController.CurrentSettings.Ingame)
                return;
            if (ActiveSettings._useIngame)
                StateController.EnableIngame();
            else
                StateController.DisableIngame();
        }
        private void DelayToggled(object sender, RoutedEventArgs e)
        {
            if (ActiveSettings._delayPickBan)
                BroadcastHubController.Instance.champSelectConnector.UseDelayPickBan();
            else
                BroadcastHubController.Instance.champSelectConnector.UseDirectPickBan();
        }

        private void delayValue_ValueChanged(ModernWpf.Controls.NumberBox sender, ModernWpf.Controls.NumberBoxValueChangedEventArgs args)
        {
            if(args.NewValue != ActiveSettings._delayPickBanValue)
            {
                Logging.Verbose($"Champ select delay changed to {args.NewValue}");
                ActiveSettings._delayPickBanValue = args.NewValue;
                Properties.Settings.Default.DelayPBValue = args.NewValue;
            }
        }
    }
}
