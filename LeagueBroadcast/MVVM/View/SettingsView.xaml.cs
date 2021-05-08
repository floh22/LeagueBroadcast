using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeagueBroadcast.MVVM.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            LogLevelSelector.SelectedIndex = (int)(ConfigController.Component.App.LogLevel);

        }

        private void LogLevelSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Log.LogLevel level = (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), ((ComboBoxItem)LogLevelSelector.SelectedItem).Tag.ToString());
            Log.SetLogLevel(level);
            if (level != ConfigController.Component.App.LogLevel)
            {
                Log.Write($"Log Level set to {level.ToString()}");
                ConfigController.Component.App.LogLevel = level;
            }
        }
    }
}
