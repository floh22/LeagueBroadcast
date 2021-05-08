using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class PickBanConfigViewModel : ObservableObject
    {
        public string Patch { get { return ConfigController.PickBan.frontend.patch; } set { ConfigController.PickBan.frontend.patch = value; OnPropertyChanged("Patch"); } }
        public bool Spells { get { return ConfigController.PickBan.frontend.spellsEnabled; } set { ConfigController.PickBan.frontend.spellsEnabled = value; OnPropertyChanged("Spells"); } }
        public bool Coaches { get { return ConfigController.PickBan.frontend.coachesEnabled; } set { ConfigController.PickBan.frontend.coachesEnabled = value; OnPropertyChanged("Coaches"); } }
        public bool Score { get { return ConfigController.PickBan.frontend.scoreEnabled; } set { ConfigController.PickBan.frontend.scoreEnabled = value; OnPropertyChanged("Score"); } }

        public static PickBanConfigViewModel ChampSelectSettings = new();

        public PickBanConfigViewModel(string patch, bool spells, bool coaches, bool score)
        {
            this.Patch = patch;
            this.Score = score;
            this.Spells = spells;
            this.Coaches = coaches;
        }

        public PickBanConfigViewModel()
        {

        }

        public void Init()
        {
            var frontend = ConfigController.PickBan.frontend;
            this.Patch = frontend.patch;
            this.Spells = frontend.spellsEnabled;
            this.Coaches = frontend.coachesEnabled;
            this.Score = frontend.scoreEnabled;
        }
    }
}
