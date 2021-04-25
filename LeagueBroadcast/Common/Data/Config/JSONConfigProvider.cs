using LeagueBroadcast.OperatingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LeagueBroadcast.Common.Data.Config
{
    class JSONConfigProvider
    {
        private static JSONConfigProvider _instance;
        public static JSONConfigProvider Instance => GETInstance();

        private string _configPath;
        public JSONConfigProvider()
        {
            _configPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
            Log.Info("Config Provider started");
            Log.Verbose($"Looking for config files at {_configPath}");
        }
        public void ReadConfig(JSONConfig config)
        {
            string fileLocation = Path.Combine(_configPath, $"{config.Name}.json");
            if(!File.Exists(fileLocation))
            {
                Log.Info($"Config {config.Name} not found. Restoring default config");
                config.GETDefault();
            }

            dynamic readConfig = JsonSerializer.Deserialize<dynamic>(File.ReadAllText(fileLocation));
            if (readConfig.FileVersion != config.GETCurrentVersion())
            {
                config.UpdateConfigVersion(readConfig.FileVersion, readConfig);
            }

            config.UpdateValues(readConfig);
        }

        public void WriteConfig(JSONConfig config)
        {
            Log.Verbose($"Writing {config.Name} config to file");
            using var stream = File.CreateText(Path.Combine(_configPath, $"{config.Name}.json"));
            var json = config.GETJson();
            Log.Verbose(json);
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
