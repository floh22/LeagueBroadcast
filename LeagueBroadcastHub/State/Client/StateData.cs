using LeagueBroadcastHub.Data;
using LeagueBroadcastHub.Data.Client.DTO;
using LeagueBroadcastHub.Data.Provider;
using LeagueBroadcastHub.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using static LeagueBroadcastHub.Data.Provider.DataDragon;

namespace LeagueBroadcastHub.State.Client
{
    public class StateData
    {
        public bool champSelectActive;
        public bool leagueConnected;
        public Team blueTeam = new Team();
        public Team redTeam = new Team();
#pragma warning disable IDE1006 // Naming Styles
        public Meta meta = new Meta();
#pragma warning restore IDE1006 // Naming Styles
        public long timer = 0;
        public string state = "PICK 1";
        private Config _config = new Config();
        public Config config { get { return _config; } }

        public StateData()
        {
        }

        public StateData(StateData toCopy)
        {
            this.champSelectActive = toCopy.champSelectActive;
            this.leagueConnected = toCopy.leagueConnected;
            this.blueTeam = toCopy.blueTeam;
            this.redTeam = toCopy.redTeam;
            this.meta = toCopy.meta;
            this.timer = toCopy.timer;
            this.state = toCopy.state;
        }

        public CurrentAction GetCurrentAction()
        {
            if(blueTeam.isActive && redTeam.isActive)
            {
                var ret = new CurrentAction();
                ret.state = "none";
                return ret;
            }
            if(!blueTeam.isActive && !redTeam.isActive)
            {
                var ret = new CurrentAction();
                ret.state = "none";
                return ret;
            }

            var activeTeam = blueTeam.isActive ? blueTeam : redTeam;
            var activeTeamName = blueTeam.isActive ? "blueTeam" : "redTeam";

            var activeBans = activeTeam.bans.Where(ban => ban.isActive).ToList();
            var activePicks = activeTeam.picks.Where(pick => pick.isActive).ToList();

            if(activeBans.Count > 0)
            {
                var ret = new CurrentAction();
                ret.state = "ban";
                ret.data = new List<PickBan>() { activeBans.ElementAt(0) };
                ret.team = activeTeamName;
                ret.num = activeTeam.bans.IndexOf(activeBans.ElementAt(0));
                return ret;
            }

            if(activePicks.Count > 0)
            {
                var ret = new CurrentAction();
                ret.state = "pick";
                ret.data = new List<PickBan>() { activePicks.ElementAt(0) };
                ret.team = activeTeamName;
                ret.num = activeTeam.picks.IndexOf(activePicks.ElementAt(0));
            }

            var last = new CurrentAction();
            last.state = "none";
            return last;
        }

        public CurrentAction RefreshAction(CurrentAction action)
        {
            if(action.state == "none")
            {
                return action;
            }

            var team = action.team == "blueTeam" ? blueTeam : redTeam;

            action.data.Clear();
            //Do not refresh if champ select has ended
            if(team.picks.Count == 0 && team.bans.Count == 0)
            {
                return action;
            }

            if(action.state == "ban")
            {
                action.data.Add(team.bans.ElementAt(action.num));
            } else
            {
                action.data.Add(team.picks.ElementAt(action.num));
            }

            //Emulate JS Rest Parameter opterator
            //action.data = array[action.num];
   
            return action;
        }

        public class CurrentAction
        {
            public string state;
            public List<PickBan> data = new List<PickBan>();
            public string team = "";
            public int num = -1;

            public CurrentAction() { }
            public CurrentAction(CurrentAction toCopy)
            {
                this.state = toCopy.state;
                this.data = toCopy.data;
                this.team = toCopy.team;
                this.num = toCopy.num;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is CurrentAction))
                    return false;
                var second = (CurrentAction)obj;
                if (this.state != second.state)
                    return false;
                if (this.state == "none" && second.state == "none")
                    return true;
                if (this.team != second.team)
                    return false;
                if (this.num == second.num)
                    return true;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(state, data, team, num);
            }
        }

    }
}
