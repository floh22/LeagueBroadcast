using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;

namespace LeagueBroadcast.Ingame.Data.Frontend
{
    public class ScoreboardConfig
    {
        public FrontEndTeam BlueTeam;
        public FrontEndTeam RedTeam;
        public double GameTime;
        public int SeriesGameCount;

        public bool ShouldSerializeGameTime()
        {
            return ConfigController.Component.Ingame.UseCustomScoreboard;
        }

        public bool ShouldSerializeSeriesGameCount()
        {
            return IngameController.CurrentSettings.TeamStats;
        }
    }
}
