using System.Collections.Generic;
using System.Numerics;

namespace LeagueBroadcast.Ingame.Data.RIOT
{
    public class Turret
    {
        public Dictionary<Player, double> LastDamagedByDictionary { get; set; }
        public string Name;
        public Vector3 Position;
        public float Health;

        public Turret(string name, Vector3 position, float health) { Name = name; Position = position; Health = health; LastDamagedByDictionary = new(); }
    }
}
