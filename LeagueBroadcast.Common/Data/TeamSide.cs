namespace LeagueBroadcast.Common.Data
{
    public enum TeamSide
    {
        None,
        Blue,
        Red
    }

    public enum IngameTeamSide
    {
        None,
        Order,
        Chaos
    }


    public static class TeamSideExtensions
    {
        public static TeamSide ToTeamSide(this IngameTeamSide side)
        {
            return side == IngameTeamSide.Chaos ? TeamSide.Blue : TeamSide.Red;
        }

        public static IngameTeamSide ToIngameTeamSide(this TeamSide side)
        {
            return side == TeamSide.Blue ? IngameTeamSide.Order : IngameTeamSide.Chaos;
        }

        public static TeamSide? ToTeamSide(string sideString)
        {
            if(Enum.TryParse(sideString, out TeamSide side))
            {
                return side;
            }

            return null;
        }

        public static IngameTeamSide? ToIngameTeamSide(string sideString)
        {
            if (Enum.TryParse(sideString, out IngameTeamSide side))
            {
                return side;
            }

            return null;
        }
    }
}
