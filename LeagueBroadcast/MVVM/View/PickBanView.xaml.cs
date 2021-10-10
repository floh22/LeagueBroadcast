using LeagueBroadcast.ChampSelect.Data.Config;
using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.MVVM.ViewModel;
using LeagueBroadcast.OperatingSystem;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace LeagueBroadcast.MVVM.View
{
    /// <summary>
    /// Interaction logic for PickBanView.xaml
    /// </summary>
    public partial class PickBanView : UserControl
    {
        ColorPickerWindow _openColorPicker;

        private static string teamIconFolder = Path.Join(Path.Join(Directory.GetCurrentDirectory(), "Cache"), "TeamIcons");
        public PickBanView()
        {
            InitializeComponent();

            OpenContent.Width = 360;
            OpenContent.Opacity = 0;

            if (JSONConfigProvider.Instance.TeamConfigs.Contains( TeamConfigViewModel.BlueTeam.Name))
            {
                OrderSelector.SelectedItem = TeamConfigViewModel.BlueTeam.Name;
            }

            if (JSONConfigProvider.Instance.TeamConfigs.Contains(TeamConfigViewModel.RedTeam.Name))
            {
                ChaosSelector.SelectedItem = TeamConfigViewModel.RedTeam.Name;
            }

            Directory.CreateDirectory(teamIconFolder);

            if(!File.Exists(Path.Join(teamIconFolder, "Default.png"))){
                File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Images", "LeagueOfLegendsIcon.png"), Path.Join(teamIconFolder, "Default.png"));
            }

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
            int value;
            bool isInt = Int32.TryParse(textBox.Text, out value);
            TeamConfig config = (string)textBox.Tag == "Blue" ? ConfigController.PickBan.frontend.blueTeam : ConfigController.PickBan.frontend.redTeam;
            if (textBox.Text != "" + config.score && isInt)
            {
                config.score = value;
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
        private void TeamTagChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TeamConfig config = (string)textBox.Tag == "Blue" ? ConfigController.PickBan.frontend.blueTeam : ConfigController.PickBan.frontend.redTeam;
            if (textBox.Text != config.nameTag)
            {
                config.nameTag = textBox.Text;
                ConfigController.PickBan.UpdateFile();
            }
        }

        private void BlueColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (_openColorPicker != null)
                _openColorPicker.Close();

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                _openColorPicker = new ColorPickerWindow(TeamConfigViewModel.BlueTeam, "blue");
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
                _openColorPicker = new ColorPickerWindow(TeamConfigViewModel.RedTeam, "red");
                _openColorPicker.Owner = Window.GetWindow(this);
                _openColorPicker.Topmost = true;
                _openColorPicker.Show();
            });
        }

        private void BlueDefaultColor_Click(object sender, RoutedEventArgs e)
        {
            if (_openColorPicker != null)
                _openColorPicker.Close();
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                _openColorPicker = new ColorPickerWindow((PickBanConfigViewModel)((Button)sender).DataContext, "blue");
                _openColorPicker.Owner = Window.GetWindow(this);
                _openColorPicker.Topmost = true;
                _openColorPicker.Show();
            });
        }

        private void RedDefaultColor_Click(object sender, RoutedEventArgs e)
        {
            if (_openColorPicker != null)
                _openColorPicker.Close();
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                _openColorPicker = new ColorPickerWindow((PickBanConfigViewModel)((Button)sender).DataContext, "red");
                _openColorPicker.Owner = Window.GetWindow(this);
                _openColorPicker.Topmost = true;
                _openColorPicker.Show();
            });
        }

        private void BlueAddTeam_Click(object sender, RoutedEventArgs e)
        {
            var currentConfig = TeamConfigViewModel.BlueTeam;
            var cfg = JSONConfigProvider.Instance.TeamConfigs.SingleOrDefault(path => path.Equals(currentConfig.Name));

            var toAdd = new ExtendedTeamConfig(currentConfig.Name)
            {
                Config = currentConfig.ConfigReference,
                IconLocation = currentConfig.IconName
            };

            if (cfg != "" && cfg != null)
            {
                //Overwrite old team config
                Log.Warn("Overwriting existing team config");
                JSONConfigProvider.Instance.WriteTeam(toAdd);
            }
            else
            {
                JSONConfigProvider.Instance.WriteTeam(toAdd);
                JSONConfigProvider.Instance.TeamConfigs.Add(currentConfig.Name);
            }

            OrderSelector.SelectedItem = currentConfig.Name;
        }

        private void BlueRemoveTeam_Click(object sender, RoutedEventArgs e)
        {
            var currentConfig = TeamConfigViewModel.BlueTeam;
            var cfg = JSONConfigProvider.Instance.TeamConfigs.SingleOrDefault(path => path.Equals(currentConfig.Name));
            if (cfg != "" && cfg != null)
            {
                JSONConfigProvider.Instance.TeamConfigs.Remove(currentConfig.Name);
            }
        }

        private async void BlueSetLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Images (*.png;*.jpeg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                TeamConfigViewModel.BlueTeam.IconName = TeamConfigViewModel.DefaultIconPath;
                await Task.Delay(100);

                //Remove old icon file if it exists
                FileInfo file = new(openFileDialog.FileName);
                string newFileName = Path.Join(teamIconFolder, TeamConfigViewModel.BlueTeam.Name + file.Extension);
                if (File.Exists(newFileName))
                {
                    Log.Warn($"Overwriting old team Logo for team {TeamConfigViewModel.BlueTeam.Name}");
                    try
                    {
                        File.Delete(newFileName);
                    } catch (IOException)
                    {
                        Log.Warn("Could not overwrite old icon");
                        return;
                    }
                }

                File.Copy(openFileDialog.FileName, newFileName, true);

                TeamConfigViewModel.BlueTeam.IconName = Path.Combine("Cache", "TeamIcons", TeamConfigViewModel.BlueTeam.Name + file.Extension);
            }
        }

        private void RedAddTeam_Click(object sender, RoutedEventArgs e)
        {
            var currentConfig = TeamConfigViewModel.RedTeam;
            var cfg = JSONConfigProvider.Instance.TeamConfigs.SingleOrDefault(path => path.Equals(currentConfig.Name));

            var toAdd = new ExtendedTeamConfig(currentConfig.Name)
            {
                Config = currentConfig.ConfigReference,
                IconLocation = currentConfig.IconName
            };

            if (cfg != "" && cfg != null)
            {
                //Overwrite old team config
                Log.Warn("Overwriting existing team config");
                JSONConfigProvider.Instance.WriteTeam(toAdd);
            } else
            {
                JSONConfigProvider.Instance.WriteTeam(toAdd);
                JSONConfigProvider.Instance.TeamConfigs.Add(currentConfig.Name);
            }

            ChaosSelector.SelectedItem = currentConfig.Name;
        }

        private void RedRemoveTeam_Click(object sender, RoutedEventArgs e)
        {
            var currentConfig = TeamConfigViewModel.BlueTeam;
            var cfg = JSONConfigProvider.Instance.TeamConfigs.SingleOrDefault(path => path.Equals(currentConfig.Name));
            if (cfg != "" && cfg != null)
            {
                JSONConfigProvider.Instance.TeamConfigs.Remove(currentConfig.Name);
            }
        }

        private async void RedSetLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Images (*.png;*.jpeg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                TeamConfigViewModel.RedTeam.IconName = TeamConfigViewModel.DefaultIconPath;
                await Task.Delay(100);

                //Remove old icon file if it exists
                FileInfo file = new(openFileDialog.FileName);
                string newFileName = Path.Join(teamIconFolder, TeamConfigViewModel.RedTeam.Name + file.Extension);
                if (File.Exists(newFileName))
                {
                    Log.Warn($"Overwriting old team Logo for team {TeamConfigViewModel.RedTeam.Name}");
                    try
                    {
                        File.Delete(newFileName);
                    }
                    catch (IOException)
                    {
                        Log.Warn("Could not overwrite old icon");
                        return;
                    }
                }

                File.Copy(openFileDialog.FileName, newFileName, true);

                TeamConfigViewModel.RedTeam.IconName = Path.Combine("Cache", "TeamIcons", TeamConfigViewModel.RedTeam.Name + file.Extension);
            }
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

        private void OrderSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged(TeamConfigViewModel.BlueTeam, (string)OrderSelector.SelectedItem);
        }

       

        private void ChaosSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged(TeamConfigViewModel.RedTeam, (string)ChaosSelector.SelectedItem);
        }

        private void SelectionChanged(TeamConfigViewModel currentConfig, string selectedTeam)
        {
            if (selectedTeam != currentConfig.Name)
            {
                var toLoad = new ExtendedTeamConfig(selectedTeam);
                var res = JSONConfigProvider.Instance.ReadTeam(toLoad);
                if (!res)
                {
                    Log.Warn("Could not load team info from disk");
                    return;
                }
                currentConfig.Name = toLoad.Config.name;
                currentConfig.NameTag = toLoad.Config.nameTag;
                currentConfig.Score = 0;
                currentConfig.Coach = toLoad.Config.coach;
                currentConfig.IconName = toLoad.IconLocation;

                BroadcastController.Instance.IGController.gameState.UpdateTeamColors();
            }
        }

        private void ResetOrderIcon_Click(object sender, RoutedEventArgs e)
        {
            TeamConfigViewModel.BlueTeam.IconName = TeamConfigViewModel.DefaultIconPath;
        }

        private void ResetChaosIcon_Click(object sender, RoutedEventArgs e)
        {
            TeamConfigViewModel.RedTeam.IconName = TeamConfigViewModel.DefaultIconPath;
        }

        private void SwapButton_Click(object sender, RoutedEventArgs e)
        {
            TeamConfigViewModel temp = new()
            {
                Name = TeamConfigViewModel.BlueTeam.Name,
                NameTag = TeamConfigViewModel.BlueTeam.NameTag,
                Coach = TeamConfigViewModel.BlueTeam.Coach,
                IconName = TeamConfigViewModel.BlueTeam.IconName,
                Score = TeamConfigViewModel.BlueTeam.Score
            };

            TeamConfigViewModel.BlueTeam.Name = TeamConfigViewModel.RedTeam.Name;
            TeamConfigViewModel.BlueTeam.NameTag = TeamConfigViewModel.RedTeam.NameTag;
            TeamConfigViewModel.BlueTeam.Coach = TeamConfigViewModel.RedTeam.Coach;
            TeamConfigViewModel.BlueTeam.IconName = TeamConfigViewModel.RedTeam.IconName;
            TeamConfigViewModel.BlueTeam.Score = TeamConfigViewModel.RedTeam.Score;

            TeamConfigViewModel.RedTeam.Name = temp.Name;
            TeamConfigViewModel.RedTeam.NameTag = temp.NameTag;
            TeamConfigViewModel.RedTeam.Coach = temp.Coach;
            TeamConfigViewModel.RedTeam.IconName = temp.IconName;
            TeamConfigViewModel.RedTeam.Score = temp.Score;

            OrderSelector.SelectedItem = TeamConfigViewModel.BlueTeam.Name;
            ChaosSelector.SelectedItem = TeamConfigViewModel.RedTeam.Name;

            BroadcastController.Instance.IGController.gameState.UpdateTeamColors();
        }
    }
}
