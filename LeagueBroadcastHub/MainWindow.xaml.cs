using LeagueBroadcastHub.Pages;
using LeagueBroadcastHub.Session;
using LeagueIngameServer;
using ModernWpf;
using ModernWpf.Controls;
using ModernWpf.Navigation;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

namespace LeagueBroadcastHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private LeagueIngameController ingameController;

        public MainWindow()
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine("Starting League Broadcast Hub");

            this.DataContext = this;

            LoadSettings();

            NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().First();
            Navigate(NavView.SelectedItem);
            ingameController = new LeagueIngameController();
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
        }

        private void OnClose(object sender, EventArgs e)
        {
            //Stop Frontend
            LeagueIngameController.Instance.OnAppExit();

            //LBH Settings
            Properties.Settings.Default.useOCR = ActiveSettings._useOCR;
            Properties.Settings.Default.resetPlayerPositions = ActiveSettings._resetPlayerPositions;
            Properties.Settings.Default.appMode = ActiveSettings._appMode;

            //Event Settings
            Properties.Settings.Default.doLevelUp = GameController.DoPlayerLevelUp;
            Properties.Settings.Default.doItemsCompleted = GameController.DoItemCompleted;
            Properties.Settings.Default.doBaronKill = GameController.DoBaronKill;
            Properties.Settings.Default.doElderKill = GameController.DoElderKill;

            Properties.Settings.Default.Save();
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
                CompletionPercentage = (double)current / (double)total;
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
        public static bool _useOCR;
        public static bool _resetPlayerPositions;
        public static byte _appMode;

        public static ActiveSettings current = new ActiveSettings();

        public bool UseOCR { get { return _useOCR; } set { _useOCR = value; Properties.Settings.Default.useOCR = value; OnPropertyChanged("UseOCR"); System.Diagnostics.Debug.WriteLine("OCR Toggled"); } }
        public bool ResetPlayerPositions { get { return _resetPlayerPositions; } set { _resetPlayerPositions = value; OnPropertyChanged("ResetPlayerPositions"); } }

        public byte AppMode { get { return _appMode; } set { _appMode = value; Properties.Settings.Default.appMode = value; OnPropertyChanged("AppMode"); } }
    }
}
