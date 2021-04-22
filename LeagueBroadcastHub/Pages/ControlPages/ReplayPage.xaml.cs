using LeagueBroadcastHub.Session;
using LeagueIngameServer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for ReplayPage.xaml
    /// </summary>
    public partial class ReplayPage : Page
    {

        private List<ToggleButton> Buttons = new List<ToggleButton>();
        public ReplayPage()
        {
            InitializeComponent();
        }

        public void ButtonChecked(object sender, RoutedEventArgs e)
        {
            var s = (sender as ToggleButton);
            switch (s.Tag)
            {
                case "tfui":
                    ReplayController.OpenTeamFightUI();
                    break;
                default:
                    break;
            }
            s.Background = ReplayControlViewModel.OnBrush;
        }
        public void ButtonUnchecked(object sender, RoutedEventArgs e)
        {
            var s = (sender as ToggleButton);
            switch (s.Tag)
            {
                case "tfui":
                    ReplayController.CloseTeamFightUI();
                    break;
                default:
                    break;
            }
            s.Background = ReplayControlViewModel.OffBrush;
        }

        private void InitButtons()
        {
            Buttons.ForEach(b => {
                switch(b.Tag)
                {
                    case "tfui":
                        b.IsChecked = ReplayController.State.TeamfightOpen;
                        break;
                    default:
                        break;
                }
            });
        }
    }

    public class ReplayControlViewModel : ViewModelBase
    {
        public enum ButtonState
        {
            Disabled,
            Off,
            On
        }

        public string Title { get; set; }

        public ButtonState State
        {
            get { return _state; }
            set
            {
                _state = value;
                switch (value)
                {
                    case ButtonState.Disabled:
                        ButtonColor = InactiveBrush;
                        break;
                    case ButtonState.Off:
                        ButtonColor = OffBrush;
                        break;
                    case ButtonState.On:
                        ButtonColor = OnBrush;
                        break;
                }
            }
        }
        public string Type { get; set; }
        public string Description { get; set; }

        private SolidColorBrush _brush;
        private ButtonState _state;

        public static SolidColorBrush OffBrush = new SolidColorBrush(Color.FromArgb(0x4D, 0x5C, 0x5C, 0x5C));
        public static SolidColorBrush OnBrush = new SolidColorBrush(Color.FromArgb(0x4D, 0x49, 0xE4, 0xE4));
        public static SolidColorBrush InactiveBrush = new SolidColorBrush(Color.FromArgb(0x1D, 0x5C, 0x5C, 0x5C));
        public SolidColorBrush ButtonColor
        {
            get { return _brush; }
            set { _brush = value; OnPropertyChanged("ButtonColor"); }
        }

        public ReplayControlViewModel()
        {
            ButtonColor = OffBrush;
        }


        public ReplayControlViewModel(string title, string type, string desc)
        {
            this.Title = title;
            this.Type = type;
            this.Description = desc;
        }

        public static ReplayControlViewModel TeamFightUI = new ReplayControlViewModel("Teamfight UI", "tfui", "Active Teamfight UI");
    }
}
