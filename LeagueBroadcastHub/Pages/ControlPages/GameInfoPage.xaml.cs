using LeagueBroadcastHub.Data;
using LeagueIngameServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
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

            if (LeagueIngameController.currentlyIngame && !LeagueIngameController.paused)
            {
                var gameState = LeagueIngameController.Instance.gameController.gameState;
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
    }

    public class PlayerViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public static ObservableCollection<PlayerViewModel> bluePlayers = new ObservableCollection<PlayerViewModel>();
        public static ObservableCollection<PlayerViewModel> redPlayers = new ObservableCollection<PlayerViewModel>();

        public static ObservableCollection<PlayerViewModel> BluePlayers { get { return bluePlayers; } set { bluePlayers = value; BluePlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(BluePlayers))); } }
        public static ObservableCollection<PlayerViewModel> RedPlayers { get { return redPlayers; } set { redPlayers = value; RedPlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(RedPlayers))); } }

        public static event PropertyChangedEventHandler BluePlayersChanged;
        public static event PropertyChangedEventHandler RedPlayersChanged;

        public string PlayerName { get; set; }

        public string ChampionName { get; set; }

        public bool HasBaron
        {
            get => HasBaronText == "Baron Active";
            set => HasBaronText = value ? "Baron Active" : "No Baron";
        }

        public int TeamID { get; set; }

        public string HasBaronText { get; set; }

        public PlayerViewModel(string playerName, string championName, int id, bool hasBaron)
        {
            this.PlayerName = playerName;
            this.ChampionName = championName;
            this.HasBaron = hasBaron;
            this.TeamID = id;
        }

        public PlayerViewModel()
        {
        }

        public static void OnProcessDrop(object sender, ProcessDropEventArgs<PlayerViewModel> e)
        {
            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            var team = e.DataItem.TeamID == 0 ? LeagueIngameController.Instance.gameController.gameState.blueTeam : LeagueIngameController.Instance.gameController.gameState.redTeam;

            Swap<Player>(team.players, e.OldIndex, e.NewIndex);

            team.UpdateIDs();

            e.ItemsSource.Move(lowerIdx, higherIdx); 
            e.ItemsSource.Move(higherIdx - 1, lowerIdx);
            

            e.Effects = DragDropEffects.Move;
        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

    }
}
