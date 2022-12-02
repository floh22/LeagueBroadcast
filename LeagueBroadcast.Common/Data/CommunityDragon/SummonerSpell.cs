using LeagueBroadcast.Common.Json;
using LeagueBroadcast.Utils.Log;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.CommunityDragon
{
    public class SummonerSpell : IEqualityComparer<SummonerSpell>
    {
        private static HashSet<SummonerSpell> _all = new();
        public static HashSet<SummonerSpell> All
        {
            get
            {
                return _all;
            }
            set
            {
                foreach(var item in value)
                {
                    SummonerSpell spellToAdd = item as SummonerSpell;
                    if(_all.FirstOrDefault(item => item.ID.Equals(spellToAdd.ID)) is null)
                    {
                        _all.Add(item);
                    }
                }
            }
        }
        [JsonPropertyName("id")]
        [JsonConverter(typeof(NumberToStringJsonConverter))]
        public string ID { get; set; } = "";

        [JsonPropertyName("key")]
        public string Key => ID;

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("icon")]
        public string Icon => IconPath;

        [JsonPropertyName("iconPath")]
        public string IconPath { get; set; } = "";

        public bool Equals(SummonerSpell? spell1, SummonerSpell? spell2)
        {
            if(spell1 == null || spell2 == null)
            {
                return spell1 is null && spell2 is null;
            }
            return (spell1.ID.Equals(spell2.ID));
        }

        public int GetHashCode([DisallowNull] SummonerSpell obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}
