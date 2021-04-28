using LeagueBroadcast.OperatingSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LeagueBroadcast.Common.Data.Config
{
    class JSONConfigProvider
    {
        private static JSONConfigProvider _instance;
        public static JSONConfigProvider Instance => GETInstance();

        private string _configPath;
        private JSONConfigProvider()
        {
            _configPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
            
            Log.Info("Config Provider started");
            Log.Verbose($"Looking for config files at {_configPath}");

            if (!Directory.Exists(_configPath))
            {
                Log.Info("Init Config Folder");
                Directory.CreateDirectory(_configPath);
            }
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
            if (readConfig.FileVersion != config.GETCurrentVersion())
            {
                Log.Info($"Config {config.Name} outdated. Updating file version");
                config.UpdateConfigVersion(readConfig.FileVersion, readConfig);
            }
            Log.Info($"Found {config.Name}.json. Reading values");
            config.UpdateValues(configString);
            if(Log.Instance.Level == Log.LogLevel.Verbose)
                Log.Verbose(config.GETJson());
            Log.Info($"Config {config.Name} loaded");
        }

        public void WriteConfig(JSONConfig config)
        {
            Log.Verbose($"Writing {config.Name} config to file");
            using var stream = File.CreateText(Path.Combine(_configPath, $"{config.Name}.json"));
            var json = config.GETJson();
            stream.Write(json);
            stream.Close();
            Log.Info($"Updated {config.Name} config file");
        }

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
