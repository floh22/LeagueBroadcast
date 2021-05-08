using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Data.LBH
{
    public class InfoSidePage
    {
        public string Title;
        public PlayerOrder Order;
        public List<PlayerTab> Players;

        public InfoSidePage(string Title, PlayerOrder Order, List<PlayerTab> Players)
        {
            this.Title = Title;
            this.Order = Order;
            this.Players = SetPlayersInOrder(Players);
        }

        public List<PlayerTab> SetPlayersInOrder(List<PlayerTab> Players)
        {
            //TODO
            return null;
        }

        public static InfoSidePage GetEXPSidePage()
        {
            //TODO
            return null;
        }

        public static InfoSidePage GetCSSidePage()
        {
            //TODO
            return null;
        }
    }

    public enum PlayerOrder
    {
        MaxToMin,
        MinToMax,
        BlueFirst,
        RedFirst
    }
}
