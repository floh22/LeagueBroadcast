using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.ChampSelect.Data.Config
{
    public class FrontendConfig
    {
        public bool scoreEnabled;
        public bool spellsEnabled;
        public bool coachesEnabled;
        public TeamConfig blueTeam;
        public TeamConfig redTeam;
        public string patch;

        public static FrontendConfig CreateDefaultConfig()
        {
            return new FrontendConfig() { scoreEnabled = true, spellsEnabled = true, coachesEnabled = true, blueTeam = TeamConfig.DefaultConfig("TUM", "rgb(80,140,230)"), redTeam = TeamConfig.DefaultConfig("LMU", "rgb(239,66,67)") };
        }
    }
}
