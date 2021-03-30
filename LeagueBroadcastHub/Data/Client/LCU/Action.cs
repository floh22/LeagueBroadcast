using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Data.Client.LCU
{
    public class Action
    {
        public bool completed;
        public int championId;
        public string type;
        public int actorCellId;
    }

    public class ActionType
    {
        public ActionType(string value) { Value = value; }

        public string Value { get; set; }

        public static ActionType PICK { get { return new ActionType("pick"); } }
        public static ActionType BAN { get { return new ActionType("ban"); } }
    }
}
