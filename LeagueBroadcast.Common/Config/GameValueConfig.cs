using System.ComponentModel;
using System.Text.Json.Serialization;
using LeagueBroadcast.Utils;

#pragma warning disable CS8618
namespace LeagueBroadcast.Common.Config
{
    public class GameValueConfig : JsonConfig
    {
        #region NonSerialized
        [JsonIgnore]
        public override string Name => "GameValues.json";

        [JsonIgnore]
        public override StringVersion CurrentVersion => new(1, 0, 0);

        #endregion

        public ObjectiveValues Objectives { get; set; }

        public override void CheckForUpdate()
        {
            if (FileVersion < CurrentVersion)
            {
                //Nothing to update yet
            }
        }

        public override void RevertToDefault()
        {
            Objectives = new()
            {
                BaronDuration = 180,
                ElderDuration = 150,
                ElderSpawnDelay = 360,
                DragonSpawnDelay = 300,
                BaronSpawnDelay = 420
            };
        }

        public class ObjectiveValues : INotifyPropertyChanged
        {
            public int BaronDuration { get; set; }
            public int ElderDuration { get; set; }

            public int BaronSpawnDelay { get; set; }
            public int DragonSpawnDelay { get; set; }
            public int ElderSpawnDelay { get; set; }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
    }
}
