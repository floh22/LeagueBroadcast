using System.Drawing;

namespace LeagueBroadcast.ChampSelect.Data.Config
{
    public class TeamConfig
    {
        public string name;
        public string nameTag;
        public int score;
        public string coach;
        public string color;
        private Color _Color { get { return _Color; } set { _Color = value; color = RGBToString(value); } }

        public static string RGBToString(Color c)
        {
            return c.ToString().ToLower();
        }

        public static TeamConfig DefaultConfig(string TeamName, string c)
        {
            string nameTag = TeamName.Length >= 3 ? TeamName.Substring(0, 3) : "TeamName";
            return new TeamConfig() { name = TeamName, score = 0, coach = "G2 Grabz", color = c, nameTag = nameTag };
        }
    }
}
