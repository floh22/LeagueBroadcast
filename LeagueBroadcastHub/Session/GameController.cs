using LeagueBroadcastHub.Data;
using LeagueBroadcastHub.Data.Game.Containers;
using LeagueBroadcastHub.Data.Game.Containers.Objectives;
using LeagueBroadcastHub.Data.Game.RiotContainers;
using LeagueBroadcastHub.Data.Provider;
using LeagueBroadcastHub.Events.Game.FrontendEvents;
using LeagueBroadcastHub.Events.Game.RiotEvents;
using LeagueBroadcastHub.Log;
using LeagueBroadcastHub.Pages.ControlPages;
using LeagueBroadcastHub.Server;
using LeagueBroadcastHub.State;
using LeagueIngameServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueBroadcastHub.Session
{
    class GameController : ITickable
    {
        public LeagueDataProvider LoLDataProvider;
        public OCRDataProvider OCRDataProvider;

        public State.Game.State gameState;
        public GameMetaData gameData;

        private bool GameFound = false;

        //Events
        public static CurrentSettings CurrentSettings = new CurrentSettings();

        public static bool IsPaused;

        public GameController()
        {
            this.LoLDataProvider = new LeagueDataProvider();
            this.OCRDataProvider = new OCRDataProvider();

            LoLDataProvider.Init();

            this.gameState = new State.Game.State(this);
            this.gameData = new GameMetaData();

            StateController.GameStop += OnGameStop;

            LoadSettings();
        }

        public void InitGameState()
        {
            this.gameState.ResetState();
            this.GameFound = false;
        }

        public async void DoTick()
        {
            //Check if ingame and get game meta data
            var newGameData = await LoLDataProvider.GetGameData();

            //Discard late rejected responses by API
            if (BroadcastHubController.CurrentLeagueState != "InProgress")
                return;

            //Wait until the game has been found
            if (!GameFound)
            {
                if(newGameData == null)
                    return;
                GameFound = true;
                gameData = newGameData;
                gameData.gameTime = 0;
                EmbedIOServer.socketServer.SendEventToAllAsync(new GameStart());
            }
            
            if (newGameData == null )
            {
                Logging.Verbose("Game exited early");
                
                StateController.GameStop.Invoke(this, EventArgs.Empty);
                return;
            }

            double timeDiff = newGameData.gameTime - gameData.gameTime;

            //Update game state
            if(gameData.gameTime == newGameData.gameTime)
            {
                if(!gameState.stateData.gamePaused)
                {
                    gameState.stateData.gamePaused = true;
                    IsPaused = true;
                    Logging.Info("Game Paused");
                    EmbedIOServer.socketServer.SendEventToAllAsync(new GamePause(gameData.gameTime));
                }
                
                return;
            }
            if(gameState.stateData.gamePaused)
            {
                gameState.stateData.gamePaused = false;
                IsPaused = false;
                Logging.Info("Game Unpaused");
                EmbedIOServer.socketServer.SendEventToAllAsync(new GameUnpause(gameData.gameTime));
            }

            //If time was scrolled back, remove old events
            var backDrake = gameState.backEndData.dragon;
            var backBaron = gameState.backEndData.baron;
            if (newGameData.gameTime < gameData.gameTime && gameState.pastIngameEvents.Count != 0)
            {
                Logging.Info("Scrolled back in timeline, reverting state");
                gameState.pastIngameEvents = gameState.pastIngameEvents.Where((e) => e.EventTime < newGameData.gameTime).ToList();
                gameState.blueTeam.goldHistory = gameState.blueTeam.goldHistory.Where((e) => e.Key < newGameData.gameTime).ToDictionary(v => v.Key, v => v.Value);
                gameState.redTeam.goldHistory = gameState.redTeam.goldHistory.Where((e) => e.Key < newGameData.gameTime).ToDictionary(v => v.Key, v => v.Value);
                if (backDrake.DurationRemaining - timeDiff > 150)
                {
                    backDrake.DurationRemaining = 0;
                    gameState.blueTeam.hasElder = false;
                    gameState.redTeam.hasElder = false;
                    OnElderDespawn();
                }
                if(backBaron.DurationRemaining - timeDiff > 180)
                {
                    backBaron.DurationRemaining = 0;
                    gameState.blueTeam.hasBaron = false;
                    gameState.redTeam.hasBaron = false;
                    gameState.GetAllPlayers().ForEach(p => p.diedDuringBaron = false);
                    OnBaronDespawn();
                }
            }


            //Update remaining time for objectives incase either team has them
           
            if (backDrake.DurationRemaining > 0)
            {
                gameState.SetObjectiveData(backDrake, gameState.stateData.dragon, backDrake.DurationRemaining - timeDiff);
                Logging.Verbose($"Elder Time left: {backDrake.DurationRemaining}");
                if (backDrake.DurationRemaining <= 0)
                {
                    var tID = gameState.blueTeam.hasElder ? 0 : 1;
                    OnElderDespawn();
                    gameState.blueTeam.hasElder = gameState.redTeam.hasElder = false;
                    gameState.GetAllPlayers().ForEach(p => p.diedDuringElder = false);
                }
            }

            
            if (backBaron.DurationRemaining > 0)
            {
                gameState.SetObjectiveData(backBaron, gameState.stateData.baron, backBaron.DurationRemaining - timeDiff);
                Logging.Verbose($"Baron Time left: {backBaron.DurationRemaining}");
                if (backBaron.DurationRemaining <= 0)
                {
                    backBaron.DurationRemaining = 0;
                    var tID = gameState.blueTeam.hasBaron ? 0 : 1;
                    OnBaronDespawn();
                    gameState.blueTeam.hasBaron = gameState.redTeam.hasBaron = false;
                    gameState.GetAllPlayers().ForEach(p => p.diedDuringBaron = false);
                }
            }

            gameData = newGameData;
            gameState.stateData.gameTime = gameData.gameTime;

            //Make distinction if we are using OCR or not since that has an impact on what kind of information is available
            if (ActiveSettings._useOCR)
            {
                var res = OCRDataProvider.GetObjectiveData().Result;
                if (res != null)
                {
                    var eventData = LoLDataProvider.GetEventData().Result;
                    gameState.UpdateEvents(eventData, res);
                    gameState.UpdateTeams(LoLDataProvider.GetPlayerData().Result, OCRDataProvider.GetTeamData().Result.ToList());
                    //Update frontend
                    EmbedIOServer.socketServer.SendEventToAllAsync(new HeartbeatEvent(gameState.stateData));
                    return;
                } else
                {
                    Logging.Verbose("OCR active but no response");
                }
            }
            try
            {
                gameState.UpdateEvents(LoLDataProvider.GetEventData().Result, new List<Objective>());
                gameState.UpdateTeams(LoLDataProvider.GetPlayerData().Result, new List<OCRTeam>());
            } catch(Exception canceled)
            {
                Logging.Warn(canceled.Message);
            }
            
            //Update frontend
            EmbedIOServer.socketServer.SendEventToAllAsync(new HeartbeatEvent(gameState.stateData));
        }

        private void LoadSettings()
        {
            Logging.Verbose("Loading Ingame Settings");
            CurrentSettings.LevelUp = Properties.Settings.Default.doLevelUp;
            CurrentSettings.Items = Properties.Settings.Default.doItemsCompleted;
            CurrentSettings.Baron = Properties.Settings.Default.doBaronKill;
            CurrentSettings.Elder = Properties.Settings.Default.doElderKill;
            Logging.Verbose("Ingame Settings loaded");
        }

        public virtual void OnLevelUp(LevelUpEventArgs e)
        {
            if (!CurrentSettings.LevelUp)
                return;
            Logging.Info("Player " + e.playerId + " lvl up");
            EmbedIOServer.socketServer.SendEventToAllAsync(new PlayerLevelUp(e.playerId, e.level));
        }

        public virtual void OnItemCompleted(ItemCompletedEventArgs e)
        {
            if (!CurrentSettings.LevelUp)
                return;
            Logging.Info("Player " + e.playerId + " finished Item " + e.itemData.itemID);
            EmbedIOServer.socketServer.SendEventToAllAsync(new ItemCompleted(e.playerId, e.itemData));
        }

        public virtual void OnElderKilled()
        {
            if (!CurrentSettings.Elder)
                return;
            //EmbedIOServer.socketServer.SendEventToAllAsync(new ObjectiveKilled("elder", -1));
            CurrentSettings.SendElder = true;
        }

        public virtual void OnElderDespawn()
        {
            if (!CurrentSettings.Elder)
                return;

            Logging.Info("Elder despawned");
            //EmbedIOServer.socketServer.SendEventToAllAsync(new BuffDespawn("elder", -1));
            CurrentSettings.SendElder = false;
        }

        public virtual void OnBaronKilled()
        {
            if (!CurrentSettings.Baron)
                return;

            //EmbedIOServer.socketServer.SendEventToAllAsync(new ObjectiveKilled("baron", 1));
            CurrentSettings.SendBaron = true;
        }

        public virtual void OnBaronDespawn()
        {
            if (!CurrentSettings.Baron)
                return;
            Logging.Info("Baron despawned");
            //EmbedIOServer.socketServer.SendEventToAllAsync(new BuffDespawn("baron", -1));
            CurrentSettings.SendBaron = false;
        }

        private void OnGameStop(object sender, EventArgs e)
        {
            BroadcastHubController.CurrentLeagueState = "None";
            GameInfoPage.ClearPlayers();
            BroadcastHubController.ToTick.Remove(this);
            OnBaronDespawn();
            OnElderDespawn();
            EmbedIOServer.socketServer.SendEventToAllAsync(new HeartbeatEvent(gameState.stateData));
            EmbedIOServer.socketServer.SendEventToAllAsync(new GameEnd());
            Logging.Info("Game ended");
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
