using LeagueBroadcast.Http;
using LeagueBroadcast.Ingame.Data.Provider;
using LeagueBroadcast.Ingame.Data.RIOT;
using LeagueBroadcast.Ingame.Events;
using LeagueBroadcast.Ingame.State;
using LeagueBroadcast.OperatingSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LeagueBroadcast.Common.Controllers
{
    class IngameController : ITickable
    {
        public static IngameDataProvider LoLDataProvider = new();
        public static CurrentSettings CurrentSettings = new();
        public static Process? LeagueProcess;
        public static bool IsPaused;

        public State gameState;
        public GameMetaData gameData;

        private static string TargetProcessName = "League of Legends";
        private static ProcessEventWatcher ProcessEventWatcher { get; } = new ProcessEventWatcher();
        private static bool GameFound = false;

        public void DoTick()
        {
            throw new NotImplementedException();
        }

        public IngameController()
        {
            LoLDataProvider = new();

            this.gameState = new State(this);
            this.gameData = new GameMetaData();

            AppStateController.GameStop += OnGameStop;

            StartWaitingForTargetProcess();
        }

        public void EnterIngame(object sender, EventArgs e)
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

        private void OnGameStop(object sender, EventArgs e)
        {
            BroadcastController.CurrentLeagueState = "None";
            //GameInfoPage.ClearPlayers();
            BroadcastController.Instance.ToTick.Remove(this);
            //OnBaronDespawn();
            //OnElderDespawn();
            //EmbedIOServer.socketServer.SendEventToAllAsync(new HeartbeatEvent(gameState.stateData));
            //EmbedIOServer.socketServer.SendEventToAllAsync(new GameEnd());
            Log.Info("Game ended");
        }

        public void InitGameState()
        {
            //this.gameState.ResetState();
            GameFound = false;
        }

        //Following adapted from https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff/App.xaml.cs
        private void StartWaitingForTargetProcess()
        {
            ProcessEventWatcher.ProcessStarted += ProcessEventWatcher_OnProcessStarted;
            ProcessEventWatcher.ProcessStopped += ProcessEventWatcher_OnProcessStopped;

            var processes = Process.GetProcessesByName(TargetProcessName);
            if (processes.Length > 0)
            {
                LeagueProcess = processes[0];
                TargetProcessStarted();
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
                if (!newProcess.ProcessName.Equals(TargetProcessName))
                {
                    return;
                }

                LeagueProcess = newProcess;
                TargetProcessStarted();
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

        private void TargetProcessStarted()
        {
            Log.Info($"Target process ({TargetProcessName}) detected.");
            AppStateController.GameStart?.Invoke(this, EventArgs.Empty);
        }

        private void TargetProcessStopped()
        {
            AppStateController.GameStop?.Invoke(this, EventArgs.Empty);
        }

        public void OnExit()
        {
            ProcessEventWatcher.Dispose();
        }
    }

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

    public class CurrentSettings
    {
        public bool Baron;
        public bool SendBaron;
        public bool Elder;
        public bool SendElder;
        public bool Items;
        public bool LevelUp;
        public bool GoldGraph;
        public bool TeamNames;
        public bool TeamStats;
        public bool Inhibs;
        public bool CS;
    }
}
