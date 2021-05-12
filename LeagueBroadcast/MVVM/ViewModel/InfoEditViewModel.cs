using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class InfoEditViewModel : ObservableObject
    {
        public bool InfoButtonIsVisible => MainViewModel.HomeVM.InfoButtonIsVisible;
        public DelegateCommand InfoEditButtonCommand => MainViewModel.HomeVM.InfoEditButtonCommand;
    }
}
