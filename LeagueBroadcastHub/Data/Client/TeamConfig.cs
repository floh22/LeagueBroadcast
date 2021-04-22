using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeagueBroadcastHub.Data.Client
{
    public class TeamConfig
    {
        public string name;
        public int score;
        public string coach;
        public string color;
        private Color _Color { get { return _Color; } set { _Color = value; color = RGBToString(value); } }

        public static string RGBToString(Color c)
        {
            return c.ToString().ToLower();
        }

        public static TeamConfig DefaultConfig(string TeamName, string c )
        {
            return new TeamConfig() { name = TeamName, score = 0, coach = "G2 Grabz", color = c };
        }
    }
}
