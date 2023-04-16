using LeagueBroadcast.Farsight.Object;
using System.Collections.Generic;

namespace LeagueBroadcast.Farsight
{
    public class Snapshot
    {
        public List<GameObject> Champions = new();
        public GameObject Dragon = new();
        public GameObject Baron = new();
        public GameObject Herald = new();
        public HashSet<GameObject> Turrets = new();
        public Dictionary<int, GameObject> ObjectMap = new();
        public Dictionary<short, int> IndexToNetID = new();

        public string NextDragonType = "";

        public HashSet<int> UpdatedThisFrame = new();
        public Snapshot()
        {
        }
    }
}
