using LeagueBroadcast.Utils;

namespace LeagueBroadcast.Common.Data
{
    public class CoachInfo : ObservableObject, IPersonalInfo
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string PictureUri { get; set; }

        public CoachInfo(string name, int age, string pictureUri)
        {
            Name = name;
            Age = age;
            PictureUri = pictureUri;
        }

        public CoachInfo(string name, int age)
        {
            Name = name;
            Age = age;
            PictureUri = "";
        }

        public CoachInfo(string name)
        {
            Name = name;
            Age = -1;
            PictureUri = "";
        }
    }
}
