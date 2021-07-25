using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Utils;
using LeagueBroadcast.Ingame.Data.LBH;
using LeagueBroadcast.Ingame.Data.RIOT;
using LeagueBroadcast.Trinket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace LeagueBroadcast.Ingame.Data.Provider
{
    class LiveEventsDataProvider
    {
        private readonly LiveEventConnector Trinket;
        private IngameController Ingame => BroadcastController.Instance.IGController;

        private const string EnableAPIString = "[LiveEvents]\nEnable=1\nPort=34243";
        private const string EnableReplayAPIString = "EnableReplayApi=1";

        public bool Connected => Trinket.Connected;

        public LiveEventsDataProvider()
        {
            //Check default path for league install. Prompt user if it cannot be found
            Log.Info("Checking League install for LiveEventsAPI");
            if (!ConfigController.Component.App.LeagueInstall.Any(InstallLocation => CheckGameConfigLocation(InstallLocation)))
            {

                //Prompt user for league install location
                Log.Warn("Game Location not found. Asking user to edit config");
                Log.WriteToFileAndPause();
                MessageBoxResult result = MessageBox.Show("Could not detect League Install. \nPlease manually add the folder containing 'Riot Games' to Config/Component.json\nStop League Broadcast now?", "League Broadcast", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        App.Instance.Shutdown();
                    });
                }
                Log.Resume();
            }

            Trinket = new LiveEventConnector();
            Trinket.OnLiveEvent += ReceiveLiveEvent;
            Trinket.OnConnect += (s, e) => { Log.Info("LiveEventAPI Connected"); };
            Trinket.OnConnectionError += (s, e) => { Log.Info("LiveEventAPI Connection Failed!"); Log.Warn(e); };

            AppStateController.GameLoad += (s, e) => { Trinket.Connect(); };
            AppStateController.GameStop += (s, e) => { Trinket.Disconnect(); Log.Info("LiveEventAPI Closed"); };
        }

        public void Connect()
        {
            Trinket.Connect();
        }

        private bool CheckGameConfigLocation(string configLocation)
        {
            //Make sure the folder exists
            if(!Directory.Exists(configLocation))
                return false;
            try
            {
                //Check for config file in given folder
                List<string> files = Directory.GetFiles(configLocation).Select(f => Path.GetFileName(f)).ToList();

                //Check for config folder in given folder
                List<string> folders = Directory.GetDirectories(configLocation).Select(f => f = f.Replace(configLocation, "").Remove(0, 1)).ToList();

                //Determine which to use depending on location of game.cfg
                string LeagueFolder = folders.Contains("Config") && configLocation.EndsWith("League of Legends")
                    ? Path.Combine(configLocation, "Config")
                    : files.Contains("game.cfg")
                        ? configLocation
                        : Path.Join(Path.Join(Path.Join(configLocation, "Riot Games"), "League of Legends"), "Config");

                if (Directory.Exists(LeagueFolder))
                {
                    Log.Info("Found League install location");
                    string cfgContent = "";
                    try
                    {
                        cfgContent = File.ReadAllText(Path.Join(LeagueFolder, "game.cfg"));
                    }
                    catch
                    {
                        Log.Warn("Could not find config file in config folder");
                        return false;
                    }
                    //Check for Live Events
                    if (!(cfgContent.Contains("[LiveEvents]") && cfgContent.Contains("Enable=1")))
                    {
                        Log.Info("Could not find LiveEvents in game config. Appending to end");
                        var writer = File.AppendText(Path.Join(LeagueFolder, "game.cfg"));
                        writer.Write($"\n\n{EnableAPIString}");
                        writer.Close();
                        Log.Info("Updated Game Config");
                    }
                    else
                    {
                        Log.Info("LiveEvents API found in Game config");
                    }

                    //Check for Replay API. This shouldnt be here but i'll shoehorn it in since it works
                    List<string> lines = cfgContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                    int generalLoc = lines.FindIndex(0, l => l == "[General]");

                    //found general config
                    if (generalLoc != -1)
                    {
                        int ReplayAPILineLoc = lines.FindIndex(l => l.StartsWith("EnableReplayApi"));
                        bool Overwrite = false;

                        //Replay API Line does not exist
                        if (ReplayAPILineLoc == -1)
                        {
                            Log.Verbose("Could not find Replay API in config");
                            lines.Insert(generalLoc + 2, EnableReplayAPIString);
                            Overwrite = true;
                        }
                        //Replay API disabled
                        else if (lines[ReplayAPILineLoc].Contains("0"))
                        {
                            Log.Info("Replay API has been manually disabled. Reenabling");
                            lines[ReplayAPILineLoc] = EnableReplayAPIString;
                            Overwrite = true;
                        }

                        if (Overwrite)
                        {
                            Log.Info("Enabling Replay API in Game config");
                            File.WriteAllLines(Path.Join(LeagueFolder, "game.cfg"), lines);
                            Log.Verbose("Replay API enabled");
                        }
                        else
                        {
                            //Replay API Enabled
                            Log.Info("Replay API found in Game config");
                        }
                    }
                    else
                    {
                        Log.Warn("Could not parse game config. Replay API may not be enabled!");
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
                    } catch(Exception e)
                    {
                        Log.Warn($"Error Parsing LiveEvents.ini:\n{e.Source} -> {e.Message}\n Stacktrace:\n{e.StackTrace}");
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            } catch (Exception e)
            {
                Log.Warn($"Could not read config in folder {configLocation}: {e}");
                return false;
            }
        }

        private void ReceiveLiveEvent(object sender, LiveEvent e)
        {
            try
            {
                if (e.SourceTeam == null || e.SourceTeam.Equals("Neutral"))
                {
                    return;
                }
                switch (e.EventName)
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
            catch (NullReferenceException ex)
            {
                Log.Warn($"Could not decode live event:\n{JsonConvert.SerializeObject(e)}");
            }

        }

        private void OnMinionKill(LiveEvent e)
        {
            Player p = GetPlayer(e);
            
            if (p != null)
            {
                if(e.Other.StartsWith("Yorick"))
                {
                    return;
                }
                AddCS(p, 1);
            }
                
        }
        private void OnJglMinionKill(LiveEvent e)
        {
            Player p = GetPlayer(e);
            
            if (p != null)
            {
                if (e.Other.StartsWith("SRU_MurkwolfMini"))
                {
                    AddCS(p, 1);
                    return;
                }

                //Just count the entire camp instead of trying to somehow predict which mini give cs and which dont
                //Maybe using statistics also would somehow work, but it can't be much more accurate than this and is far more effort
                if((e.Other.StartsWith("SRU_Razorbeak") || e.Other.StartsWith("SRU_Krug")) && !e.Other.Contains("Mini"))
                {
                    AddCS(p, 4);
                    return;
                }

                if (e.Other.StartsWith("SRU_Murkwolf"))
                {
                    AddCS(p, 2);
                    return;
                }

                //Do not count mini monsters
                //Because Senna Souls and GP Barrels are both Barrels.... and neutral minions. Riot... get help
                if (e.Other.Contains("Mini") || e.Other.Equals("Barrel"))
                {
                    return;
                }

                //Scuttle, Herald, Baron, Drake, Blue, Red all give 4 cs
                AddCS(p, 4);

                if (e.Other.StartsWith("SRU_Dragon"))
                {

                    //Convert types to API friendly names
                    string type = e.Other.Remove(0, 11).Replace("Air", "Cloud", StringComparison.OrdinalIgnoreCase).Replace("Earth", "Mountain", StringComparison.OrdinalIgnoreCase).Replace("Water", "Ocean", StringComparison.OrdinalIgnoreCase);


                    IngameController.DragonTaken.Invoke(this, new ObjectiveTakenArgs(type, GetTeam(e), Ingame.gameData.gameTime));
                    return;
                }
                if (e.Other.StartsWith("SRU_Baron"))
                {
                    Team t = GetTeam(e);
                    t.hasBaron = true;
                    t.players.ForEach(p => p.diedDuringBaron = false);
                    IngameController.BaronTaken.Invoke(this, new ObjectiveTakenArgs("Baron", t, Ingame.gameData.gameTime));
                    return;
                }

                if(e.Other.StartsWith("SRU_RiftHerald"))
                {
                    Team t = GetTeam(e);
                    IngameController.HeraldTaken.Invoke(this, new ObjectiveTakenArgs("Herald", GetTeam(e), Ingame.gameData.gameTime));
                    return;
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
            if (Ingame.gameState.blueTeam == null || Ingame.gameState.redTeam == null)
                return null;
            return Ingame.gameState.GetTeam(e.SourceTeam).players.SingleOrDefault(p => p.summonerName.Equals(e.Source, StringComparison.OrdinalIgnoreCase));
        }

        private Player GetPlayer(LiveEvent e, Team t)
        {
            if (t == null)
                return null;
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
