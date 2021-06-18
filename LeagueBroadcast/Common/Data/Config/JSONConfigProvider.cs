using LeagueBroadcast.OperatingSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace LeagueBroadcast.Common.Data.Config
{
    class JSONConfigProvider
    {
        private static JSONConfigProvider _instance;
        public static JSONConfigProvider Instance => GETInstance();

        private string _configPath;

        public ObservableCollection<string> TeamConfigs;
        private JSONConfigProvider()
        {
            _configPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
            
            Log.Info("Config Provider started");
            Log.Info($"Looking for config files at /Config");

            if (!Directory.Exists(_configPath))
            {
                Log.Info("Init Config Folder");
                Directory.CreateDirectory(_configPath);
            }

            if(!Directory.Exists(Path.Join(_configPath, "Teams")))
            {
                Log.Info("Init Teams Folder");
                Directory.CreateDirectory(Path.Join(_configPath, "Teams"));
            }

            TeamConfigs = new ObservableCollection<string>(Directory
                .EnumerateFiles(Path.Join(_configPath, "Teams"), "*", SearchOption.AllDirectories)
                .Select(f => Path.GetFileName(f).Replace(".json", "")));
        }
        public void ReadConfig(JSONConfig config)
        {
            string fileLocation = Path.Combine(_configPath, $"{config.Name}.json");
            if(!File.Exists(fileLocation))
            {
                Log.Info($"Config {config.Name} not found. Restoring default config");
                config.RevertToDefault();
                WriteConfig(config);
                return;
            }

            var configString = File.ReadAllText(fileLocation);
            dynamic readConfig = JsonConvert.DeserializeObject<dynamic>(configString);
            bool updateValues = true;
            if (readConfig.FileVersion != config.GETCurrentVersion())
            {
                Log.Info($"Config {config.Name} outdated. Updating file version");
                updateValues = config.UpdateConfigVersion(readConfig.FileVersion.ToString(), configString);
            }
            if(updateValues)
            {
                Log.Info($"Found {config.Name}.json. Reading values");
                config.UpdateValues(configString);
            }
            if(Log.Instance.Level == Log.LogLevel.Verbose)
                Log.Verbose(config.GETJson());
            Log.Info($"Config {config.Name} loaded");
        }

        public void WriteConfig(JSONConfig config)
        {
            if(config.FileVersion == null)
            {
                return;
            }
            Log.Info($"Writing {config.Name} config to file");
            using var stream = File.CreateText(Path.Combine(_configPath, $"{config.Name}.json"));
            var json = config.GETJson();
            stream.Write(json);
            stream.Close();
            Log.Info($"Updated {config.Name} config file");
        }

        #region Team
        public bool ReadTeam(JSONConfig config)
        {
            string fileLocation = Path.Combine(Path.Combine(_configPath, "Teams"), $"{config.Name}.json");
            if (!File.Exists(fileLocation))
            {
                Log.Info($"Team {config.Name} not found on disk");
                return false;
            }

            var configString = File.ReadAllText(fileLocation);
            dynamic readConfig = JsonConvert.DeserializeObject<dynamic>(configString);
            if (readConfig.FileVersion != config.GETCurrentVersion())
            {
                Log.Info($"Team {config.Name} outdated. Updating file version");
                config.UpdateConfigVersion(readConfig.FileVersion.ToString(), configString);
            }
            config.UpdateValues(configString);
            Log.Info($"Team {config.Name} loaded");
            return true;
        }

        public void WriteTeam(JSONConfig config)
        {
            if (config.FileVersion == null)
            {
                return;
            }
            Directory.CreateDirectory(Path.Combine(_configPath, "Teams"));
            using var stream = File.CreateText(Path.Combine(Path.Combine(_configPath, "Teams"), $"{config.Name}.json"));
            var json = config.GETJson();
            stream.Write(json);
            stream.Close();
            Log.Info($"Updated {config.Name} team file");
        }
        #endregion

        private static JSONConfigProvider GETInstance()
        {
            if(_instance == null)
            {
                _instance = new();
            }
            return _instance;
        }
    }
}
