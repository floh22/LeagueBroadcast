using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.Ingame.Data.Config;
using LeagueBroadcast.Ingame.Data.Frontend;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        public static string FontLocation = Path.Combine(Directory.GetCurrentDirectory(), "Cache", "Fonts");

        public ObservableCollection<LocalFont> LocalFonts;

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
                var result = MessageBox.Show("Failed to load configuration. Corrupted Install detected. Try removing Config folder and restarting", "LeagueBroadcast", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    Application.Current.Shutdown();
                });
            }

            Log.Info("Loading config watchers");
            PickBanWatcher = new("PickBan.json", PickBan);
            ComponentWatcher = new("Component.json", Component);
            IngameWatcher = new("Ingame.json", Ingame);

            //TODO Somehow loading these fonts into a stylesheet
            /*
            Log.Info("Loading local fonts");
            Directory.CreateDirectory(FontLocation);

            new ObservableCollection<LocalFont>(Directory
                .EnumerateFiles(FontLocation, "*", SearchOption.AllDirectories)
                .Select(f => {
                    var fontLocation = $"/cache/Fonts/{Path.GetFileName(f)}";
                    var fontName = Path.GetFileNameWithoutExtension(f);
                    return new LocalFont();
                }));
            */
            App.Instance.Exit += OnClose;
        }

        public static void LoadOffsetConfig()
        {
            JSONConfigProvider.Instance.ReadConfig(Farsight);
            if(Farsight.FileVersion == null)
            {
                Log.Warn("Could not load Offsets");
                var result = MessageBox.Show("Failed to load offsets. Manually download or write Config/Farsight.json. Check github for current file version.", "LeagueBroadcast", MessageBoxButton.OK, MessageBoxImage.Error);
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

            try
            {
                PickBanWatcher.Stop();
                ComponentWatcher.Stop();
                FarsightWatcher.Stop();
                IngameWatcher.Stop();
                controller.WriteConfig(PickBan);
                controller.WriteConfig(Component);
                controller.WriteConfig(Farsight);
                controller.WriteConfig(Ingame);

                Log.Info("Configs saved");
            } catch
            {
                Log.Warn("Could not save all configs");
            }
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

        public void Stop()
        {
            watcher.Dispose();
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
                    await Task.Delay(500);
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
