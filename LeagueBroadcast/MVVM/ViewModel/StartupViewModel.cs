using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.MVVM.Core;
using LeagueBroadcast.OperatingSystem;
using System;
using System.Windows;

namespace LeagueBroadcast.MVVM.ViewModel
{
    public class StartupViewModel : ObservableObject
    {
        public string AppVersion => $"v{BroadcastController.AppVersion}";

        private string _status;

        public string Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }

        private double _loadProgress;

        public double LoadProgress { get { return _loadProgress; } set { _loadProgress = value; OnPropertyChanged(); } }

        private int _loadingBarWidth;

        public int LoadingBarWidth
        {
            get { return _loadingBarWidth; }
            set { _loadingBarWidth = value; OnPropertyChanged(); }
        }

        private bool _showUpdateDialog;

        public bool ShowUpdateDialog
        {
            get { return _showUpdateDialog; }
            set { _showUpdateDialog = value; OnPropertyChanged(); }
        }

        private string _updateText = "An Update Is Available";

        public string UpdateText
        {
            get { return _updateText; }
            set { _updateText = value; OnPropertyChanged(); }
        }

        public EventHandler Update, SkipUpdate;

        public StartupViewModel()
        {
            DataDragon.FileDownloadComplete += (s, e) => UpdateCacheDownloadProgress(e);
        }

        public void UpdateLoadProgress(LoadStatus loadStatus, int progress = 100)
        {
            double next = (int)loadStatus.Next();
            double conversion = (next - (int)loadStatus) / 100;
            LoadProgress = (int)loadStatus + progress * conversion;
            LoadingBarWidth = (int)(LoadProgress / 100 * 380);
        }

        public void UpdateCacheDownloadProgress(FileLoadProgressEventArgs e)
        {
            LoadProgress = (double)LoadStatus.DDragonStart + ((double)e.Completed / (double)e.Total * ((double)LoadStatus.DDragonStart.Next() - (double)LoadStatus.DDragonStart));
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Application.Current.MainWindow is null)
                        return;
                    LoadingBarWidth = (int)(LoadProgress / 100 * Application.Current.MainWindow.ActualWidth);
                });
            }
            catch
            {
                //Cant update. Ignored
            }
            Status = $"{e.Task} {e.Completed}/{e.Total} Assets: {e.FileName}";
        }
    
}

    public enum LoadStatus
    {
        PreInit = 3,
        DDragonStart = 5,
        Init = 80,
        PostInit = 95,
        FinishInit = 100
    }
}
