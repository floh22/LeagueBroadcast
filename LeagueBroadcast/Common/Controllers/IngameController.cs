using LeagueBroadcast.Common.Data.RIOT;
using LeagueBroadcast.Http;
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
        public static EventHandler<ObjectiveTakenArgs> DragonTaken, BaronTaken;

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

            
        }

        public async void DoTick()
        {
            //Check if ingame and get game meta data
            var newGameData = await LoLDataProvider.GetGameData();

            //Discard late rejected responses by API
            if (BroadcastController.CurrentLeagueState != "InProgress" || newGameData == null || LeagueProcess == null)
                return;

            //Wait until the game has been found
            if (!GameFound)
            {
                if(!await LoLDataProvider.IsSpectatorGame())
                {
                    Log.Warn("Essence not enabled in live game");
                    AppStateController.GameStop.Invoke(null, EventArgs.Empty);
                    return;
                }
                LoadGame(newGameData);
            }

            #region GameTime
            //Check if game is paused/unpaused
            double timeDiff = newGameData.gameTime - gameData.gameTime;
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

            //Check for time scrolled back
            if (newGameData.gameTime < gameData.gameTime && gameState.pastIngameEvents.Count != 0)
            {
                Log.Info("Scrolled back in timeline, reverting state");
                Log.Info(newGameData.gameTime);
                gameState.pastIngameEvents = gameState.pastIngameEvents.Where((e) => e.EventTime < gameData.gameTime).ToList();
                Log.Verbose("Rolling back Players");
                gameState.GetAllPlayers().ForEach(p => {
                    //Roll back gold history
                    p.goldHistory = p.goldHistory
                    .Where(pair => pair.Key < gameData.gameTime)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                    //Set current cs to clostest old known value
                    var keys = new List<double>(p.csHistory.Keys);
                    var closestCsIndex = keys.BinarySearch(gameData.gameTime);
                    if(closestCsIndex != -1)
                        p.scores.creepScore = p.csHistory[closestCsIndex];

                    //Roll back cs history
                    p.csHistory = p.csHistory
                    .Where(pair => pair.Key < gameData.gameTime)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
                });

                Log.Verbose("Rolling back Objectives");
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
            }
            #endregion

            #region Objectives
            //Update remaining time for objectives incase either team has them

            if (backDragon.DurationRemaining > 0)
            {
                gameState.SetObjectiveData(backDragon, gameState.stateData.dragon, backDragon.DurationRemaining - timeDiff);
                Log.Verbose($"Elder Time left: {backDragon.DurationRemaining}");
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
                Log.Verbose($"Baron Time left: {backBaron.DurationRemaining}");
                if (backBaron.DurationRemaining <= 0)
                {
                    backBaron.DurationRemaining = 0;
                    OnBaronEnd(null, EventArgs.Empty);
                    gameState.blueTeam.hasBaron = gameState.redTeam.hasBaron = false;
                    gameState.GetAllPlayers().ForEach(p => p.diedDuringBaron = false);
                }
            }
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
            }
            catch (Exception canceled)
            {
                Log.Warn(canceled.Message);
            }

            //Update frontend
            EmbedIOServer.socketServer.SendEventToAllAsync(new HeartbeatEvent(gameState.stateData));
        }

        private void LoadGame(GameMetaData gameData)
        {
            GameFound = true;
            this.gameData = gameData;
            this.gameData.gameTime = 0;
            Log.Verbose("Game Loaded");
            EmbedIOServer.socketServer.SendEventToAllAsync(new GameStart());
            AppStateController.GameLoad?.Invoke(this, EventArgs.Empty);
        }

        private void SetGamePauseState(bool PauseState)
        {
            gameState.stateData.gamePaused = PauseState;
            IsPaused = PauseState;
            Log.Info(PauseState? "Game Paused" : "Game Resumed");
            EmbedIOServer.socketServer.SendEventToAllAsync(PauseState? new GameUnpause(gameData.gameTime): new GamePause(gameData.gameTime));
        }

        public void EnterIngame(object sender, Process p)
        {
            var Instance = BroadcastController.Instance;
            if(Instance.ToTick.Contains(this))
            {
                return;
            }
            Instance.ToTick.Add(this);
            if(BroadcastController.CurrentLeagueState == "ChampSelect")
            {
                Instance.ToTick.Remove(Instance.PBController);
            }
            BroadcastController.CurrentLeagueState = "InProgress";
            InitGameState();
        }

        public void InitGameState()
        {
            Log.Verbose("Init Game State");
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
            Log.Info($"{e.Type} Dragon Taken by {e.Team.teamName}");
            e.Team.dragonsTaken.Add(e.Type);
            if(e.Type.Equals("Elder", StringComparison.OrdinalIgnoreCase))
            {
                gameState.stateData.backDragon.TakeGameTime = gameData.gameTime;
                gameState.stateData.backDragon.BlueStartGold = gameState.blueTeam.GetGold();
                gameState.stateData.backDragon.RedStartGold = gameState.redTeam.GetGold();
                gameState.SetObjectiveData(gameState.stateData.backDragon, gameState.stateData.dragon, 150);
                e.Team.hasElder = true;
            }
        }

        public void OnBaronTaken(object sender, ObjectiveTakenArgs e)
        {
            Log.Info($"Baron Taken by {e.Team.teamName}");
            gameState.stateData.backBaron.TakeGameTime = gameData.gameTime;
            gameState.stateData.backBaron.BlueStartGold = gameState.blueTeam.GetGold();
            gameState.stateData.backBaron.RedStartGold = gameState.redTeam.GetGold();
            gameState.SetObjectiveData(gameState.stateData.backBaron, gameState.stateData.baron, 180);
            e.Team.hasBaron = true;
        }

        public void OnBaronEnd(object sender, EventArgs e)
        {
            gameState.GetBothTeams().ForEach(t => t.players.ForEach(p => p.diedDuringBaron = false));
        }

        public void OnDragonEnd(object sender, EventArgs e)
        {
            gameState.GetBothTeams().ForEach(t => t.players.ForEach(p => p.diedDuringElder = false));
        }

        private void OnGameStop(object sender, EventArgs e)
        {
            BroadcastController.CurrentLeagueState = "None";
            //GameInfoPage.ClearPlayers();
            BroadcastController.Instance.ToTick.Remove(this);
            EmbedIOServer.socketServer.SendEventToAllAsync(new HeartbeatEvent(gameState.stateData));
            EmbedIOServer.socketServer.SendEventToAllAsync(new GameEnd());
            Log.Info("Game ended");
        }

        public void OnLevelUp(LevelUpEventArgs e)
        {
            if (!CurrentSettings.LevelUp)
                return;
            Log.Info("Player " + e.playerId + " lvl up");
            EmbedIOServer.socketServer.SendEventToAllAsync(new PlayerLevelUp(e.playerId, e.level));
        }

        public void OnItemCompleted(ItemCompletedEventArgs e)
        {
            if (!CurrentSettings.LevelUp)
                return;
            Log.Info("Player " + e.playerId + " finished Item " + e.itemData.itemID);
            EmbedIOServer.socketServer.SendEventToAllAsync(new ItemCompleted(e.playerId, e.itemData));
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
        
        public bool CS = false;
        public bool CSPerMin = false;
        public bool EXP = false;
        public bool GoldGraph = false;
        public bool PlayerGold = false;
    }
}
