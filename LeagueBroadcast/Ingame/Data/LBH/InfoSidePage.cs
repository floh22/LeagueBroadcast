using System;
using System.Collections.Generic;
using System.Linq;
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
            switch (Order)
            {
                case PlayerOrder.MaxToMin:
                    return Players.OrderByDescending(o => o.Values.MaxValue).ThenByDescending(o => o.Values.CurrentValue).ToList();
                case PlayerOrder.MinToMax:
                    return Players.OrderBy(o => o.Values.MaxValue).ThenBy(o => o.Values.CurrentValue).ToList();
                default:
                    break;
            }

            throw new Exception();
        }
    }

    public enum PlayerOrder
    {
        MaxToMin,
        MinToMax
    }
}
