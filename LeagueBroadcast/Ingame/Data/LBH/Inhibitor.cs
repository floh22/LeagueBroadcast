using Newtonsoft.Json;
using System.Collections.Generic;

namespace LeagueBroadcast.Ingame.Data.LBH
{
    public class Inhibitor
    {
        [JsonIgnore]
        public static List<Inhibitor> Inhibitors => InhibList();

        public string id;
        public int key;
        public string timer;

        public Inhibitor(int key, string id)
        {
            this.id = id;
            this.key = key;
            timer = " - ";
        }

        private static List<Inhibitor> InhibList()
        {
            return new List<Inhibitor>() { new Inhibitor(0, "T1_L1"), new Inhibitor(1, "T1_C1"), new Inhibitor(2, "T1_R1"),
                new Inhibitor(3, "T2_L1"), new Inhibitor(4, "T2_C1"), new Inhibitor(5, "T2_R1")};
        }
    }
}
