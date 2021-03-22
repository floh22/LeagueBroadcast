using LeagueBroadcastHub.Data;
using LeagueBroadcastHub.Data.Containers;
using LeagueBroadcastHub.Events;
using LeagueBroadcastHub.Events.FrontendEvents;
using LeagueBroadcastHub.Events.RiotEvents;
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
    class GameController
    {
        public LeagueDataProvider LoLDataProvider;
        public OCRDataProvider OCRDataProvider;

        public State.State gameState;
        public GameMetaData gameData;

        //Events
        public static bool DoPlayerLevelUp;

        public static bool DoItemCompleted;

        public static bool DoElderKill;

        public static bool DoBaronKill;

        public GameController()
        {
            this.LoLDataProvider = new LeagueDataProvider();
            this.OCRDataProvider = new OCRDataProvider();

            LoLDataProvider.Init();

            this.gameState = new State.State(this);
            this.gameData = new GameMetaData();

            LeagueIngameController.gameStop += OnGameStop;

            LoadSettings();
        }

        public void SetGameData(GameMetaData gameData)
        {
            this.gameData = gameData;
        }

        public void InitGameState()
        {
            this.gameState.ResetState();
            EmbedIOServer.socketServer.SendEventToAllAsync(new GameStart());
        }

        public void DoTick()
        {
            //Check if ingame and get game meta data
            var newGameData = LoLDataProvider.GetGameData().Result;
            if (newGameData == null || LeagueIngameController.currentlyIngame == false)
            {
                
                LeagueIngameController.currentlyIngame = false;
                LeagueIngameController.gameStop.Invoke(this, EventArgs.Empty);
                EmbedIOServer.socketServer.SendEventToAllAsync(new GameEnd());
                Logging.Info("Game ended ");
                return;
            }

            double timeDiff = newGameData.gameTime - gameData.gameTime;

            //Update game state
            if(gameData.gameTime == newGameData.gameTime)
            {
                if(!gameState.stateData.gamePaused)
                {
                    gameState.stateData.gamePaused = true;
                    LeagueIngameController.paused = true;
                    Logging.Info("Game Paused");
                    EmbedIOServer.socketServer.SendEventToAllAsync(new GamePause(gameData.gameTime));
                }
                
                return;
            }
            if(gameState.stateData.gamePaused)
            {
                gameState.stateData.gamePaused = false;
                LeagueIngameController.paused = false;
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
                if(backDrake.DurationRemaining - timeDiff > 150)
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
                gameState.UpdateEvents(LoLDataProvider.GetEventData().Result, new List<Data.Containers.Objectives.Objective>());
                gameState.UpdateTeams(LoLDataProvider.GetPlayerData().Result, new List<Data.Containers.OCRTeam>());
            } catch(Exception canceled)
            {
                Logging.Warn(canceled.Message);
            }
            
            //Update frontend
            EmbedIOServer.socketServer.SendEventToAllAsync(new HeartbeatEvent(gameState.stateData));
        }

        private void LoadSettings()
        {
            Logging.Verbose("Settings loaded");
            DoPlayerLevelUp = Properties.Settings.Default.doLevelUp;
            DoItemCompleted = Properties.Settings.Default.doItemsCompleted;
            DoBaronKill = Properties.Settings.Default.doBaronKill;
            DoElderKill = Properties.Settings.Default.doElderKill;
        }

        public virtual void OnLevelUp(LevelUpEventArgs e)
        {
            if (!DoPlayerLevelUp)
                return;
            Logging.Info("Player " + e.playerId + " lvl up");
            EmbedIOServer.socketServer.SendEventToAllAsync(new PlayerLevelUp(e.playerId, e.level));
        }

        public virtual void OnItemCompleted(ItemCompletedEventArgs e)
        {
            if (!DoPlayerLevelUp)
                return;
            Logging.Info("Player " + e.playerId + " finished Item " + e.itemData.itemID);
            EmbedIOServer.socketServer.SendEventToAllAsync(new ItemCompleted(e.playerId, e.itemData));
        }

        public virtual void OnElderKilled()
        {
            if (!DoElderKill)
                return;
            EmbedIOServer.socketServer.SendEventToAllAsync(new ObjectiveKilled("elder", -1));
        }

        public virtual void OnElderDespawn()
        {
            if (!DoElderKill)
                return;

            Logging.Info("Elder despawned");
            EmbedIOServer.socketServer.SendEventToAllAsync(new BuffDespawn("elder", -1));
        }

        public virtual void OnBaronKilled()
        {
            if (!DoBaronKill)
                return;
            
            EmbedIOServer.socketServer.SendEventToAllAsync(new ObjectiveKilled("baron", 1));
        }

        public virtual void OnBaronDespawn()
        {
            if (!DoBaronKill)
                return;
            Logging.Info("Baron despawned");
            EmbedIOServer.socketServer.SendEventToAllAsync(new BuffDespawn("baron", -1));
        }

        private void OnGameStop(object sender, EventArgs e)
        {
            GameInfoPage.ClearPlayers();

            OnBaronDespawn();
            OnElderDespawn();
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

}
