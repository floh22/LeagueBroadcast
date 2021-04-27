using LeagueBroadcast.MVVM.Core;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand SettingsViewCommand { get; set; }

        public HomeViewModel HomeVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }


        private ConnectionStatusViewModel _connectionStatus;

        public ConnectionStatusViewModel ConnectionStatus 
        {
            get { return _connectionStatus; }
            set { _connectionStatus = value; OnPropertyChanged(); }
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

        public MainViewModel()
        {
            HomeVM = new ();
            SettingsVM = new ();
            CurrentView = HomeVM;
            ConnectionStatus = ConnectionStatusViewModel.DISCONNECTED;

            HomeViewCommand = new(o => { CurrentView = HomeVM; });
            SettingsViewCommand = new(o => { CurrentView = SettingsVM; });
        }
    }
}
