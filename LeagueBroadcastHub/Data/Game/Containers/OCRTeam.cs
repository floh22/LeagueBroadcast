using System;
namespace LeagueBroadcastHub.Data.Game.Containers
{
    class OCRTeam
    {
        public int Id;
        public string TeamName;
        public int Gold;

        public OCRTeam(int Id, string TeamName, int Gold)
        {
            this.Id = Id;
            this.TeamName = TeamName;
            this.Gold = Gold;
        }
    }
}
