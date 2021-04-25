using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Config
{
    public abstract class JSONConfig
    {
        [JsonIgnore]
        public abstract string Name { get; }

        protected string _fileVersion;
        public abstract string FileVersion { get; set; }
        [JsonIgnore]
        public static string CurrentVersion { get; }

        public abstract string GETJson();

        public abstract void UpdateValues(dynamic readValues);

        public abstract string GETDefault();

        public abstract string GETCurrentVersion();

        public abstract void UpdateConfigVersion(string oldVersion, dynamic oldValues);

        public string SerializeIndented(object o)
        {
            return JsonSerializer.Serialize(o, new JsonSerializerOptions() { WriteIndented = true });
        }
    }
}
