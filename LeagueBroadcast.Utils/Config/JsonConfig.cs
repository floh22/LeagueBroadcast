using LeagueBroadcast.Utils;
using System;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Config
{
    public abstract class JsonConfig : ObservableObject, IFileConfig, IWatchableFile
    {
        #region SerializationIgnoredMembers

        [JsonIgnore]
        public abstract string Name { get; } 
        [JsonIgnore]
        public abstract StringVersion CurrentVersion { get; }

        [JsonIgnore]
        public string FilePath { get; set; } = ConfigController.GetConfigDirectory();

        public event EventHandler? FileChanged;
        public event EventHandler? FileRemoved;

        #endregion

        #region SerializedMembers
        public StringVersion FileVersion { get; set; } = new StringVersion(new int[] { 1, 0, 0 });
        #endregion

        public bool AttachFileWatcher()
        {
            return ConfigController.WatchConfig(this);
        }

        public bool DetachFileWatcher()
        {
            return ConfigController.UnwatchConfig(this);
        }

        public abstract void RevertToDefault();

        public abstract void CheckForUpdate();

        public JsonConfig()
        {

        }

        public JsonConfig(string filePath)
        {
            this.FilePath = filePath;
        }
    }

}
