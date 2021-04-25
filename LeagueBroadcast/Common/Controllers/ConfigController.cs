using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.OperatingSystem;

namespace LeagueBroadcast.Common.Controllers
{
    class ConfigController
    {
        private static ConfigController _instance;
        public static ConfigController Instance => GetInstance();

        public static ChampSelect.Data.Config.PickBanConfig PickBan;

        public static ComponentConfig Component;
        public ConfigController()
        {
            Log.Info("Starting Config Controller");
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
    }
}
