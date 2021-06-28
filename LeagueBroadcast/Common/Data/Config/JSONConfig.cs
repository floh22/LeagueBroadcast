using LeagueBroadcast.Common.Controllers;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Data.Config
{
    public abstract class JSONConfig
    {
        [JsonIgnore]
        public EventHandler ConfigUpdate;
        [JsonIgnore]
        public abstract string Name { get; }

        protected string _fileVersion;
        public abstract string FileVersion { get; set; }
        [JsonIgnore]
        public static string CurrentVersion { get; }

        public abstract string GETJson();

        public abstract void UpdateValues(string readValues);

        public abstract string GETDefaultString();

        public abstract void RevertToDefault();

        public abstract string GETCurrentVersion();

        public abstract bool UpdateConfigVersion(string oldVersion, string oldValues);

        public string SerializeIndented(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented);
        }

        public void UpdateFile()
        {
            ConfigController.UpdateConfigFile(this);
        }

        public void Reload()
        {
            if (ConfigUpdate == null)
                return;
            try {
                JSONConfigProvider.Instance.ReadConfig(this);
                ConfigUpdate.Invoke(this, EventArgs.Empty);
            } catch
            {
                Log.Warn($"Could not reload {this.Name}");
            }
            
        }
    }
}
