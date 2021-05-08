﻿using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Ingame.Data.LBH;
using LeagueBroadcast.Ingame.Data.RIOT;
using LeagueBroadcast.Trinket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace LeagueBroadcast.Ingame.Data.Provider
{
    class LiveEventsDataProvider
    {
        private readonly LiveEventConnector Trinket;
        private IngameController Ingame => BroadcastController.Instance.IGController;

        private const string EnableAPIString = "[LiveEvents]\nEnable=1\nPort=34243";
        public LiveEventsDataProvider()
        {
            //Check default path for league install. Prompt user if it cannot be found
            Log.Info("Checking Default League install for LiveEventsAPI");
            bool found = false;
            ConfigController.Component.App.LeagueInstall.ForEach(InstallLocation => {
                found = CheckGameConfigLocation(InstallLocation) || found;
            });

            if(!found)
            {
                //Prompt user for league install location
                Log.Warn("Game Location not found. Asking user on startup");
            }

            Trinket = new LiveEventConnector();
            Trinket.OnLiveEvent += ReceiveLiveEvent;
            Trinket.OnLiveEvent += (s, e) => { Log.Verbose($"New Live Event:  {JsonConvert.SerializeObject(e)}"); };
            Trinket.OnConnect += (s, e) => { Log.Verbose("LiveEventAPI Connected"); };
            Trinket.OnConnectionError += (s, e) => { Log.Verbose("LiveEventAPI Connection Failed!"); Log.Warn(e); };

            AppStateController.GameLoad += (s, e) => {Trinket.Connect(); };
            AppStateController.GameStop += (s, e) => { Trinket.Disconnect(); Log.Verbose("LiveEventAPI Closed"); };
        }

        private bool CheckGameConfigLocation(string configLocation)
        {
            string LeagueFolder = Path.Join(Path.Join(Path.Join(configLocation, "Riot Games"), "League of Legends"), "Config");
            if (Directory.Exists(LeagueFolder))
            {
                Log.Info("Found League install location");
                var cfgContent = File.ReadAllText(Path.Join(LeagueFolder, "game.cfg"));
                if (!(cfgContent.Contains("[LiveEvents]") && cfgContent.Contains("Enable=1")))
                {
                    Log.Info("Could not find LiveEvents in game config. Appending to end");
                    File.AppendText(Path.Join(LeagueFolder, "game.cfg")).Write($"\n{EnableAPIString}");
                    Log.Info("Updated Game Config");
                }
                else
                {
                    Log.Info("LiveEvents API found in Game config");
                }

                try
                {
                    Log.Info("Verifying LiveEvents list");
                    var liveEventsCfg = Path.Join(LeagueFolder, "LiveEvents.ini");
                    var events = File.ReadAllLines(liveEventsCfg);

                    if (!events.Contains("OnMinionKill"))
                    {
                        File.AppendText(liveEventsCfg).Write("\nOnMinionKill");
                        Log.Info("Adding OnMinionKill to LiveEvents.ini");
                    }
                    if (!events.Contains("OnNeutralMinionKill"))
                    {
                        File.AppendText(liveEventsCfg).Write("\nOnNeutralMinionKill");
                        Log.Info("Adding OnNeutralMinionKill to LiveEvents.ini");
                    }
                    return true;
                }
                catch (FileNotFoundException)
                {
                    
                    Log.Info("LiveEvents.ini not found. Generating now");
                    File.WriteAllLines(Path.Join(LeagueFolder, "LiveEvents.ini"), new string[] { "OnMinionKill", "OnNeutralMinionKill" });
                    Log.Info("LiveEvents.ini created. Added only nescesary events!");
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void ReceiveLiveEvent(object sender, LiveEvent e)
        {
            if(e.SourceTeam.Equals("Neutral"))
            {
                return;
            }
            switch(e.EventName)
            {
                case ("OnMinionKill"):
                    OnMinionKill(e);
                    break;
                case ("OnNeutralMinionKill"):
                    OnJglMinionKill(e);
                    break;
                default:
                    break;
            }
            
        }

        private void OnMinionKill(LiveEvent e)
        {
            Player p = GetPlayer(e);
            if (p != null)
                AddCS(p, 1);
        }
        private void OnJglMinionKill(LiveEvent e)
        {
            Player p = GetPlayer(e);
            if (p != null)
            {
                if(e.Other.StartsWith("SRU_Razorbeak") || e.Other.StartsWith("SRU_Krug") || e.Other.StartsWith("SRU_MurkwolfMini"))
                {
                    AddCS(p, 1);
                    return;
                }
                if(e.Other.StartsWith("SRU_Murkwolf"))
                {
                    AddCS(p, 2);
                    return;
                }
                AddCS(p, 4);
                if (e.Other.StartsWith("SRU_Dragon"))
                {
                    string type = e.Other.Remove(e.Other.Length - 5, 5).Remove(0, 11);


                    IngameController.DragonTaken.Invoke(this, new ObjectiveTakenArgs(type, GetTeam(e), Ingame.gameData.gameTime));
                    
                }
                if (e.Other.StartsWith("SRU_Baron"))
                {
                    Team t = GetTeam(e);
                    t.hasBaron = true;
                    t.players.ForEach(p => p.diedDuringBaron = false);
                    IngameController.BaronTaken.Invoke(this, new ObjectiveTakenArgs("Baron", GetTeam(e), Ingame.gameData.gameTime));
                }
            }
        }

        private void AddCS(Player p, int CS)
        {
            p.scores.creepScore += CS;
            p.csHistory[Ingame.gameData.gameTime] = p.scores.creepScore;
        }

        private Player GetPlayer(LiveEvent e)
        {
            return Ingame.gameState.GetTeam(e.SourceTeam).players.SingleOrDefault(p => p.summonerName.Equals(e.Source, StringComparison.OrdinalIgnoreCase));
        }

        private Player GetPlayer(LiveEvent e, Team t)
        {
            return t.players.SingleOrDefault(p => p.summonerName.Equals(e.Source, StringComparison.OrdinalIgnoreCase));
        }

        private Team GetTeam(LiveEvent e)
        {
            return Ingame.gameState.GetTeam(e.SourceTeam);
        }

    }

    public class ObjectiveTakenArgs
    {
        public string Type;
        public Team Team;
        public double GameTime;

        public ObjectiveTakenArgs(string Type, Team Team, double GameTime)
        {
            this.Type = Type;
            this.Team = Team;
            this.GameTime = GameTime;
        }
    }
}