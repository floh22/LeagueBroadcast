using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Common.Data.DTO
{
    public class Champion
    {
        public static List<Champion> Champions = new();

        public string id;
        public int key;
        public string name;
        public string splashImg;
        public string splashCenteredImg;
        public string loadingImg;
        public string squareImg;
    }
}
