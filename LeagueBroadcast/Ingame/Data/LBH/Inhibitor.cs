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
            this.timeLeft = 0;
        }

        private string GetTimerString()
        {
            return timeLeft > 0 ? TimeSpan.FromMilliseconds(timeLeft).ToString(@"mm\:ss") : "-";
        }

        public static List<Inhibitor> InhibList()
        {
            return new List<Inhibitor>() {
                    new Inhibitor(0, "T1_L1"),
                    new Inhibitor(1, "T1_C1"),
                    new Inhibitor(2, "T1_R1"),
                    new Inhibitor(3, "T2_L1"),
                    new Inhibitor(4, "T2_C1"),
                    new Inhibitor(5, "T2_R1")
                };
        }
    }

    public class InhibitorInfo
    {
        public List<Inhibitor> Inhibitors;

        public Vector2 Location => ConfigController.Ingame.InhibDisplay.Location;

        public InhibitorInfo()
        {
            this.Inhibitors = Inhibitor.InhibList();
        }
    }
}
