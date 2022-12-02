using LeagueBroadcast.Client.MVVM.Core;
using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Client
{
    public class ClientConfig : JsonConfig
    {
        #region NonSerialized
        [JsonIgnore]
        public override string Name => "Client.json";
        [JsonIgnore]
        public override StringVersion CurrentVersion => new(1, 0, 0);
        #endregion

        #region Serialized
        private StringVersion _lastSkippedVersion = StringVersion.Zero;

        public StringVersion LastSkippedVersion
        {
            get { return _lastSkippedVersion; }
            set { _lastSkippedVersion = value; OnPropertyChanged(); }
        }

        private HotKeySettings _hotKeys = new();

        public HotKeySettings HotKeys
        {
            get { return _hotKeys; }
            set { _hotKeys = value; OnPropertyChanged(); }
        }


        public ClientConfig()
        {
            HotKeys.PropertyChanged += (s, e) => { OnPropertyChanged("HotKeys"); };
        }

        #endregion

        public override void CheckForUpdate()
        {
            if (FileVersion != CurrentVersion)
            {
                $"{Name} update detected".Info();
            }
        }

        public override void RevertToDefault()
        {
            LastSkippedVersion = StringVersion.Zero;
        }
    }

    public class HotKeySettings : INotifyPropertyChanged
    {
        public bool IsEnabled { get; set; } = true;
        public HotKey SwapTeamSidesHotKey { get; set; } = HotKey.None;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
