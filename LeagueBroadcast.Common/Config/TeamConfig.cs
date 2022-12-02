using LeagueBroadcast.Common.Data;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Config;
using System.Drawing;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Config
{
    internal class TeamConfig : JsonDataFile
    {

        #region NonSerialized
        [JsonIgnore]
        public override string Name { get; set; }

        [JsonIgnore]
        public override StringVersion CurrentVersion => new(1,0,0);

        #endregion

        //This is trash
        public TeamInfo? TeamInfo { get; set; }

        public override void CheckForUpdate()
        {
            throw new NotImplementedException();
        }

        public override void RevertToDefault(string fileName)
        {
            Name = fileName;
            TeamInfo = new TeamInfo("Team Name", "TAG", new TeamScoreData(0, 0), new(0, 0), TeamSide.None, new() { new PlayerInfo("PLAYER1", 0, "", Data.Pregame.RoleType.Mid, new Data.Pregame.Pick(0)) }, new() { new CoachInfo("Marcel") }, new List<System.Drawing.Color>()
            {
                ColorTranslator.FromHtml("#7DAD1F"),
                ColorTranslator.FromHtml("#000000")
            });
        }

        public TeamConfig(string filePath, string fileName) : base(filePath)
        {
            Name = fileName;
        }
    }
}
