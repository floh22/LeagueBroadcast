
using System.Collections.Generic;

namespace LeagueBroadcast.Common.Data.DTO
{
    public class SummonerSpell
    {
        public static List<SummonerSpell> SummonerSpells = new();

        public string id;
        public int key;
        public string name;
        public string icon;
    }
}
