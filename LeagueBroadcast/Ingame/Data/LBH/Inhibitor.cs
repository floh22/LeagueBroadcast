using LeagueBroadcast.Common.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace LeagueBroadcast.Ingame.Data.LBH
{
    public class Inhibitor
    {

        public string id;
        public int key;

        public double timeLeft;

        public Inhibitor(int key, string id)
        {
            this.id = id;
            this.key = key;
            timeLeft = 0;
        }

    }

    public class InhibitorInfo
    {
        public List<Inhibitor> Inhibitors;

        public InhibitorInfo()
        {
            Inhibitors = new List<Inhibitor>() {
                    new Inhibitor(0, "T1_L1"),
                    new Inhibitor(1, "T1_C1"),
                    new Inhibitor(2, "T1_R1"),
                    new Inhibitor(3, "T2_R1"),
                    new Inhibitor(4, "T2_C1"),
                    new Inhibitor(5, "T2_L1")
                };
        }
    }
}
