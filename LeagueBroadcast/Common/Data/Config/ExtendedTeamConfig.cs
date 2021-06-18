using LeagueBroadcast.ChampSelect.Data.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Common.Data.Config
{
    class ExtendedTeamConfig : JSONConfig
    {
        #region ConfigOverhead
        [JsonIgnore]
        public string TeamName;
        public override string FileVersion { get => _fileVersion; set => _fileVersion = value; }

        [JsonIgnore]
        public static new string CurrentVersion => "1.0";

        public override string Name => TeamName;

        #endregion

        public TeamConfig Config;

        public string IconLocation;

        public ExtendedTeamConfig(string TeamName)
        {
            this.TeamName = TeamName;
            this._fileVersion = "1.0";
        }

        public override string GETCurrentVersion()
        {
            return CurrentVersion;
        }

        public override string GETDefaultString()
        {
            //Cannot get default since there is no default team
            throw new InvalidOperationException();
        }

        public override string GETJson()
        {
            return SerializeIndented(this);
        }

        public override void RevertToDefault()
        {
            //Cannot revert to default since there is no default team
            throw new InvalidOperationException();
        }

        public override bool UpdateConfigVersion(string oldVersion, string oldValues)
        {
            //No update needed yet
            return true;
        }

        public override void UpdateValues(string readValues)
        {
            var cfg = JsonConvert.DeserializeObject<ExtendedTeamConfig>(readValues);
            this.Config = cfg.Config;
            this.IconLocation = cfg.IconLocation;
        }
    }
}
