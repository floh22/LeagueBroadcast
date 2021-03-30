using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcastHub.Data.Game.RiotContainers
{
    class GameMetaData
    {
        public string gameMode;
        public double gameTime;
        public string mapName;
        public int mapNumber;
        public string mapTerrain;

        public GameMetaData(string gameMode, double gameTime, string mapName, int mapNumber, string mapTerrain)
        {
            this.gameMode = gameMode;
            this.gameTime = gameTime;
            this.mapName = mapName;
            this.mapNumber = mapNumber;
            this.mapTerrain = mapTerrain;
        }

        public GameMetaData() : this("debug", -1, "debug", -1, "debug") { }
    }
}
