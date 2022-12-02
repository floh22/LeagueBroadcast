using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Config
{
    public class AppConfig : JsonConfig
    {
        #region NonSerialized
        [JsonIgnore]
        public override string Name => "App.json";

        [JsonIgnore]
        public override StringVersion CurrentVersion => new(1, 0, 0);

        #endregion

        #region Serialized

        private int _webserverPort;
        public int WebserverPort { get { return _webserverPort; } set { _webserverPort = value; OnPropertyChanged(); } }

        private bool _createDebugLog;
        public bool CreateDebugLog { get { return _createDebugLog; } set { _createDebugLog = value; OnPropertyChanged(); } }

        private bool _checkForUpdates;
        public bool CheckForUpdates { get { return _checkForUpdates; } set { _checkForUpdates = value; OnPropertyChanged(); } }

        private string _updateRepositoryUrl = "";
        public string UpdateRepositoryUrl { get { return _updateRepositoryUrl; } set { _updateRepositoryUrl = value; OnPropertyChanged(); } }

        private string _updateRepositoryName = "";
        public string UpdateRepositoryName { get { return _updateRepositoryName; } set { _updateRepositoryName = value; OnPropertyChanged(); } }

        private bool _checkForUpdatedOffsets;
        public bool CheckForUpdatedOffsets { get { return _checkForUpdatedOffsets; } set { _checkForUpdatedOffsets = value; OnPropertyChanged(); } }

        private string _offsetRepository = "";
        public string OffsetRepository { get { return _offsetRepository; } set { _offsetRepository = value; OnPropertyChanged(); } }


        #endregion

        public override void RevertToDefault()
        {
            "Reverting App.json".Info();
            WebserverPort = 9001;
            CreateDebugLog = true;
            CheckForUpdates = true;
            UpdateRepositoryName = @"floh22/LeagueBroadcast";
            UpdateRepositoryUrl = "https://github.com/floh22/LeagueBroadcast";
            CheckForUpdatedOffsets = true;
            OffsetRepository = "https://api.github.com/repos/floh22/LeagueBroadcast/contents/Offsets?ref=v2";
        }

        public override void CheckForUpdate()
        {
            if (FileVersion != CurrentVersion)
            {
                $"{Name} update detected".Info();
            }
        }
    }
}
