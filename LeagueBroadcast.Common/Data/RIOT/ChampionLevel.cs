using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Common.Data.RIOT
{
    public class ChampionLevel : IComparable
    {
        public static List<ChampionLevel> Levels = new() {
            new(1, 0),
            new(2, 280),
            new(3, 660),
            new(4, 1140),
            new(5, 1720),
            new(6, 2400),
            new(7, 3180),
            new(8, 4060),
            new(9, 5040),
            new(10, 6120),
            new(11, 7300),
            new(12, 8580),
            new(13, 9960),
            new(14, 11440),
            new(15, 13020),
            new(16, 14700),
            new(17, 16480),
            new(18, 18360)
        };

        public static int EXPToLevel(float exp)
        {
            int index = Levels.BinarySearch(new ChampionLevel(0, exp));
            if(index < 0)
            {
                index = ~index - 1;
            }
            if(index >= 0)
            {
                return Levels[index].level;
            }
            Log.Warn("Tried converting negative XP to Level");
            return -1;
        }

        public float exp;
        public int level;
        public ChampionLevel(int level, float exp)
        {
            this.level = level;
            this.exp = exp;
        }

        public int CompareTo(object obj)
        {
            return exp.CompareTo(((ChampionLevel)obj).exp);
        }
    }


}
