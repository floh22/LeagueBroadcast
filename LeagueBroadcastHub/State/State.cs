using LeagueBroadcastHub.Data;
using LeagueBroadcastHub.Data.Containers;
using LeagueBroadcastHub.Data.Containers.Objectives;
using LeagueBroadcastHub.Events.RiotEvents;
using LeagueBroadcastHub.Pages.ControlPages;
using LeagueBroadcastHub.Session;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueBroadcastHub.State
{
    class State
    {
        private GameController controller;

        public StateData stateData;
        public BackEndData backEndData;
        public List<DirtyEvent> pastIngameEvents;

        public Team blueTeam;
        public Team redTeam;

        public State(GameController controller)
        {
            pastIngameEvents = new List<DirtyEvent>();
            this.stateData = null;
            this.backEndData = null;
            this.controller = controller;
            this.blueTeam = null;
            this.redTeam = null;
        }

        public void UpdateEvents(List<DirtyEvent> allEvents, List<Objective> currentObjectives)
        {
            //Init Event List incase its empty
            if(pastIngameEvents.Count == 0)
            {
                pastIngameEvents = allEvents;
                return;
            }

            //Get all new events and send those events to all clients
            List<DirtyEvent> newEvents = new List<DirtyEvent>();
            allEvents.ForEach(e => {
                if (pastIngameEvents.Where(o => o.EventID == e.EventID).ToList().Count == 0)
                {
                    newEvents.Add(e);
                }
            });

            //Only incase OCR is being used. Make sure OCR is actually sending data
            if (ActiveSettings.current.UseOCR && currentObjectives.Count != 0)
            {
                var drake = currentObjectives.ElementAt(0);
                var baron = currentObjectives.ElementAt(1);


                //If OCR has found the Team for an objective, update the objective and send events
                if (drake.FoundTeam && !(blueTeam.hasElder || redTeam.hasElder))
                {    
                    Team t = null;
                    switch (drake.LastTakenBy)
                    {
                        case -1:
                            break;
                        case 0:
                            t = blueTeam;
                            backEndData.dragon.BlueStartGold -= 1350;
                            break;
                        case 1:
                            t = redTeam;
                            backEndData.dragon.RedStartGold -= 1350;
                            break;
                        default:
                            break;
                    }

                    if (t != null)
                    {
                        t.hasElder = true;
                        t.dragonsTaken.Add("elder");
                        System.Diagnostics.Debug.WriteLine($"Found Elder Team: {t.teamName}");
                        backEndData.dragon.DurationRemaining -= drake.TimeSinceTaken;
                        controller.OnElderKilled();
                    }
                }

                if (baron.FoundTeam && !blueTeam.hasBaron && !redTeam.hasBaron)
                {
                    Team t = null;
                    switch (baron.LastTakenBy)
                    {
                        case -1:
                            break;
                        case 0:
                            t = blueTeam;
                            backEndData.baron.BlueStartGold -= 1500;
                            break;
                        case 1:
                            t = redTeam;
                            backEndData.baron.RedStartGold -= 1500;
                            break;
                        default:
                            break;
                    }

                    if (t != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Found Team: {t.teamName}");
                        backEndData.baron.DurationRemaining -= baron.TimeSinceTaken;
                        controller.OnBaronKilled();
                        t.hasBaron = true;
                    }
                }

                //Update drakes incase one was killed
                if (stateData.dragon.Objective.IsAlive && !drake.IsAlive)
                {
                    var isElder = drake.Type.Equals("elder", StringComparison.OrdinalIgnoreCase);
                    //Move this before the team check since sometimes the team cannot be determined
                    if (isElder)
                    {
                        backEndData.dragon.BlueStartGold = stateData.blueGold;
                        backEndData.dragon.RedStartGold = stateData.redGold;
                        SetObjectiveData(backEndData.dragon, stateData.dragon, 148);
                        if(!drake.FoundTeam)
                        {
                            blueTeam.hasElder = false;
                            redTeam.hasElder = false;
                        }
                    }

                    int teamId = -1;

                    //Check incase the team couldnt be determined to not mess up data.
                    //TODO: THIS IS WRONG. What if drake was taken by another team but lastTaken hasnt been updated yet?
                    if (drake.LastTakenBy != -1)
                    {
                        //Update Team
                        Team t = (drake.LastTakenBy == 0) ? blueTeam : redTeam;
                        t.dragonsTaken.Add(drake.Type);

                        //Update StateData
                        if (drake.LastTakenBy == 0)
                            stateData.blueDragons.Add(drake.Type);
                        else
                            stateData.redDragons.Add(drake.Type);

                        //Update Info incase drake was elder drake
                        teamId = t.id;
                    }
                }
                //Update baron incase it was killed
                if (stateData.baron.Objective.IsAlive && !baron.IsAlive)
                {
                    System.Diagnostics.Debug.WriteLine("Baron Killed. Waiting for Team information");
                    backEndData.baron.BlueStartGold = stateData.blueGold;
                    backEndData.baron.RedStartGold = stateData.redGold;
                    SetObjectiveData(backEndData.baron, stateData.baron, 178);
                    if(!baron.FoundTeam)
                    {
                        blueTeam.hasBaron = false;
                        redTeam.hasBaron = false;
                    }
                    
                }

                //Save objective data
                stateData.dragon.Objective = drake;
                stateData.baron.Objective = baron;
            }

            //Save event data;
            pastIngameEvents = allEvents;
        }

        public void UpdateTeams(List<Player> newData, List<OCRTeam> teamData)
        {
            var bluePlayers = newData.Where(p => p.team == "ORDER").ToList();
            var redPlayers = newData.Where(p => p.team != "ORDER").ToList();

            //Init data on first run
            bool firstRun = false;
            if (blueTeam == null)
            {
                blueTeam = new Team(0, bluePlayers);
                firstRun = true;
            }
                
            if (redTeam == null)
            {
                redTeam = new Team(1, redPlayers);
                firstRun = true;
            }

            if (firstRun)
            {
                blueTeam.UpdateIDs();
                redTeam.UpdateIDs();

                GameInfoPage.InitPlayers(blueTeam);
                GameInfoPage.InitPlayers(redTeam);

                return;
            }

            //Iterate through each player
            var Players = GetAllPlayers();
            for(int i = 0; i < Players.Count; i++)
            {
                var p = Players.ElementAt(i);

                //Determine if the player has died during baron
                Player newP = newData.Single(p2 => p2.summonerName.Equals(p.summonerName, StringComparison.Ordinal));
                Team playerTeam = (newP.team == "ORDER") ? blueTeam : redTeam;
                if (playerTeam.hasBaron && newP.isDead && !p.diedDuringBaron)
                    p.diedDuringBaron = true;

                //Level up Events
                if (p.level < 6 && newP.level >= 6)
                    controller.OnLevelUp(new LevelUpEventArgs(p.id, 6));
                else if (p.level < 11 && newP.level >= 11)
                    controller.OnLevelUp(new LevelUpEventArgs(p.id, 11));
                else if (p.level < 16 && newP.level >= 16)
                    controller.OnLevelUp(new LevelUpEventArgs(p.id, 16));

                //New item Events
                var newItems = newP.items.ToList().Where(i => !p.items.ToList().Any(l => i.itemID == l.itemID));
                newItems = newItems.Where(i => DataDragon.Instance.FullIDs.Contains(i.itemID));

                newItems.ToList().ForEach(newI => controller.OnItemCompleted(new ItemCompletedEventArgs(p.id, DataDragon.Instance.GetItemById(newI.itemID))));


                //Update old information
                p.UpdateInfo(newP);
            }

            //Team Updates
            GetBothTeams().ForEach(t =>
            {
                //Only incase OCR is being used. Make sure OCR is actually returning data
                if (ActiveSettings._useOCR && teamData != null)
                {
                    //Update Gold Data
                    var ocrData = teamData.Single((t2) => t2.Id == t.id);
                    t.gold = ocrData.Gold;
                    t.goldHistory.Add(controller.gameData.gameTime, t.gold);

                    //Update State Data
                    if (t.id == 0)
                        stateData.blueGold = t.gold;
                    else
                        stateData.redGold = t.gold;

                    //Determine if the team still has baron
                    if (t.hasBaron && t.players.All((p) => p.diedDuringBaron))
                    {
                        t.hasBaron = false;
                        SetObjectiveData(backEndData.baron, stateData.baron, 0);
                        controller.OnBaronDespawn();
                    }

                    //Determine if the team still has elder
                    if(t.hasElder && t.players.All(p => p.diedDuringElder))
                    {
                        t.hasElder = false;
                        SetObjectiveData(backEndData.dragon, stateData.dragon, 0);
                        controller.OnBaronDespawn();
                    }
                }
            });
        }

        public List<Player> GetAllPlayers()
        {
            return blueTeam.players.Concat(redTeam.players).ToList();
        }

        public List<Team> GetBothTeams()
        {
            return new List<Team>() { blueTeam, redTeam };
        }

        public Dictionary<double, int> GetGoldGraph()
        {
            var outList = new Dictionary<double, int>();

            //If the two teams have different gold info, return now since the info is 100% wrong
            if (blueTeam.goldHistory.Count != redTeam.goldHistory.Count)
                return outList;

            //For each point in time, add gold difference at time to graph
            for (int i = 0; i < blueTeam.goldHistory.Count; i++)
            {
                outList.Add(controller.gameData.gameTime, blueTeam.goldHistory.ElementAt(i).Value - redTeam.goldHistory.ElementAt(i).Value);
            }

            return outList;
        }
        public void SetObjectiveData(BackEndObjective back, FrontEndObjective front, double time)
        {
            //Generate text version of time for frontend
            backEndData.baron.DurationRemaining = time;
            TimeSpan t = TimeSpan.FromSeconds(time);
            front.DurationRemaining = t.ToString(@"mm\:ss");

            //Gold differences since objective was taken
            var originalDiff = back.BlueStartGold - back.RedStartGold;
            var currentDiff = stateData.blueGold - stateData.redGold;

            //Check if blue has the objective to determine in which direction the gold difference should go
            var blueHasObjective = (front.Objective.GetObjectiveType() == Objective.ObjectiveType.Baron)? blueTeam.hasBaron : blueTeam.hasElder;

            //Difference between the gold gained inverted based on if Blue or Red team has the objective
            front.GoldDifference = (currentDiff - originalDiff) * (blueHasObjective ? 1 : -1);
        }
    }
}
