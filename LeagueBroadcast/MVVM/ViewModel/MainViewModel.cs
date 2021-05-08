using LeagueBroadcast.Common;
using LeagueBroadcast.MVVM.Core;
using LeagueBroadcast.MVVM.View;
using System.Windows;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand SettingsViewCommand { get; set; }

        public RelayCommand PickBanViewCommand { get; set; }

        public RelayCommand IngameViewCommand { get; set; }

        public RelayCommand PostGameViewCommand { get; set; }

        public static HomeViewModel HomeVM { get; set; }
        public static SettingsViewModel SettingsVM { get; set; }

        public static PickBanViewModel PickBanVM { get; set; }

        public static IngameViewModel IngameVM { get; set; }

        public static PostGameViewModel PostGameVM { get; set; }


        private ConnectionStatusViewModel _connectionStatus;

        public ConnectionStatusViewModel ConnectionStatus 
        {
            get { return _connectionStatus; }
            set { _connectionStatus = value; OnPropertyChanged(); Log.Verbose("Connection State Changed"); }
        }


        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private MainWindow Window;

        public MainViewModel()
        {
            HomeVM = new();
            PickBanVM = new();
            IngameVM = new();
            PostGameVM = new();
            SettingsVM = new();

            CurrentView = HomeVM;

            ConnectionStatus = ConnectionStatusViewModel.DISCONNECTED;

            HomeViewCommand = new(o => { OpenHomeView(); });
            SettingsViewCommand = new(o => { CurrentView = SettingsVM; });
            PickBanViewCommand = new(o => { OpenPickBanView(); });
            IngameViewCommand = new(o => { OpenIngameView(); });
            PostGameViewCommand = new(o => OpenPostGameView());

            PickBanVM.IsOpen = false;
            IngameVM.IsOpen = false;
            PostGameVM.IsOpen = false;

        }

        public void SetMainWindow(MainWindow Window)
        {
            this.Window = Window;
        }

        private void OpenHomeView()
        {
            if(CurrentView != HomeVM)
            {
                CurrentView = HomeVM;
            }
            PickBanVM.IsOpen = false;
            IngameVM.IsOpen = false;
            PostGameVM.IsOpen = false;
            Window.SetHomeSelected();
        }

        private void OpenPickBanView()
        {
            if (CurrentView != HomeVM)
            {
                CurrentView = HomeVM;
            }
            PickBanVM.IsOpen = true;
            IngameVM.IsOpen = false;
            PostGameVM.IsOpen = false;
            Window.SetPickBanSelected();
        }

        private void OpenIngameView()
        {
            if (CurrentView != HomeVM)
            {
                CurrentView = HomeVM;
            }
            PickBanVM.IsOpen = false;
            IngameVM.IsOpen = true;
            PostGameVM.IsOpen = false;
            Window.SetIngameSelected();
        }

        private void OpenPostGameView()
        {
            if (CurrentView != HomeVM)
            {
                CurrentView = HomeVM;
            }
            PickBanVM.IsOpen = false;
            IngameVM.IsOpen = false;
            PostGameVM.IsOpen = true;
            Window.SetPostGameSelected();
        }
    }
}
