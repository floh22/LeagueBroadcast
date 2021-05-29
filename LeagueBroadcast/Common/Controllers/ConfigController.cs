using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.Ingame.Data.Config;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace LeagueBroadcast.Common.Controllers
{
    class ConfigController
    {
        private static ConfigController _instance;
        public static ConfigController Instance => GetInstance();

        public static ChampSelect.Data.Config.PickBanConfig PickBan = new();
        public static ConfigWatcher PickBanWatcher;

        public static ComponentConfig Component = new();
        public static ConfigWatcher ComponentWatcher;

        public static FarsightConfig Farsight = new();
        public static ConfigWatcher FarsightWatcher;

        public static IngameConfig Ingame = new();
        public static ConfigWatcher IngameWatcher;

        public static string ConfigLocation = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        public ConfigController()
        {
            Log.Info("Starting Config Controller");

            var controller = JSONConfigProvider.Instance;

            controller.ReadConfig(PickBan);
            controller.ReadConfig(Ingame);
            controller.ReadConfig(Component);

            if(PickBan.FileVersion == null || Component.FileVersion == null || Ingame.FileVersion == null)
            {
                Log.Warn("Config load failed");
                var result = MessageBox.Show("Failed to load configuration. Corrupted Install detected. Try removing Config folder and restarting", "Essence", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    Application.Current.Shutdown();
                });
            }

            Log.Info("Loading config watchers");
            PickBanWatcher = new("PickBan.json", PickBan);
            ComponentWatcher = new("Component.json", Component);
            IngameWatcher = new("Ingame.json", Ingame);

            App.Instance.Exit += OnClose;
        }

        public static void LoadOffsetConfig()
        {
            JSONConfigProvider.Instance.ReadConfig(Farsight);
            if(Farsight.FileVersion == null)
            {
                Log.Warn("Could not load Offsets");
                var result = MessageBox.Show("Failed to load offsets. Manually download or write Config/Farsight.json. Check github for current file version.", "Essence", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    Application.Current.Shutdown();
                });
            }

            FarsightWatcher = new("Farsight.json", Farsight);
        }

        private static ConfigController GetInstance()
        {
            if (_instance == null)
                _instance = new();
            return _instance;
        }

        public static void UpdateConfigFile(JSONConfig config)
        {
            JSONConfigProvider.Instance.WriteConfig(config);
        }

        private void OnClose(object sender, EventArgs e)
        {
            Log.Info("Saving all configs to file");
            var controller = JSONConfigProvider.Instance;

            controller.WriteConfig(PickBan);
            controller.WriteConfig(Component);
            controller.WriteConfig(Farsight);
            Log.Info("Configs saved");
        }
    }

    public class ConfigWatcher
    {
        private FileSystemWatcher watcher;
        private JSONConfig config;
        private bool waitingForRead;

        public ConfigWatcher(string configName, JSONConfig config)
        {
            this.config = config;

            watcher = new FileSystemWatcher(ConfigController.ConfigLocation)
            {
                NotifyFilter = NotifyFilters.LastWrite
            };

            watcher.Error += OnError;
            watcher.Changed += OnChanged;
            watcher.Filter = configName;

            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;

            Log.Info($"Watching {watcher.Filter} for changes");
        }

        public async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (waitingForRead)
                return;
            waitingForRead = true;

            Log.Info($"{watcher.Filter} change detected");
            int attempts = 0;
            while (attempts < 10)
            {
                try
                {
                    await Task.Delay(500);
                    config.Reload();
                    waitingForRead = false;
                    return;
                }
                catch
                {
                    Log.Warn($"{watcher.Filter} locked, cannot read!");
                    attempts++;
                }
            }

        }

        protected void OnError(object sender, ErrorEventArgs e) =>
            Log.Warn("File Watch error:" + e.GetException());
    }
}
