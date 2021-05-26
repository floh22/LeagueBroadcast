using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;
using LeagueBroadcast.MVVM.View;
using System.Windows.Input;

namespace LeagueBroadcast.MVVM.ViewModel
{
    public class PickBanViewModel : ObservableObject
    {
        public bool IsActive
        {
            get { return ConfigController.Component.PickBan.IsActive; }
            set { ConfigController.Component.PickBan.IsActive = value; OnPropertyChanged(); }
        }

        private bool _isOpen;

        public bool IsOpen
        {
            get { return _isOpen; }
            set { _isOpen = value; OnPropertyChanged(); }
        }

        private DelegateCommand _openCommand;

        public DelegateCommand OpenCommand
        {
            get { return _openCommand; }
        }

        private DelegateCommand _closeCommand;

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
            set { _closeCommand = value; }
        }


        public PickBanViewModel()
        {

            TeamConfigViewModel.BlueTeam.Init(ConfigController.PickBan.frontend.blueTeam, "blue");
            TeamConfigViewModel.RedTeam.Init(ConfigController.PickBan.frontend.redTeam, "red");

            _openCommand = new(o => { 
                IsOpen = true; 
                BroadcastController.Instance.Main.SetPickBanSelected();
                MainViewModel.HomeVM.InfoButtonIsVisible = false;
                MainViewModel.HomeVM.InfoIsOpen = false;
            });
            _openCommand.MouseGesture = MouseAction.LeftClick;

            _closeCommand = new(o => { IsOpen = false; });
            _closeCommand.GestureKey = Key.Escape;
        }

    }
}