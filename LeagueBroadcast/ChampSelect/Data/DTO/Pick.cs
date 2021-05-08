using LeagueBroadcast.Common.Data.DTO;

namespace LeagueBroadcast.ChampSelect.Data.DTO
{
    public class Pick : PickBan
    {
        public int id;
        public SummonerSpell spell1;
        public SummonerSpell spell2;
        public bool isActive = false;
        public string displayName = "";

        public Pick(int id)
        {
            this.id = id;
        }
    }
}
