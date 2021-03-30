using LeagueBroadcastHub.Data.Game.Containers;
using LeagueBroadcastHub.Session;
using LeagueIngameServer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;
using WPF.JoshSmith.ServiceProviders.UI;

namespace LeagueBroadcastHub.Pages.ControlPages
{
    /// <summary>
    /// Interaction logic for GameInfoPage.xaml
    /// </summary>
    public partial class GameInfoPage : Page
    {

        public static GameInfoPage Instance;
        private SynchronizationContext syncContext;

        public GameInfoPage()
        {
            InitializeComponent();

            Instance = this;
            syncContext = SynchronizationContext.Current;

            BluePlayerList.ItemsSource = PlayerViewModel.BluePlayers;
            RedPlayerList.ItemsSource = PlayerViewModel.RedPlayers;

            if (BroadcastHubController.CurrentLeagueState.Equals("InProgress") && !GameController.IsPaused && PlayerViewModel.BluePlayers.Count == 0)
            {
                var gameState = BroadcastHubController.Instance.gameController.gameState;
                InitPlayers(gameState.blueTeam);
                InitPlayers(gameState.redTeam);
            }

            var blueDropManager = new ListViewDragDropManager<PlayerViewModel>(BluePlayerList);
            var redDropManager = new ListViewDragDropManager<PlayerViewModel>(RedPlayerList);
            blueDropManager.ProcessDrop += PlayerViewModel.OnProcessDrop;
            redDropManager.ProcessDrop += PlayerViewModel.OnProcessDrop;
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
                PlayerViewModel.BluePlayers = new ObservableCollection<PlayerViewModel>(players);
                Instance.syncContext.Post(state => { Instance.BluePlayerList.ItemsSource = PlayerViewModel.BluePlayers; }, null);
            }
            else
            {
                PlayerViewModel.RedPlayers = new ObservableCollection<PlayerViewModel>(players);
                Instance.syncContext.Post(state => { Instance.RedPlayerList.ItemsSource = PlayerViewModel.RedPlayers; }, null);
            }
        }

        public static void ClearPlayers()
        {
            Instance.syncContext.Post(state => { PlayerViewModel.BluePlayers.Clear(); }, null);
            Instance.syncContext.Post(state => { PlayerViewModel.RedPlayers.Clear(); }, null);
        }
    }
}
