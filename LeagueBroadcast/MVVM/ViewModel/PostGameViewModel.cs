using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class PostGameViewModel : ObservableObject
    {
        public bool IsActive
        {
            get { return ConfigController.Component.PostGame.IsActive; }
            set { ConfigController.Component.PostGame.IsActive = value; OnPropertyChanged(); }
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


        public PostGameViewModel()
        {
            _openCommand = new(o => {
                IsOpen = true;
                BroadcastController.Instance.Main.SetPostGameSelected();
                MainViewModel.HomeVM.InfoButtonIsVisible = false;
                MainViewModel.HomeVM.InfoIsOpen = false;
            });
            _openCommand.MouseGesture = MouseAction.LeftClick;

            _closeCommand = new(o => { IsOpen = false; });
            _closeCommand.GestureKey = Key.Escape;
        }
    }
}
