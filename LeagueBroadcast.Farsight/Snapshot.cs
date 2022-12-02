using System.Collections.Generic;

namespace LeagueBroadcast.Farsight
{
    public class Snapshot
    {
        public float GameTimeInSeconds = 0;
        public HashSet<GameObject> Champions = new();
        public HashSet<GameObject> Turrets = new();
        public HashSet<GameObject> Inhibitors = new();
        public GameObject Dragon = new();
        public GameObject Baron = new();
        public GameObject Herald = new();
        public Dictionary<int, GameObject> ObjectMap = new();
        public Dictionary<short, int> IndexToNetID = new();

        public HashSet<int> UpdatedThisFrame = new();
        public Snapshot()
        {
        }
    }
}
