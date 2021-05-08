using LeagueBroadcast.ChampSelect.Data.Config;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.ViewModel;
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
    /// Interaction logic for PickBanView.xaml
    /// </summary>
    public partial class PickBanView : UserControl
    {
        ColorPickerWindow _openColorPicker;
        public PickBanView()
        {
            InitializeComponent();

            OpenContent.Width = 360;
            OpenContent.Opacity = 0;

            App.Instance.Exit += (s, e) => { if (_openColorPicker != null) { _openColorPicker.Close(); } };
        }
        private void TeamNameChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TeamConfig config = (string)textBox.Tag == "Blue" ? ConfigController.PickBan.frontend.blueTeam : ConfigController.PickBan.frontend.redTeam;
            if (textBox.Text != config.name)
            {
                config.name = textBox.Text;
                ConfigController.PickBan.UpdateFile();
            }
        }

        private void ScoreChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TeamConfig config = (string)textBox.Tag == "Blue" ? ConfigController.PickBan.frontend.blueTeam : ConfigController.PickBan.frontend.redTeam;
            if (textBox.Text != "" + config.score)
            {
                config.score = Int32.Parse(textBox.Text);
                ConfigController.PickBan.UpdateFile();
            }
        }
        private void CoachChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TeamConfig config = (string)textBox.Tag == "Blue" ? ConfigController.PickBan.frontend.blueTeam : ConfigController.PickBan.frontend.redTeam;
            if (textBox.Text != config.coach)
            {
                config.coach = textBox.Text;
                ConfigController.PickBan.UpdateFile();
            }
        }

        private void PatchChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            FrontendConfig config = ConfigController.PickBan.frontend;
            if (textBox.Text != config.patch)
            {
                config.patch = textBox.Text;
                ConfigController.PickBan.UpdateFile();
            }
        }

        private void BlueColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (_openColorPicker != null)
                _openColorPicker.Close();

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                _openColorPicker = new ColorPickerWindow(TeamConfigViewModel.BlueTeam);
                _openColorPicker.Owner = Window.GetWindow(this);
                _openColorPicker.Topmost = true;
                _openColorPicker.Show();
            });
        }

        private void RedColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (_openColorPicker != null)
                _openColorPicker.Close();
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                _openColorPicker = new ColorPickerWindow(TeamConfigViewModel.RedTeam);
                _openColorPicker.Owner = Window.GetWindow(this);
                _openColorPicker.Topmost = true;
                _openColorPicker.Show();
            });
        }

        private bool IsTextAccepted(TextBox sender, String text)
        {
            return int.TryParse(sender.Text + text, out int res) || (text == "0" && sender.Text.Length == 0);
        }

        private void PreviewTextInputHandler(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAccepted((TextBox)sender, e.Text);
        }

        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAccepted((TextBox)sender, text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }
    }
}
