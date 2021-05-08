namespace LeagueBroadcast.Ingame.Data.RIOT
{
    public class Score
    {
        public int assists;
        public int creepScore;
        public int deaths;
        public int kills;
        public float wardScore;

        public void Update(Score s, bool updateCS)
        {
            this.assists = s.assists;
            this.deaths = s.deaths;
            this.kills = s.kills;
            this.wardScore = s.wardScore;
            if (updateCS)
                this.creepScore = s.creepScore;
        }
    }
}
