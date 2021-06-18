using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Ingame.Data.RIOT;
using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WPF.JoshSmith.ServiceProviders.UI;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class IngameTeamsViewModel : ObservableObject, INotifyPropertyChanged
    {

        public bool AutoInitUI
        {
            get { return ConfigController.Component.Replay.UseAutoInitUI; }
            set { ConfigController.Component.Replay.UseAutoInitUI = value; OnPropertyChanged(); }
        }


        public static ObservableCollection<PlayerViewModel> bluePlayers = new();
        public static ObservableCollection<PlayerViewModel> redPlayers = new();

        public static ObservableCollection<PlayerViewModel> BluePlayers { get { return bluePlayers; } set { bluePlayers = value; BluePlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(BluePlayers))); } }
        public static ObservableCollection<PlayerViewModel> RedPlayers { get { return redPlayers; } set { redPlayers = value; RedPlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(RedPlayers))); } }


        public static event PropertyChangedEventHandler BluePlayersChanged;
        public static event PropertyChangedEventHandler RedPlayersChanged;

        public static void InvokeBlueChanged()
        {
            BluePlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(BluePlayers)));
        }

        public static void InvokeRedChanged()
        {
            RedPlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(RedPlayers)));
        }
    }

    public class PlayerViewModel : ObservableObject
    {
        private string _playerName;
        public string PlayerName
        {
            get { return _playerName; }
            set { _playerName = value; OnPropertyChanged(); }
        }

        private string _championName;
        public string ChampionName
        {
            get { return _championName; }
            set { _championName = value; OnPropertyChanged(); }
        }

        public SolidColorBrush Color {get { return TeamConfigViewModel.BlueTeam.ColorBrush; } }

        public bool HasBaron
        {
            get => HasBaronText == "Baron Active";
            set { HasBaronText = value ? "Baron Active" : ""; OnPropertyChanged("HasBaronText"); }
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

        public static void AddPlayer(PlayerViewModel pvm, int TeamID)
        {
            if (TeamID == 0)
            {
                IngameTeamsViewModel.BluePlayers.Add(pvm);
                IngameTeamsViewModel.InvokeBlueChanged();
            }
            else
            {
                IngameTeamsViewModel.RedPlayers.Add(pvm);
                IngameTeamsViewModel.InvokeRedChanged();
            }
        }

        public PlayerViewModel()
        {
        }

        public static void OnProcessDrop(object sender, ProcessDropEventArgs<PlayerViewModel> e)
        {
            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            var team = e.DataItem.TeamID == 0 ? BroadcastController.Instance.IGController.gameState.blueTeam : BroadcastController.Instance.IGController.gameState.redTeam;

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
