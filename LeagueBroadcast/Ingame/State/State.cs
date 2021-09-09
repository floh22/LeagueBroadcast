using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.Farsight;
using LeagueBroadcast.Farsight.Object;
using LeagueBroadcast.Ingame.Data.LBH;
using LeagueBroadcast.Ingame.Data.LBH.Objectives;
using LeagueBroadcast.Ingame.Data.RIOT;
using LeagueBroadcast.Ingame.Events;
using LeagueBroadcast.MVVM.View;
using LeagueBroadcast.MVVM.ViewModel;
using LeagueBroadcast.OperatingSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace LeagueBroadcast.Ingame.State
{
    class State
    {
        private IngameController controller;
        private bool ShowedChampionMemoryError = false;

        public StateData stateData;
        public List<RiotEvent> pastIngameEvents;
        public int lastGoldDifference;
        public double lastGoldUpdate;


        //Track past objectives
        private GameObject lastDragon;
        private GameObject lastHerald;
        private GameObject lastBaron;

        public Team blueTeam;
        public Team redTeam;

        public State(IngameController controller)
        {
            this.controller = controller;
            pastIngameEvents = new List<RiotEvent>();
            this.stateData = null;
            this.controller = controller;
            this.blueTeam = null;
            this.redTeam = null;
            this.lastGoldDifference = 0;
            this.lastGoldUpdate = 0;
            AppStateController.GameStop += (s, e) => { lastGoldDifference = 0; lastGoldUpdate = 0; };
        }

        public void UpdateTeams(List<Player> PlayerData, Snapshot gameSnap)
        {
            var bluePlayers = PlayerData.Where(p => p.team == "ORDER").ToList();
            var redPlayers = PlayerData.Where(p => p.team != "ORDER").ToList();

            //Init data on first run
            bool firstRun = false;
            if (blueTeam == null)
            {
                blueTeam = new Team(0, bluePlayers);
                stateData.scoreboard.BlueTeam = new(TeamConfigViewModel.BlueTeam.NameTag, false);
                firstRun = true;
            }

            if (redTeam == null)
            {
                redTeam = new Team(1, redPlayers);
                stateData.scoreboard.RedTeam = new(TeamConfigViewModel.RedTeam.NameTag, true);
                firstRun = true;
            }

            if (firstRun)
            {
                Log.Info("Init Team data");
                blueTeam.UpdateIDs();
                redTeam.UpdateIDs();

                IngameTeamsView.InitPlayers(blueTeam);
                IngameTeamsView.InitPlayers(redTeam);

                return;
            }

            //Update players
            GetAllPlayers().ForEach(p =>
            {
                //Get new player data
                Player newP = PlayerData.Where(p2 => p2.summonerName.Equals(p.summonerName, StringComparison.Ordinal)).FirstOrDefault();
                if (newP == null)
                    return;

                //Get player memory data. Remove special characters since memory names do not contain them
                GameObject playerObject;
                try
                {
                    playerObject = gameSnap.Champions.First(c => c.Name.Equals(p.championID, StringComparison.OrdinalIgnoreCase));
                } catch (Exception e)
                {
                    //Champ could not be found. Inform user that mapping is currently not working
                    Log.Warn(p.championName + " not found in memory snapshot. Values will be incorrect!");
                    if(!ShowedChampionMemoryError)
                    {
                        Log.Warn(e.Message);
                        Log.Verbose($"Players:\n{JsonConvert.SerializeObject(GetAllPlayers())}\nSnapshot:\n{JsonConvert.SerializeObject(gameSnap.Champions)}");
                        MessageBoxUtils.ShowErrorBox("Could not read all champion data from memory. Please submit an issue on github containing the current log and replay.");
                        ShowedChampionMemoryError = true;
                    }
                    return;
                }
                
                Team playerTeam = (newP.team == "ORDER") ? blueTeam : redTeam;

                //We have all the info we need now, so update the player and check if anything has happened
                //At some point move all this from ingame api to farsight, but for now this will have to work


                //Check if player died with buff
                if (playerTeam.hasBaron && newP.isDead && !p.diedDuringBaron)
                {
                    Log.Verbose($"{p.championName} died with baron buff");
                    p.diedDuringBaron = true;
                }
                if(playerTeam.hasElder && newP.isDead && !p.diedDuringElder)
                {
                    Log.Verbose($"{p.championName} died with elder buff");
                    p.diedDuringElder = true;
                }

                //If Viego is alive and outside of base, his item buys are most likely from possession
                if(!(p.championID == "Viego" && !p.isDead && (newP.team == "ORDER"? Vector3.Distance(Vector3.Zero, playerObject.Position) > 1200 : Vector3.Distance(new Vector3(15000,170, 15000), playerObject.Position) > 1200)))
                {
                    //Level up Events
                    if (p.level < 6 && newP.level >= 6)
                        controller.OnLevelUp(new LevelUpEventArgs(p.id, 6));
                    else if (p.level < 11 && newP.level >= 11)
                        controller.OnLevelUp(new LevelUpEventArgs(p.id, 11));
                    else if (p.level < 16 && newP.level >= 16)
                        controller.OnLevelUp(new LevelUpEventArgs(p.id, 16));
                }

                //New item Events
                var newItems = newP.items.ToList().Where(i => !p.items.ToList().Any(l => i.itemID == l.itemID));
                newItems = newItems.Where(i => DataDragon.Instance.FullIDs.Contains(i.itemID));

                newItems.ToList().ForEach(newI => controller.OnItemCompleted(new ItemCompletedEventArgs(p.id, DataDragon.Instance.GetItemById(newI.itemID))));

                //Gold
                p.goldHistory[stateData.gameTime] = playerObject.GoldTotal;

                p.UpdateInfo(newP);
                p.farsightObject = playerObject;
            });

            //Update Teams
            GetBothTeams().ForEach(t => {
                //Determine if the team still has baron
                if (t.hasBaron && t.players.All((p) => p.diedDuringBaron))
                {
                    t.hasBaron = false;
                    SetObjectiveData(stateData.backBaron, stateData.baron, 0);
                    controller.OnBaronEnd(null, EventArgs.Empty);
                    Log.Info("All Players died during baron");
                }

                //Determine if the team still has elder
                if (t.hasElder && t.players.All(p => p.diedDuringElder))
                {
                    t.hasElder = false;
                    SetObjectiveData(stateData.backBaron, stateData.dragon, 0);
                    controller.OnDragonEnd(null, EventArgs.Empty);
                    Log.Info("All Players died during elder");
                }
            });
        }

        public void UpdateEvents(List<RiotEvent> allEvents, Snapshot gameSnap)
        {
            //Init Event List incase its empty
            if (pastIngameEvents.Count == 0)
            {
                pastIngameEvents = allEvents;
                return;
            }

            //Get new events
            List<RiotEvent> newEvents = new List<RiotEvent>();
            allEvents.ForEach(e => {
                if (pastIngameEvents.Where(o => o.EventID == e.EventID).ToList().Count == 0)
                {
                    newEvents.Add(e);
                    if (Log.Instance.Level == Log.LogLevel.Verbose)
                        Log.Verbose($"New Event: {JsonConvert.SerializeObject(e)}");
                }
            });

            //Save event data;
            pastIngameEvents = allEvents;

            newEvents.ForEach(e => {
                switch (e.EventName)
                {
                    case "TurretKilled":
                        if (e.TurretKilled.Contains("T2"))
                        {
                            blueTeam.towers++;
                        }
                        if (e.TurretKilled.Contains("T1"))
                        {
                            redTeam.towers++;
                        }
                        break;
                    case "ChampionKill":
                        UpdateKillsForTeam(blueTeam);
                        UpdateKillsForTeam(redTeam);
                        break;
                    case "InhibKilled":
                        stateData.inhibitors.Inhibitors.Single(inhib => inhib.id == e.InhibKilled.Substring(9, 5)).timeLeft = 300;
                        break;
                    default:
                        break;
                }
            });


            //Check for Drake spawn
            if(gameSnap.Dragon.ID != 0 && lastDragon?.ID == 0)
            {
                //Get the last part of the drake name and then map the memory names to API names. I god damn hope these aren't language specific again
                string drakeName = gameSnap.Dragon.Name.Split("_")[^1].Replace("Air", "Cloud", StringComparison.OrdinalIgnoreCase).Replace("Earth", "Mountain", StringComparison.OrdinalIgnoreCase).Replace("Water", "Ocean", StringComparison.OrdinalIgnoreCase);

                //Dont think this should be saved as a game event? Don't for now and implement it here if I ever need it to be
                controller.OnObjectiveSpawn(drakeName);
            }

            //Check for Herald spawn
            if(gameSnap.Herald.ID != 0 && lastHerald?.ID == 0)
            {
                //Same as drake spawn
                controller.OnObjectiveSpawn("Herald");
            }

            //Update past objectives
            lastDragon = gameSnap.Dragon;
            lastBaron = gameSnap.Baron;
            lastHerald = gameSnap.Herald;
        }

        public void UpdateScoreboard()
        {
            stateData.scoreboard.GameTime = stateData.gameTime;
            stateData.scoreboard.SeriesGameCount = ConfigController.Component.Ingame.SeriesGameCount;

            var currentTeam = stateData.scoreboard.BlueTeam;
            currentTeam.Name = TeamConfigViewModel.BlueTeam.NameTag;
            currentTeam.Icon = TeamConfigViewModel.BlueTeam.IconName;
            currentTeam.Kills = blueTeam.kills;
            currentTeam.Towers = blueTeam.towers;
            currentTeam.Gold = blueTeam.GetGold(stateData.gameTime);
            currentTeam.Score = TeamConfigViewModel.BlueTeam.Score;

            currentTeam = stateData.scoreboard.RedTeam;
            currentTeam.Name = TeamConfigViewModel.RedTeam.NameTag;
            currentTeam.Icon = TeamConfigViewModel.RedTeam.IconName;
            currentTeam.Kills = redTeam.kills;
            currentTeam.Towers = redTeam.towers;
            currentTeam.Gold = redTeam.GetGold(stateData.gameTime);
            currentTeam.Score = TeamConfigViewModel.RedTeam.Score;

        }

        public void UpdateTeamColors()
        {
            //Nothing to update if not ingame
            if (blueTeam == null || redTeam == null)
                return;
            blueTeam.color = TeamConfigViewModel.BlueTeam.Color.ToSerializedString();
            redTeam.color = TeamConfigViewModel.RedTeam.Color.ToSerializedString();
        }

        #region Getters
        public Team GetTeam(string TeamName)
        {
            if (TeamName.Equals("Order", StringComparison.OrdinalIgnoreCase))
                return blueTeam;
            if (TeamName.Equals("Chaos", StringComparison.OrdinalIgnoreCase))
                return redTeam;
            return null;
        }

        public Dictionary<double, float> GetGoldGraph()
        {
            var outList = new Dictionary<double, float>();

            //For each point in time, add gold difference at time to graph
            Team teamToCount = blueTeam.players.Count > 0 ? blueTeam : redTeam;
            int dataPoints = teamToCount.players[0].goldHistory.Count;
            if(dataPoints < 2)
            {
                return new Dictionary<double, float>() {
                    {0, 0},
                    {1, 0}
                };
            }
            for (int i = 0; i < dataPoints; i++)
            {
                //Sum gold values for all players in respective team and get point in time
                float blueGold = blueTeam.GetGold(i);
                float redGold = redTeam.GetGold(i);
                double time = teamToCount.players[0].goldHistory.Keys.ElementAt(i);
                float goldDiff = blueGold - redGold;
                if(i == 0 || i == dataPoints - 1 || Math.Abs(outList.Last().Value - goldDiff) > 500 || time - outList.Last().Key > 15)
                {
                    outList.TryAdd(time, blueGold - redGold);
                }
            }

            return outList;
        }

        public void UpdateKillsForTeam(Team t)
        {
            t.kills = pastIngameEvents.Where(e => e.eventType.Equals("ChampionKill") && t.players.Select(p => p.summonerName).Contains(e.KillerName)).Count();
        }

        public List<Player> GetAllPlayers()
        {
            return blueTeam.players.Concat(redTeam.players).ToList();
        }

        public List<Team> GetBothTeams()
        {
            return new List<Team>() { blueTeam, redTeam };
        }
        #endregion

        #region Setters
        public void SetObjectiveData(BackEndObjective back, FrontEndObjective front, double time)
        {
            //Generate text version of time for frontend
            back.DurationRemaining = time;
            TimeSpan t = TimeSpan.FromSeconds(time);
            front.DurationRemaining = t.ToString(@"mm\:ss");

            //Gold differences since objective was taken
            var originalDiff = back.BlueStartGold - back.RedStartGold;
            var currentDiff = stateData.blueGold - stateData.redGold;

            //Check if blue has the objective to determine in which direction the gold difference should go
            var blueHasObjective = (front.Type == Objective.ObjectiveType.Baron) ? blueTeam.hasBaron : blueTeam.hasElder;

            //Difference between the gold gained inverted based on if Blue or Red team has the objective
            front.GoldDifference = (currentDiff - originalDiff) * (blueHasObjective ? 1 : -1);
        }

        public void ResetState()
        {
            this.blueTeam = null;
            this.redTeam = null;
            this.stateData = new StateData();
            this.stateData.inhibitors = new InhibitorInfo();
            this.pastIngameEvents = new List<RiotEvent>();
            Log.Info("Game State reset");
        }
        #endregion
    }
}
