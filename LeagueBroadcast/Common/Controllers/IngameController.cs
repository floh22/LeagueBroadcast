using LeagueBroadcast.Common.Data.RIOT;
using LeagueBroadcast.Http;
using LeagueBroadcast.Ingame.Data.LBH;
using LeagueBroadcast.Ingame.Data.Provider;
using LeagueBroadcast.Ingame.Data.RIOT;
using LeagueBroadcast.Ingame.Events;
using LeagueBroadcast.Ingame.State;
using LeagueBroadcast.MVVM.ViewModel;
using LeagueBroadcast.OperatingSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LeagueBroadcast.Common.Controllers
{
    class IngameController : ITickable
    {
        public static IngameDataProvider LoLDataProvider = new();
        public static LiveEventsDataProvider LiveEventsProvider;
        public static CurrentSettings CurrentSettings = new();
#nullable enable
        public static Process? LeagueProcess;
#nullable disable
        public static bool IsPaused;

        public State gameState;
        public GameMetaData gameData;

        private static string TargetProcessName = "League of Legends";
        private static ProcessEventWatcher ProcessEventWatcher { get; } = new ProcessEventWatcher();
        private static bool GameFound = false;

        public static EventHandler BaronEnd, DragonEnd;
        public static EventHandler<ObjectiveTakenArgs> DragonTaken, BaronTaken, HeraldTaken;

        public IngameController()
        {
            this.gameState = new State(this);
            this.gameData = new GameMetaData();
            LiveEventsProvider = new();

            AppStateController.GameStop += OnGameStop;

            BaronTaken += OnBaronTaken;
            BaronEnd += OnBaronEnd;
            DragonTaken += OnDragonTaken;
            DragonEnd += OnDragonEnd;
            HeraldTaken += OnHeraldTaken;
            
        }

        public async void DoTick()
        {
            //Check if ingame and get game meta data
            var newGameData = await LoLDataProvider.GetGameData();

            //Discard late rejected responses by API
            if (!BroadcastController.CurrentLeagueState.HasFlag(LeagueState.InProgress) || newGameData == null || LeagueProcess == null)
            {
                Log.Verbose("game reponse invalid and discarded");
                return;
            }
                

            //Wait until the game has been found
            if (!GameFound)
            {
                GameFound = true;
                if(!await LoLDataProvider.IsSpectatorGame())
                {
                    Log.Warn("LeagueBroadcast not enabled in live game. Stopping game connection");
                    AppStateController.GameStop.Invoke(null, EventArgs.Empty);
                    return;
                }
                LoadGame(newGameData);
            }

            #region GameTime
            //Check if game is paused/unpaused
            double timeDiff = newGameData.gameTime - gameData.gameTime;
            Log.Verbose($"Tick: {newGameData.gameTime}. Update duration: {timeDiff}");

            if(timeDiff == 0 || newGameData.gameTime == 0)
            {
                if(!gameState.stateData.gamePaused)
                {
                    SetGamePauseState(true);
                }
                return;
            }
            if(gameState.stateData.gamePaused)
            {
                SetGamePauseState(false);
                //LiveEvents could not connect because game was paused too long
                //Reconnect when it is once again accessible because the game has continued
                if(!LiveEventsProvider.Connected)
                {
                    LiveEventsProvider.Connect();
                }
            }
            var backDragon = gameState.stateData.backDragon;
            var backBaron = gameState.stateData.backBaron;


            //Update objective timers - Drake not super useful since I cant get type before it spawns
            gameState.stateData.dragon.SpawnTimer = Math.Max(0, gameState.stateData.dragon.SpawnTimer - timeDiff);
            double newBaronTimer = Math.Min(1200, Math.Max(0, gameState.stateData.baron.SpawnTimer - timeDiff));

            //Check for time scrolled back
            if (newGameData.gameTime < gameData.gameTime && gameState.pastIngameEvents.Count != 0)
            {
                Log.Info("Scrolled back in timeline, reverting state");
                Log.Info(newGameData.gameTime);
                gameState.pastIngameEvents = gameState.pastIngameEvents.Where((e) => e.EventTime < gameData.gameTime).ToList();
                Log.Info("Rolling back Players");
                gameState.GetAllPlayers().ForEach(p => {

                    //Roll back gold history
                    p.goldHistory = p.goldHistory
                    .Where(pair => pair.Key <= gameData.gameTime)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                    //Roll back cs history
                    p.csHistory = p.csHistory
                    .Where(pair => pair.Key < gameData.gameTime)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                    p.scores.creepScore = p.csHistory.Last().Value;
                });

                Log.Info("Rolling back Objectives");
                //Check if Elder died in roll back period
                if (backDragon.DurationRemaining - timeDiff > 150)
                {
                    backDragon.DurationRemaining = 0;
                    gameState.blueTeam.hasElder = false;
                    gameState.redTeam.hasElder = false;
                    OnDragonEnd(null, EventArgs.Empty);
                }

                //Check if Baron died in roll back period
                if (backBaron.DurationRemaining - timeDiff > 180)
                {
                    backBaron.DurationRemaining = 0;
                    gameState.blueTeam.hasBaron = false;
                    gameState.redTeam.hasBaron = false;
                    gameState.GetAllPlayers().ForEach(p => p.diedDuringBaron = false);
                    OnBaronEnd(null, EventArgs.Empty);
                }

                var drakesTaken = gameState.pastIngameEvents.Where(e => e.EventName == "ObjectiveKilled" && ((ObjectiveKilled)e).ObjectiveName == "Dragon");
                int blueDrakesTaken = drakesTaken.Count(e => ((ObjectiveKilled)e).TeamName.Equals("Order", StringComparison.OrdinalIgnoreCase));
                int redDrakesTaken = drakesTaken.Count(e => ((ObjectiveKilled)e).TeamName.Equals("Chaos", StringComparison.OrdinalIgnoreCase));

                //Remove all drakes taken after this point in time
                gameState.blueTeam.dragonsTaken.RemoveRange(blueDrakesTaken, gameState.blueTeam.dragonsTaken.Count - blueDrakesTaken);
                gameState.redTeam.dragonsTaken.RemoveRange(redDrakesTaken, gameState.redTeam.dragonsTaken.Count - redDrakesTaken);


                var lastBaronTake = gameState.pastIngameEvents.LastOrDefault(e => e.EventName == "ObjectiveKilled" && ((ObjectiveKilled)e).ObjectiveName == "Baron");
                if(lastBaronTake != null)
                {
                    //7 Minutes minus the time from the last baron take to now
                    gameState.stateData.baron.SpawnTimer = Math.Max(0, 420 - (lastBaronTake.EventTime - newGameData.gameTime));
                }
                else
                {
                    //Baron was never taken, so 20 minute spawn timer minus current game time
                    gameState.stateData.baron.SpawnTimer = Math.Max(0, 1200 - newGameData.gameTime);
                }


                var lastDragonTake = gameState.pastIngameEvents.LastOrDefault(e => e.EventName == "ObjectiveKilled" && ((ObjectiveKilled)e).ObjectiveName == "Dragon");
                if (lastDragonTake != null)
                {
                    //5 or 6 minutes depending on if elder should spawn next minus the time from the last drake taken to now
                    gameState.stateData.dragon.SpawnTimer = blueDrakesTaken >= 4 || redDrakesTaken >= 4 ? Math.Max(0, 360 - (lastDragonTake.EventTime - newGameData.gameTime)) : Math.Max(0, 300 - (lastDragonTake.EventTime - newGameData.gameTime));
                }
                else
                {
                    //Dragon was never taken, so 5 minute spawn timer minus current game time
                    gameState.stateData.dragon.SpawnTimer = Math.Max(0, 300 - newGameData.gameTime);
                }
            }
            else
            {
                //Only do this if the game did not scroll back since we do not always want these events to trigger when conditions are met, but rather only at the exact point in time

                //Check for baron spawn
                if (gameState.stateData.baron.SpawnTimer > 0 && newBaronTimer == 0)
                {
                    OnObjectiveSpawn("Baron");

                }
                gameState.stateData.baron.SpawnTimer = newBaronTimer;
            }
            #endregion

            #region Objectives
            //Update remaining time for objectives incase either team has them

            if (backDragon.DurationRemaining > 0)
            {
                gameState.SetObjectiveData(backDragon, gameState.stateData.dragon, backDragon.DurationRemaining - timeDiff);
                Log.Verbose($"Elder Time left: {backDragon.DurationRemaining:C0}");
                if (backDragon.DurationRemaining <= 0)
                {
                    OnDragonEnd(null, EventArgs.Empty);
                    gameState.blueTeam.hasElder = gameState.redTeam.hasElder = false;
                    gameState.GetAllPlayers().ForEach(p => p.diedDuringElder = false);
                }
            }


            if (backBaron.DurationRemaining > 0)
            {
                gameState.SetObjectiveData(backBaron, gameState.stateData.baron, backBaron.DurationRemaining - timeDiff);
                Log.Verbose($"Baron Time left: {backBaron.DurationRemaining:C0}");
                if (backBaron.DurationRemaining <= 0)
                {
                    backBaron.DurationRemaining = 0;
                    OnBaronEnd(null, EventArgs.Empty);
                    gameState.blueTeam.hasBaron = gameState.redTeam.hasBaron = false;
                    gameState.GetAllPlayers().ForEach(p => p.diedDuringBaron = false);
                }
            }

            //Update inhibitors      
            gameState.stateData.inhibitors.Inhibitors.ForEach(inhib => {
                inhib.timeLeft = Math.Max(0, inhib.timeLeft - timeDiff);
                //Make sure to reset inhibs incase of scrollback
                if (inhib.timeLeft > 300)
                    inhib.timeLeft = 0;
            });

            #endregion

            //Update Meta Data
            gameData = newGameData;
            gameState.stateData.gameTime = gameData.gameTime;

            //Update State
            try
            {
                var snapshot = BroadcastController.Instance.MemoryController.CreateSnapshot(gameState.stateData.gameTime);
                gameState.UpdateEvents(LoLDataProvider.GetEventData().Result, snapshot);
                gameState.UpdateTeams(LoLDataProvider.GetPlayerData().Result, snapshot);
                gameState.UpdateScoreboard();
                UpdateInfoPage();
            }
            catch (Exception canceled)
            {
                Log.Warn($"Could not update State:\n{canceled.Source} -> {canceled.Message}\n Stacktrace:\n{canceled.StackTrace}");
            }

            //Update frontend
            EmbedIOServer.SocketServer.SendEventToAllAsync(new HeartbeatEvent(gameState.stateData));
        }

        private void UpdateInfoPage()
        {
            if(!CurrentSettings.SideGraph)
                return;

            if (CurrentSettings.EXP)
            {
                gameState.stateData.infoPage = new InfoSidePage("EXP per player", PlayerOrder.MaxToMin, PlayerTab.GetEXPTabs());
                return;
            }
            if(CurrentSettings.PlayerGold)
            {
                gameState.stateData.infoPage = new InfoSidePage("Player Gold", PlayerOrder.MaxToMin, PlayerTab.GetGoldTabs());
                return;
            }
            if(CurrentSettings.CSPerMin)
            {
                gameState.stateData.infoPage = new InfoSidePage("CS/min", PlayerOrder.MaxToMin, PlayerTab.GetCSPerMinTabs());
                return;
            }
        }

        private void LoadGame(GameMetaData gameData)
        {
            this.gameData = gameData;
            this.gameData.gameTime = 0;
            Log.Info("Game Loaded");
            EmbedIOServer.SocketServer.SendEventToAllAsync(new GameStart());
            AppStateController.GameLoad?.Invoke(this, EventArgs.Empty);
        }

        private void SetGamePauseState(bool PauseState)
        {
            gameState.stateData.gamePaused = PauseState;
            IsPaused = PauseState;
            Log.Info(PauseState? "Game Paused" : "Game Resumed");
            EmbedIOServer.SocketServer.SendEventToAllAsync(PauseState? new GameUnpause(gameData.gameTime): new GamePause(gameData.gameTime));
        }

        public void EnterIngame(object sender, Process p)
        {
            var Instance = BroadcastController.Instance;
            if(Instance.ToTick.Contains(this))
            {
                return;
            }
            Instance.ToTick.Add(this);
            FlagsHelper.Set( ref BroadcastController.CurrentLeagueState, LeagueState.InProgress);
            InitGameState();
        }

        public void InitGameState()
        {
            Log.Info("Init Game State");
            this.gameState.ResetState();
            GameFound = false;
        }

        #region Process
        //Following adapted from https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff/App.xaml.cs
        public void StartWaitingForTargetProcess()
        {
            ProcessEventWatcher.ProcessStarted += ProcessEventWatcher_OnProcessStarted;
            ProcessEventWatcher.ProcessStopped += ProcessEventWatcher_OnProcessStopped;

            var processes = Process.GetProcessesByName(TargetProcessName);
            if (processes.Length > 0)
            {
                LeagueProcess = processes[0];
                TargetProcessStarted(LeagueProcess);
            }
        }

        private void ProcessEventWatcher_OnProcessStarted(object sender, ProcessEventArguments e)
        {
            if (LeagueProcess != null)
            {
                return;
            }

            try
            {
                var newProcess = Process.GetProcessById(e.ProcessId);
                if (newProcess != null && !newProcess.ProcessName.Equals(TargetProcessName))
                {
                    return;
                }

                LeagueProcess = newProcess;
                TargetProcessStarted(LeagueProcess);
            }
            catch 
            {
                // ignored
            }
        }

        private void ProcessEventWatcher_OnProcessStopped(object sender, ProcessEventArguments e)
        {
            if (LeagueProcess == null || LeagueProcess.Id != e.ProcessId)
            {
                return;
            }

            LeagueProcess = null;
            TargetProcessStopped();
        }

        private void TargetProcessStarted(Process p)
        {
            Log.Info($"Target process ({TargetProcessName}) detected.");
            AppStateController.GameStart?.Invoke(this, p);
        }

        private void TargetProcessStopped()
        {
            AppStateController.GameStop?.Invoke(this, EventArgs.Empty);
        }

        public void OnExit()
        {
            ProcessEventWatcher.Dispose();
        }

        #endregion

        #region Events
        public void OnDragonTaken(object sender, ObjectiveTakenArgs e)
        {
            e.Team.dragonsTaken.Add(e.Type);

            //Check if the next drake is an elder spawn, and set spawn timer accordingly
            gameState.stateData.dragon.SpawnTimer = gameState.blueTeam.GetDragonsTaken() >= 4 || gameState.redTeam.GetDragonsTaken() >= 4 ? 360 : 300;

            if (e.Type.Equals("Elder", StringComparison.OrdinalIgnoreCase))
            {
                gameState.stateData.backDragon.TakeGameTime = gameData.gameTime;
                gameState.stateData.backDragon.BlueStartGold = gameState.blueTeam.GetGold(gameData.gameTime);
                gameState.stateData.backDragon.RedStartGold = gameState.redTeam.GetGold(gameData.gameTime);
                gameState.SetObjectiveData(gameState.stateData.backDragon, gameState.stateData.dragon, 150);
                e.Team.hasElder = true;
            }

            gameState.pastIngameEvents.Add(new ObjectiveKilled("Dragon", e.Team.teamName, gameData.gameTime));
            OnObjectiveKilled(e.Type, e.Team.teamName);
        }

        public void OnBaronTaken(object sender, ObjectiveTakenArgs e)
        {
            gameState.stateData.backBaron.TakeGameTime = gameData.gameTime;
            gameState.stateData.backBaron.BlueStartGold = gameState.blueTeam.GetGold(gameData.gameTime);
            gameState.stateData.backBaron.RedStartGold = gameState.redTeam.GetGold(gameData.gameTime);
            gameState.SetObjectiveData(gameState.stateData.backBaron, gameState.stateData.baron, 180);
            gameState.stateData.baron.SpawnTimer = 420;
            e.Team.hasBaron = true;

            gameState.pastIngameEvents.Add(new ObjectiveKilled("Baron", e.Team.teamName, gameData.gameTime));
            OnObjectiveKilled("Baron", e.Team.teamName);
        }

        public void OnHeraldTaken(object sender, ObjectiveTakenArgs e)
        {
            gameState.pastIngameEvents.Add(new ObjectiveKilled("Herald", e.Team.teamName, gameData.gameTime));
            OnObjectiveKilled("Herald", e.Team.teamName);
        }

        public void OnBaronEnd(object sender, EventArgs e)
        {
            Log.Info("Baron ended. Resetting baron status for all players");
            gameState.GetBothTeams().ForEach(t => t.players.ForEach(p => p.diedDuringBaron = false));
        }

        public void OnDragonEnd(object sender, EventArgs e)
        {
            Log.Info("Elder dragon ended. Resetting baron status for all players");
            gameState.GetBothTeams().ForEach(t => t.players.ForEach(p => p.diedDuringElder = false));
        }

        private void OnGameStop(object sender, EventArgs e)
        {
            FlagsHelper.Unset(ref BroadcastController.CurrentLeagueState, LeagueState.InProgress);
            //GameInfoPage.ClearPlayers();
            BroadcastController.Instance.ToTick.Remove(this);
            gameState.stateData.scoreboard.GameTime = -1;
            gameState.stateData.gameTime = -1;
            EmbedIOServer.SocketServer.SendEventToAllAsync(new HeartbeatEvent(gameState.stateData));
            EmbedIOServer.SocketServer.SendEventToAllAsync(new GameEnd());
            Log.Info("Game ended");
        }

        public void OnLevelUp(LevelUpEventArgs e)
        {
            if (!CurrentSettings.LevelUp)
                return;
            Log.Info("Player " + e.playerId + " lvl up");
            EmbedIOServer.SocketServer.SendEventToAllAsync(new PlayerLevelUp(e.playerId, e.level));
        }

        public void OnItemCompleted(ItemCompletedEventArgs e)
        {
            if (!CurrentSettings.LevelUp)
                return;
            Log.Info("Player " + e.playerId + " finished Item " + e.itemData.itemID);
            EmbedIOServer.SocketServer.SendEventToAllAsync(new ItemCompleted(e.playerId, e.itemData));
        }

        public void OnObjectiveSpawn(string objectiveName)
        {
            Log.Info($"{objectiveName} spawned");
            if (!CurrentSettings.ObjectiveSpawnPopUp)
                return;
            EmbedIOServer.SocketServer.SendEventToAllAsync(new ObjectiveSpawnSimple(objectiveName));
        }

        public void OnObjectiveKilled(string objectiveName, string teamName)
        {
            Log.Info($"{objectiveName} killed by {teamName}");
            if (!CurrentSettings.ObjectiveKillPopUp)
                return;
            EmbedIOServer.SocketServer.SendEventToAllAsync(new ObjectiveKilledSimple(objectiveName, teamName));
        }

        #endregion
    }

    #region EventArgs
    public class LevelUpEventArgs : EventArgs
    {
        public int playerId;
        public int level;

        public LevelUpEventArgs(int playerId, int level)
        {
            this.playerId = playerId;
            this.level = level;
        }
    }

    public class ItemCompletedEventArgs : EventArgs
    {
        public int playerId;
        public ItemData itemData;

        public ItemCompletedEventArgs(int playerId, ItemData itemData)
        {
            this.playerId = playerId;
            this.itemData = itemData;
        }
    }

    #endregion

    public class CurrentSettings
    {
        public bool Baron => ConfigController.Component.Ingame.Objectives.DoBaronKill;
        public bool Elder => ConfigController.Component.Ingame.Objectives.DoDragonKill;
        public bool Inhibs => ConfigController.Component.Ingame.Objectives.DoInhibitors;
        public bool Items => ConfigController.Component.Ingame.DoItemCompleted;
        public bool LevelUp => ConfigController.Component.Ingame.DoLevelUp;
        public bool TeamNames => ConfigController.Component.Ingame.Teams.DoTeamNames;
        public bool TeamIcons => ConfigController.Component.Ingame.Teams.DoTeamIcons;
        public bool TeamStats => ConfigController.Component.Ingame.Teams.DoTeamScores;

        public bool ObjectiveSpawnPopUp => ConfigController.Component.Ingame.Objectives.DoObjectiveSpawnPopUp;
        public bool ObjectiveKillPopUp => ConfigController.Component.Ingame.Objectives.DoObjectiveKillPopUp;

        public bool CS = false;
        public bool CSPerMin = false;
        public bool EXP = false;
        public bool GoldGraph = false;
        public bool PlayerGold = false;

        public bool SideGraph => CS || CSPerMin || EXP || PlayerGold;
    }
}
