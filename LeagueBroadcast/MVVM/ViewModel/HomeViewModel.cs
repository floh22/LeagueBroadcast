using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class HomeViewModel : ObservableObject
    {
        public PickBanViewModel PickBanVM => MainViewModel.PickBanVM;

        public IngameViewModel IngameVM => MainViewModel.IngameVM;

        public PostGameViewModel PostGameVM => MainViewModel.PostGameVM;

        public InfoEditViewModel InfoEditVM => MainViewModel.InfoEditVM;

        private bool _infoIsOpen;

        public bool InfoIsOpen
        {
            get { return _infoIsOpen; }
            set { _infoIsOpen = value; OnPropertyChanged(); }
        }

        private DelegateCommand _infoEditButtonCommand;

        public DelegateCommand InfoEditButtonCommand
        {
            get { return _infoEditButtonCommand; }
            set { _infoEditButtonCommand = value; }
        }

        private bool _infoButtonIsVisible;

        public bool InfoButtonIsVisible
        {
            get { return _infoButtonIsVisible; }
            set { _infoButtonIsVisible = value; OnPropertyChanged(); }
        }


        public HomeViewModel()
        {
            _infoEditButtonCommand = new(o => { InfoIsOpen ^= true; });
            _infoEditButtonCommand.MouseGesture = MouseAction.LeftClick;

            InfoButtonIsVisible = true;
        }
    }
}
