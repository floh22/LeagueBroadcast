using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class HomeViewModel : ObservableObject
    {
        public PickBanViewModel PickBanVM => MainViewModel.PickBanVM;

        public IngameViewModel IngameVM => MainViewModel.IngameVM;

        public PostGameViewModel PostGameVM => MainViewModel.PostGameVM;

        public HomeViewModel()
        {
        }
    }
}
