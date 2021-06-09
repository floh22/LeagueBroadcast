using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Ingame.Data.LBH;
using LeagueBroadcast.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.JoshSmith.ServiceProviders.UI;

namespace LeagueBroadcast.MVVM.View
{
    /// <summary>
    /// Interaction logic for IngameTeamsView.xaml
    /// </summary>
    public partial class IngameTeamsView : UserControl
    {
        private SynchronizationContext syncContext;

        public static IngameTeamsView Instance;

        private readonly IDictionary<int, int> GameCountToIndex = new Dictionary<int, int>
        {
            {1, 0},
            {2, 1},
            {3, 2},
            {5, 3}
        };


        public IngameTeamsView()
        {
            InitializeComponent();

            Instance = this;
            syncContext = SynchronizationContext.Current;

            BluePlayerList.ItemsSource = IngameTeamsViewModel.BluePlayers;
            RedPlayerList.ItemsSource = IngameTeamsViewModel.RedPlayers;

            SeriesCountSelector.SelectedIndex = GameCountToIndex[ConfigController.Component.Ingame.SeriesGameCount];

            if (BroadcastController.CurrentLeagueState.HasFlag(LeagueState.InProgress) && !IngameController.IsPaused && IngameTeamsViewModel.BluePlayers.Count == 0)
            {
                var gameState = BroadcastController.Instance.IGController.gameState;

            }

            var blueDropManager = new ListViewDragDropManager<PlayerViewModel>(BluePlayerList);
            var redDropManager = new ListViewDragDropManager<PlayerViewModel>(RedPlayerList);
            blueDropManager.ProcessDrop += PlayerViewModel.OnProcessDrop;
            redDropManager.ProcessDrop += PlayerViewModel.OnProcessDrop;
        }

        private void SeriesCountSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gameCount = Int32.Parse((string)((ComboBoxItem)SeriesCountSelector.SelectedItem).Tag);
            if (gameCount != ConfigController.Component.Ingame.SeriesGameCount)
            {
                ConfigController.Component.Ingame.SeriesGameCount = gameCount;
            }
        }

        public static void InitPlayers(Team t)
        {
            List<PlayerViewModel> players = new List<PlayerViewModel>();

            t.players.ForEach(p => {
                var pvm = new PlayerViewModel(p.summonerName, p.championName, t.id, t.hasBaron && !p.diedDuringBaron);
                players.Add(pvm);
            });

            if (t.id == 0)
            {
                IngameTeamsViewModel.BluePlayers = new ObservableCollection<PlayerViewModel>(players);
                Instance.syncContext.Post(state => { Instance.BluePlayerList.ItemsSource = IngameTeamsViewModel.BluePlayers; }, null);
            }
            else
            {
                IngameTeamsViewModel.RedPlayers = new ObservableCollection<PlayerViewModel>(players);
                Instance.syncContext.Post(state => { Instance.RedPlayerList.ItemsSource = IngameTeamsViewModel.RedPlayers; }, null);
            }
        }

        public static void ClearPlayers()
        {
            Instance.syncContext.Post(state => { IngameTeamsViewModel.BluePlayers.Clear(); }, null);
            Instance.syncContext.Post(state => { IngameTeamsViewModel.RedPlayers.Clear(); }, null);
        }
    }
}
