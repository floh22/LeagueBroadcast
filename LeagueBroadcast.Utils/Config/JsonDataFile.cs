using LeagueBroadcast.Common.Config;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Utils.Config
{
    public abstract class JsonDataFile : IDataFile, IWatchableFile, INotifyPropertyChanged
    {
        #region SerializationIgnoredMembers

        [JsonIgnore]
        public abstract string Name { get; set; }
        [JsonIgnore]
        public abstract StringVersion CurrentVersion { get; }

        public string SubFolder { get; set; }

        [JsonIgnore]
        public string FilePath { get; set; }

        public event EventHandler? FileChanged;
        public event EventHandler? FileRemoved;
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region SerializedMembers
        public StringVersion FileVersion { get; set; } = new StringVersion(new int[] { 1, 0, 0 });
        #endregion

        public bool AttachFileWatcher()
        {
            return DataFileController.WatchFile(this);
        }

        public bool DetachFileWatcher()
        {
            return DataFileController.UnwatchFile(this);
        }

        public abstract void RevertToDefault(string fileName);

        public abstract void CheckForUpdate();

        public JsonDataFile(string subFolder)
        {
            this.SubFolder = subFolder;
            this.FilePath = Path.Combine(DataFileController.GetDataDirectory(), this.SubFolder);
            Directory.CreateDirectory(this.FilePath);
        }
    }
}
