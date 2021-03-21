using LeagueBroadcastHub.Session;
using LeagueIngameServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LeagueBroadcastHub.Pages.ControlPages
{
    /// <summary>
    /// Interaction logic for ControlsPage.xaml
    /// </summary>
    public partial class ControlsPage : Page
    {

        private static List<ToggleButton> buttons;

        public ControlsPage()
        {
            InitializeComponent();
            this.DataContext = this;
            // Get data objects and place them into an ObservableCollection

            /*
            TestButton.Checked += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Active Button");
            };

            TestButton.Unchecked += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Off Button");
            };
            */

            var events = ControlViewModel.GetEvents();
            var autoEvents = ControlViewModel.GetAutomaticEvents();
            buttons = new List<ToggleButton>();

            EventsRow1.ItemsSource = new ObservableCollection<ControlViewModel>(events);
            AutoEvents1.ItemsSource = new ObservableCollection<ControlViewModel>(autoEvents.GetRange(0, 3));
            AutoEvents2.ItemsSource = new ObservableCollection<ControlViewModel>(autoEvents.GetRange(3, 3));
        }

        public void OnLoad(object sender, RoutedEventArgs e)
        {
            buttons = EventsRow1.Children.OfType<ToggleButton>().Concat(AutoEvents1.Children.OfType<ToggleButton>()).Concat(AutoEvents2.Children.OfType<ToggleButton>()).ToList();
            System.Diagnostics.Debug.WriteLine("Found " + buttons.Count + " Buttons");
            InitButtons();
        }

        public void ButtonChecked(object sender, RoutedEventArgs e)
        {
            var s = (sender as ToggleButton);
            switch (s.Tag)
            {
                case ("levelUp"):
                    GameController.DoPlayerLevelUp = true;
                    break;
                case ("itemFinish"):
                    GameController.DoItemCompleted = true;
                    break;
                case ("baronPlay"):
                    if (!ActiveSettings._useOCR)
                        return;
                    GameController.DoBaronKill = true;
                    break;
                case ("elderPlay"):
                    if (!ActiveSettings._useOCR)
                        return;
                    GameController.DoElderKill = true;
                    break;
                case ("teamInfo"):
                    LeagueIngameController.Instance.gameController.OnBaronKilled();
                    break;
                case ("teamWR"):
                    break;
                case ("fullRelativeGoldGraph"):
                    if (!ActiveSettings._useOCR)
                        return;
                    break;
                case ("inhibs"):
                    break;
                case ("cs/m"):
                    break;
                case (""):
                    break;
                default:
                    break;
            }

            s.Background = ControlViewModel.OnBrush;
        }

        public void ButtonUnchecked(object sender, RoutedEventArgs e)
        {
            var s = (sender as ToggleButton);
            switch (s.Tag)
            {
                case ("levelUp"):
                    GameController.DoPlayerLevelUp = false;
                    break;
                case ("itemFinish"):
                    GameController.DoItemCompleted = false;
                    break;
                case ("baronPlay"):
                    GameController.DoBaronKill = false;
                    break;
                case ("elderPlay"):
                    GameController.DoElderKill = false;
                    break;
                case ("teamInfo"):
                    LeagueIngameController.Instance.gameController.OnBaronDespawn();
                    break;
                case ("teamWR"):
                    break;
                case ("fullRelativeGoldGraph"):
                    break;
                case ("inhibs"):
                    break;
                case ("cs/m"):
                    break;
                case (""):
                    break;
                default:
                    break;
            }

            s.Background = ControlViewModel.OffBrush;
        }

        public void ActivateButton(ToggleButton tb)
        {
            tb.IsEnabled = true;
            tb.Background = ControlViewModel.InactiveBrush;
        }

        public void DeactivateButton(ToggleButton tb)
        {
            tb.IsEnabled = false;
            tb.Background = ControlViewModel.OffBrush;
        }

        private void InitButtons()
        {
            buttons.ForEach(b =>
            {
                b.Background = ControlViewModel.OffBrush;
                switch (b.Tag)
                {
                    case ("levelUp"):
                        b.IsChecked = GameController.DoPlayerLevelUp;
                        break;
                    case ("itemFinish"):
                        b.IsChecked = GameController.DoItemCompleted;
                        break;
                    case ("baronPlay"):
                        b.IsChecked = GameController.DoBaronKill;
                        break;
                    case ("elderPlay"):
                        b.IsChecked = GameController.DoElderKill;
                        break;
                    case ("teamInfo"):
                        break;
                    case ("teamWR"):
                        break;
                    case ("fullRelativeGoldGraph"):
                        break;
                    case ("inhibs"):
                        break;
                    case ("cs/m"):
                        break;
                    case (""):
                        break;
                    default:
                        break;
                }
            });
        }

    }

    public class ControlViewModel : ViewModelBase
    {
        public string Title { get; set; }

        public string Type { get; set; }

        public Action OnActivate { get; set; }
        public Action OnDeactivate { get; set; }

        public string Description { get; set; }


        private SolidColorBrush _brush;

        public static SolidColorBrush OffBrush = new SolidColorBrush(Color.FromArgb(0x4D, 0x5C, 0x5C, 0x5C));
        public static SolidColorBrush OnBrush = new SolidColorBrush(Color.FromArgb(0x4D, 0x49, 0xE4, 0xE4));
        public static SolidColorBrush InactiveBrush = new SolidColorBrush(Color.FromArgb(0x1D, 0x5C, 0x5C, 0x5C));
        public SolidColorBrush ButtonColor 
        { 
            get { return _brush; } 
            set { _brush = value; OnPropertyChanged("ButtonColor"); } 
        }

        public ControlViewModel()
        {
            ButtonColor = OffBrush;
        }


        public ControlViewModel(string title, string type, string desc)
        {
            this.Title = title;
            this.Type = type;
            this.Description = desc;
        }

        public static List<ControlViewModel> GetAutomaticEvents()
        {

            return new List<ControlViewModel>
            {
                new ControlViewModel("Level Up Pop-Up", "levelUp", "Show Level Up indicators for levels 6/11/16"),
                new ControlViewModel("Item Finish Pop-Up", "itemFinish","Show Item completion notifications"),
                new ControlViewModel("Baron PowerPlay", "baronPlay", "Show baron power play notification"),
                new ControlViewModel("Team Info", "teamInfo", "Displays Team names and Logos"),
                new ControlViewModel("Team Winrates", "teamWR", "Displays Team Winrates in addition to their name and logo"),
                new ControlViewModel("Elder PowerPlay", "elderPlay", "Show elder gold difference notification")
            };
        }

        public static List<ControlViewModel> GetEvents()
        {
            return new List<ControlViewModel>
            {
                new ControlViewModel("Relative Gold Graph", "fullRelativeGoldGraph", "Show relative gold between both teams since the beginning of the Game"),
                new ControlViewModel("Inhib Indicators", "inhibs", "Show inhib timers and their current status"),
                new ControlViewModel("Player CS/M", "cs/m", "Displays a graph of the CS per Minute for all players")
            };
        }
    }
}
