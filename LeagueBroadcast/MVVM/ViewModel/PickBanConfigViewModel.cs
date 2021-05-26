using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;
using LeagueBroadcast.OperatingSystem;
using System;
using System.Windows.Media;

namespace LeagueBroadcast.MVVM.ViewModel
{
    public class PickBanConfigViewModel : ObservableObject
    {
        public string Patch { get { return ConfigController.PickBan.frontend.patch; } set { ConfigController.PickBan.frontend.patch = value; OnPropertyChanged("Patch"); } }
        public bool Spells { get { return ConfigController.PickBan.frontend.spellsEnabled; } set { ConfigController.PickBan.frontend.spellsEnabled = value; OnPropertyChanged("Spells"); } }
        public bool Coaches { get { return ConfigController.PickBan.frontend.coachesEnabled; } set { ConfigController.PickBan.frontend.coachesEnabled = value; OnPropertyChanged("Coaches"); } }
        public bool Score { get { return ConfigController.PickBan.frontend.scoreEnabled; } set { ConfigController.PickBan.frontend.scoreEnabled = value; OnPropertyChanged("Score"); } }
        public Color DefaultBlueColor { get { return ConfigController.Component.PickBan.DefaultBlueColor.ToColor(); } set { ConfigController.Component.PickBan.DefaultBlueColor = value.ToSerializedString(); OnPropertyChanged(); OnPropertyChanged("DefaultBlueColorBrush"); } }
        public Color DefaultRedColor { get { return ConfigController.Component.PickBan.DefaultRedColor.ToColor(); } set { ConfigController.Component.PickBan.DefaultRedColor = value.ToSerializedString(); OnPropertyChanged(); OnPropertyChanged("DefaultRedColorBrush"); } }
        public SolidColorBrush DefaultBlueColorBrush { get { return new SolidColorBrush(DefaultBlueColor); } }
        public SolidColorBrush DefaultRedColorBrush { get { return new SolidColorBrush(DefaultRedColor); } }
        public bool UseDelay { get { return ConfigController.Component.PickBan.UseDelay; } set { ConfigController.Component.PickBan.UseDelay = value; OnPropertyChanged("UseDelay"); } }

        public string DelayValue { get { return ConfigController.Component.PickBan.DelayValue.ToString(); } set { UpdateDelay(value); OnPropertyChanged("DelayValue"); } }

        public static PickBanConfigViewModel ChampSelectSettings = new();

        public PickBanConfigViewModel(string patch, bool spells, bool coaches, bool score)
        {
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

        private void UpdateDelay(string delay)
        {
            bool res = Int32.TryParse(delay, out var val);
            if(res)
            {
                ConfigController.Component.PickBan.DelayValue = val;
            } else
            {
                Log.Warn($"Could not update delay value to {delay}");
            }
        }
    }
}
