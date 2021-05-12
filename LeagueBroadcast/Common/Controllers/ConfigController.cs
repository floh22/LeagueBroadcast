using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.Ingame.Data.Config;
using System;
using System.Windows;

namespace LeagueBroadcast.Common.Controllers
{
    class ConfigController
    {
        private static ConfigController _instance;
        public static ConfigController Instance => GetInstance();

        public static ChampSelect.Data.Config.PickBanConfig PickBan = new();

        public static ComponentConfig Component = new();

        public static FarsightConfig Farsight = new();
        public ConfigController()
        {
            Log.Info("Starting Config Controller");
            var controller = JSONConfigProvider.Instance;

            controller.ReadConfig(PickBan);
            controller.ReadConfig(Component);
            

            if(PickBan.FileVersion == null || Component.FileVersion == null)
            {
                Log.Warn("Config load failed");
                var result = MessageBox.Show("Failed to load configuration. Corrupted Install detected. Try removing Config folder and restarting", "Essence", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    Application.Current.Shutdown();
                });
            }

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
}
