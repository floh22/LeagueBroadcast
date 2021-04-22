using LeagueBroadcastHub.Data;
using LeagueBroadcastHub.Data.Game.RiotContainers;
using LeagueBroadcastHub.Log;
using LeagueBroadcastHub.Pages;
using LeagueBroadcastHub.Session;
using LeagueIngameServer;
using ModernWpf;
using ModernWpf.Controls;
using ModernWpf.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using WPF.JoshSmith.ServiceProviders.UI;

namespace LeagueBroadcastHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private BroadcastHubController ingameController;

        public MainWindow()
        {
            new Logging((Logging.LogLevel)Enum.Parse(typeof(Logging.LogLevel), Properties.Settings.Default.LogLevel));
            
            InitializeComponent();

            Logging.Info("Starting League Broadcast Hub");

            this.DataContext = this;

            LoadSettings();

            NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().First();
            Navigate(NavView.SelectedItem);
            ingameController = new BroadcastHubController();
            ingameController.Start();

            Loaded += delegate
            {
                UpdateAppTitle();
            };

        }

        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            if (ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            }
        }

        private void Window_ActualThemeChanged(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(ThemeManager.GetActualTheme(this));
        }

        void UpdateAppTitle()
        {
            //ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, TitleBar.GetSystemOverlayRightInset(this), currMargin.Bottom);
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            ContentFrame.GoBack();
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                Navigate(typeof(SettingsPage));
            }
            else
            {
                Navigate(args.InvokedItemContainer);
            }
        }

        private void NavView_PaneOpening(NavigationView sender, object args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavView_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            Thickness currMargin = AppTitleBar.Margin;
            if (sender.DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness((sender.CompactPaneLength * 2), currMargin.Top, currMargin.Right, currMargin.Bottom);

            }
            else
            {
                AppTitleBar.Margin = new Thickness(sender.CompactPaneLength, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }

            UpdateAppTitleMargin(sender);
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType() == typeof(SettingsPage))
            {
                NavView.SelectedItem = NavView.SettingsItem;
            }
            else
            {
                NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => GetPageType(x) == e.SourcePageType());
            }
        }

        private void UpdateAppTitleMargin(NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;

            Thickness currMargin = AppTitle.Margin;

            if ((sender.DisplayMode == NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                     sender.DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                AppTitle.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitle.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }

        private void Navigate(object item)
        {
            if (item is NavigationViewItem menuItem)
            {
                Type pageType = GetPageType(menuItem);
                if (ContentFrame.CurrentSourcePageType != pageType)
                {
                    ContentFrame.Navigate(pageType);
                }
            }
        }

        private void Navigate(Type sourcePageType)
        {
            if (ContentFrame.CurrentSourcePageType != sourcePageType)
            {
                ContentFrame.Navigate(sourcePageType);
            }
        }

        private Type GetPageType(NavigationViewItem item)
        {
            return item.Tag as Type;
        }

        private void LoadSettings()
        {
            ActiveSettings.current.UseOCR = Properties.Settings.Default.useOCR;
            ActiveSettings.current.AppMode = Properties.Settings.Default.appMode;
            ActiveSettings.current.UsePickBan = Properties.Settings.Default.UsePickBan;
            ActiveSettings.current.UseIngame = Properties.Settings.Default.UseIngame;
            ActiveSettings.current.DelayPickBan = Properties.Settings.Default.DelayPickBan;
            ActiveSettings.current.DelayPickBanValue = Properties.Settings.Default.DelayPBValue;
            ActiveSettings.current.UseReplayAPI = Properties.Settings.Default.UseReplayAPI;
            ActiveSettings.current.InitLeagueUI = Properties.Settings.Default.InitLeagueUI;
        }

        private void OnClose(object sender, EventArgs e)
        {
            //Stop Frontend
            BroadcastHubController.Instance.OnAppExit();

            Logging.Info("Saving Settings");
            //LBH Settings
            Properties.Settings.Default.useOCR = ActiveSettings.current.UseOCR;
            Properties.Settings.Default.appMode = ActiveSettings.current.AppMode;
            Properties.Settings.Default.UsePickBan = ActiveSettings.current.UsePickBan;
            Properties.Settings.Default.UseIngame = ActiveSettings.current.UseIngame;
            Properties.Settings.Default.DelayPickBan = ActiveSettings.current.DelayPickBan;
            Properties.Settings.Default.DelayPBValue = ActiveSettings.current.DelayPickBanValue;
            Properties.Settings.Default.UseReplayAPI = ActiveSettings.current.UseReplayAPI;
            Properties.Settings.Default.InitLeagueUI = ActiveSettings.current.InitLeagueUI;

            //Event Settings
            Properties.Settings.Default.doLevelUp = GameController.CurrentSettings.LevelUp;
            Properties.Settings.Default.doItemsCompleted = GameController.CurrentSettings.Items;
            Properties.Settings.Default.doBaronKill = GameController.CurrentSettings.Baron;
            Properties.Settings.Default.doElderKill = GameController.CurrentSettings.Elder;

            Properties.Settings.Default.Save();
            Logging.Info("League Broadcast Hub closed");
        }
    }

    public class LoadingProgress : ViewModelBase
    {
        public string _description;
        public int currentCount;
        public int totalCount;
        private bool _visible;
        private double _completion;
        private string _progressText;

        private static LoadingProgress _loadingPopUp = new LoadingProgress();
        public static LoadingProgress LoadingPopUp { set { _loadingPopUp = value; } get { return _loadingPopUp; } }


        public double CompletionPercentage { private set { _completion = value; } get { return _completion; } }

        public string Description { set { _description = value; OnPropertyChanged("Description"); } get { return _description; } }

        public bool IsVisible { set { _visible = value; OnPropertyChanged("IsVisible"); } get { return _visible; } }

        public string ProgressText { set { _progressText = value; OnPropertyChanged("ProgressText"); } get { return _progressText; } }

        public void UpdateProgress(int current, int total)
        {
            currentCount = current;
            totalCount = total;
            if (total != 0)
                CompletionPercentage = ((double)current / (double)total) * 100;
            else
                CompletionPercentage = 0;
            OnPropertyChanged("CompletionPercentage");

            ProgressText = $"{current}/{total}";
        }

        public LoadingProgress(string desc, int currentCount, int totalCount, bool visible)
        {
            this.Description = desc;
            this.IsVisible = visible;
            this.currentCount = currentCount;
            this.totalCount = totalCount;

            UpdateProgress(currentCount, totalCount);
        }

        public LoadingProgress() :this("",0,0,false){ }
    }

    public class ActiveSettings : ViewModelBase
    {
        private static bool _useOCR;
        private static bool _resetPlayerPositions;
        private static byte _appMode;
        private static bool _useIngame;
        private static bool _usePickBan;
        private static bool _delayPickBan;
        private static double _delayPickBanValue;
        private static bool _useReplayAPI;
        private static bool _initLeagueUI;

        public static ActiveSettings current = new ActiveSettings();

        public bool UseOCR { get { return _useOCR; } set { _useOCR = value; Properties.Settings.Default.useOCR = value; OnPropertyChanged("UseOCR"); System.Diagnostics.Debug.WriteLine("OCR Toggled"); } }
        public bool ResetPlayerPositions { get { return _resetPlayerPositions; } set { _resetPlayerPositions = value; OnPropertyChanged("ResetPlayerPositions"); } }
        public byte AppMode { get { return _appMode; } set { _appMode = value; Properties.Settings.Default.appMode = value; OnPropertyChanged("AppMode"); } }
        public bool UseIngame { get { return _useIngame; } set { _useIngame = value; OnPropertyChanged("UseIngame"); } }
        public bool UsePickBan { get { return _usePickBan; }  set { _usePickBan = value; OnPropertyChanged("UsePickBan"); } }
        public bool DelayPickBan { get { return _delayPickBan; } set { _delayPickBan = value; OnPropertyChanged("DelayPickBan"); } }
        public double DelayPickBanValue { get { return _delayPickBanValue; } set { _delayPickBanValue = value; OnPropertyChanged("DelayPickBanValue"); } }
        public bool UseReplayAPI { get { return _useReplayAPI; } set { _useReplayAPI = value; OnPropertyChanged("UseReplayAPI"); } }
        public bool InitLeagueUI { get { return _initLeagueUI; } set { _initLeagueUI = value; OnPropertyChanged("InitLeagueUI"); } }
    }

    public class PlayerViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public static ObservableCollection<PlayerViewModel> bluePlayers = new ObservableCollection<PlayerViewModel>();
        public static ObservableCollection<PlayerViewModel> redPlayers = new ObservableCollection<PlayerViewModel>();

        public static ObservableCollection<PlayerViewModel> BluePlayers { get { return bluePlayers; } set { bluePlayers = value; BluePlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(BluePlayers))); } }
        public static ObservableCollection<PlayerViewModel> RedPlayers { get { return redPlayers; } set { redPlayers = value; RedPlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(RedPlayers))); } }

        public static event PropertyChangedEventHandler BluePlayersChanged;
        public static event PropertyChangedEventHandler RedPlayersChanged;

        public string PlayerName { get; set; }

        public string ChampionName { get; set; }

        public bool HasBaron
        {
            get => HasBaronText == "Baron Active";
            set { HasBaronText = value ? "Baron Active" : ""; OnPropertyChanged("HasBaronText"); }
        }

        public int TeamID { get; set; }

        public string HasBaronText { get; set; }

        public PlayerViewModel(string playerName, string championName, int id, bool hasBaron)
        {
            this.PlayerName = playerName;
            this.ChampionName = championName;
            this.HasBaron = hasBaron;
            this.TeamID = id;
        }

        public static void AddPlayer(PlayerViewModel pvm, int TeamID)
        {
            if (TeamID == 0)
            {
                BluePlayers.Add(pvm);
                BluePlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(BluePlayers)));
            }
            else
            {
                RedPlayers.Add(pvm);
                RedPlayersChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(RedPlayers)));
            }
        }

        public PlayerViewModel()
        {
        }

        public static void OnProcessDrop(object sender, ProcessDropEventArgs<PlayerViewModel> e)
        {
            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            var team = e.DataItem.TeamID == 0 ? BroadcastHubController.Instance.gameController.gameState.blueTeam : BroadcastHubController.Instance.gameController.gameState.redTeam;

            Swap<Player>(team.players, e.OldIndex, e.NewIndex);

            team.UpdateIDs();

            e.ItemsSource.Move(lowerIdx, higherIdx);
            e.ItemsSource.Move(higherIdx - 1, lowerIdx);


            e.Effects = DragDropEffects.Move;
        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

    }
}
