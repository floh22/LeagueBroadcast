using LeagueBroadcastHub.Data;
using LeagueBroadcastHub.Data.Client;
using LeagueIngameServer;
using ModernWpf.Controls;
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

namespace LeagueBroadcastHub.Pages.ControlPages
{
    /// <summary>
    /// Interaction logic for ChampSelectPage.xaml
    /// </summary>
    public partial class ChampSelectPage : System.Windows.Controls.Page
    {
        public ChampSelectPage()
        {
            InitializeComponent();

            //ChampSelectSettingsViewModel.ChampSelectSettings.Init();
            ChampSelectTeamViewModel.BlueTeam.Init(BroadcastHubController.ClientConfig.frontend.blueTeam);
            ChampSelectTeamViewModel.RedTeam.Init(BroadcastHubController.ClientConfig.frontend.redTeam);
        }

        private void TeamNameChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TeamConfig config = (string)textBox.Tag == "Blue" ? BroadcastHubController.ClientConfig.frontend.blueTeam : BroadcastHubController.ClientConfig.frontend.redTeam;
            if(textBox.Text != config.name)
            {
                config.name = textBox.Text;
                BroadcastHubController.UpdateConfig();
            } 
        }

        private void ScoreChanged(ModernWpf.Controls.NumberBox sender, ModernWpf.Controls.NumberBoxValueChangedEventArgs args)
        {
            TeamConfig config = (string)sender.Tag == "Blue" ? BroadcastHubController.ClientConfig.frontend.blueTeam : BroadcastHubController.ClientConfig.frontend.redTeam;
            if(args.NewValue != config.score)
            {
                config.score = (int)args.NewValue;
                BroadcastHubController.UpdateConfig();
            }
        }
        private void CoachChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TeamConfig config = (string)textBox.Tag == "Blue" ? BroadcastHubController.ClientConfig.frontend.blueTeam : BroadcastHubController.ClientConfig.frontend.redTeam;
            if(textBox.Text != config.coach)
            {
                config.coach = textBox.Text;
                BroadcastHubController.UpdateConfig();
            }
        }
        private void ColorChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TeamConfig config = (string)textBox.Tag == "Blue" ? BroadcastHubController.ClientConfig.frontend.blueTeam : BroadcastHubController.ClientConfig.frontend.redTeam;
            if(textBox.Text != config.color)
            {
                config.color = textBox.Text;
                BroadcastHubController.UpdateConfig();
            }
        }

        private void PatchChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            FrontendConfig config = BroadcastHubController.ClientConfig.frontend;
            if (textBox.Text != config.patch)
            {
                config.patch = textBox.Text;
                BroadcastHubController.UpdateConfig();
            }
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
    }

    public class ChampSelectTeamViewModel : ViewModelBase
    {
        private string _name;
        private int _score;
        private string _coach;
        private string _color;
        private TeamConfig configReference;

        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        public int Score { get { return _score; } set { _score = value; OnPropertyChanged("Score"); } }
        public string Coach { get { return _coach; } set { _coach = value; OnPropertyChanged("Coach"); } }
        public string Color { get { return _color; } set { _color = value; OnPropertyChanged("Color"); } }

        public static ChampSelectTeamViewModel BlueTeam = new ChampSelectTeamViewModel();
        public static ChampSelectTeamViewModel RedTeam = new ChampSelectTeamViewModel();

        public ChampSelectTeamViewModel(TeamConfig config)
        {
            this.configReference = config;
        }

        public ChampSelectTeamViewModel()
        {

        }

        public void Init(TeamConfig config)
        {
            this.configReference = config;
            this.Name = configReference.name;
            this.Score = configReference.score;
            this.Coach = configReference.coach;
            this.Color = configReference.color;
        }
    }

    public class ChampSelectSettingsViewModel: ViewModelBase
    {

        public string Patch { get { return BroadcastHubController.ClientConfig.frontend.patch; } set { BroadcastHubController.ClientConfig.frontend.patch = value; OnPropertyChanged("Patch"); } }
        public bool Spells { get { return BroadcastHubController.ClientConfig.frontend.spellsEnabled; }  set { BroadcastHubController.ClientConfig.frontend.spellsEnabled = value; OnPropertyChanged("Spells"); } }
        public bool Coaches { get { return BroadcastHubController.ClientConfig.frontend.coachesEnabled; } set { BroadcastHubController.ClientConfig.frontend.coachesEnabled = value; OnPropertyChanged("Coaches"); } }
        public bool Score { get { return BroadcastHubController.ClientConfig.frontend.scoreEnabled; } set { BroadcastHubController.ClientConfig.frontend.scoreEnabled = value; OnPropertyChanged("Score"); } }

        public static ChampSelectSettingsViewModel ChampSelectSettings = new ChampSelectSettingsViewModel();

        public ChampSelectSettingsViewModel (string patch, bool spells, bool coaches, bool score)
        {
            this.Patch = patch;
            this.Score = score;
            this.Spells = spells;
            this.Coaches = coaches;
        }

        public ChampSelectSettingsViewModel()
        {

        }

        public void Init()
        {
            var frontend = BroadcastHubController.ClientConfig.frontend;
            this.Patch = frontend.patch;
            this.Spells = frontend.spellsEnabled;
            this.Coaches = frontend.coachesEnabled;
            this.Score = frontend.scoreEnabled;
        }
    }

}
