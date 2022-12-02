using LeagueBroadcast.Common.Data.Pregame;
using LeagueBroadcast.Utils;

namespace LeagueBroadcast.Common.Data
{
    public class PlayerInfo : ObservableObject, IPersonalInfo
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string PictureUri { get; set; }
        public RoleType Role { get; set; }
        public Pick Pick { get; set; }

        public PlayerInfo(string name, int age, string pictureUri, RoleType role, Pick pick)
        {
            Name = name;
            Age = age;
            PictureUri = pictureUri;
            Role = role;
            Pick = pick;
        }


        public PlayerInfo(string name, RoleType role, Pick pick)
        {
            Name = name;
            Age = -1;
            PictureUri = "";
            Role = role;
            Pick = pick;
        }
    }
}
