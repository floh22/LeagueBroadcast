using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Utils;
using System.Drawing;

namespace LeagueBroadcast.Common.Data
{
    public class TeamInfo : ObservableObject
    {
        public string Name { get; set; }

        public string Tag { get; set; }
        public TeamScoreData CurrentScore { get; set; }
        public TeamHistoricScoreData HistoricScore { get; set; }

        //TODO move this out of here...
        public TeamSide Side { get; set; }
        public HashSet<PlayerInfo> Players { get; set; } 
        public HashSet<CoachInfo> Coaches { get; set; }
        public List<Color> Colors { get; set; }



        private static ComponentConfig _componentConfig = ConfigController.Get<ComponentConfig>();


        public Color GetPrimaryColor()
        {
            if(_componentConfig.PickBan.TeamInfo.UseTeamColors)
                return Colors.ElementAtOrDefault(0);
            return ColorTranslator.FromHtml(Side == TeamSide.Blue ? _componentConfig.PickBan.SideColors.BlueColorPrimary : _componentConfig.PickBan.SideColors.RedColorPrimary);
        }

        public Color GetSecondaryColor()
        {
            if (_componentConfig.PickBan.TeamInfo.UseTeamColors)
                return Colors.ElementAtOrDefault(1);
            return ColorTranslator.FromHtml(Side == TeamSide.Blue ? _componentConfig.PickBan.SideColors.BlueColorSecondary : _componentConfig.PickBan.SideColors.RedColorSecondary);
        }


        public TeamInfo(string name, string tag, TeamScoreData currentScore, TeamHistoricScoreData historicScore, TeamSide side, HashSet<PlayerInfo> players, HashSet<CoachInfo> coaches, List<Color> colors)
        {
            Name = name;
            Tag = tag;
            CurrentScore = currentScore;
            HistoricScore = historicScore;
            Side = side;
            Players = players;
            Coaches = coaches;
            Colors = colors;
        }
    }


    public class TeamHistoricScoreData
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }

        public TeamHistoricScoreData(int wins, int losses, int draws)
        {
            Wins = wins;
            Losses = losses;
            Draws = draws;
        }

        public TeamHistoricScoreData(int wins, int losses)
        {
            Wins = wins;
            Losses = losses;
            Draws = 0;
        }
    }

    public class TeamScoreData
    {
        public int Wins { get; set; }

        public int Losses { get; set; }

        public TeamScoreData (int wins, int losses)
        {
            Wins = wins;
            Losses = losses;
        }
    }
}
