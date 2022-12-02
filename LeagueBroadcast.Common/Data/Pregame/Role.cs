namespace LeagueBroadcast.Common.Data.Pregame
{

    public struct Role
    {
        public RoleType Type { get; set; }

        public int ChampionsInRole { get; set; }
    }
    public enum RoleType
    {
        Top,
        Jungle,
        Mid,
        Marksman,
        Support
    }
}
