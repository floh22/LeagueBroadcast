﻿using LeagueBroadcast.ChampSelect.Data.DTO;
using LeagueBroadcast.ChampSelect.Data.LCU;
using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.DTO;
using LeagueBroadcast.Common.Data.Provider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueBroadcast.ChampSelect.StateInfo
{
    class Converter
    {

        public static Team ConvertTeam(ConversionInput kwargs)
        {
            Team newTeam = new();
            List<Pick> picks = new();
            kwargs.team.ForEach(cell => {
                var currentAction = kwargs.actions.Where(action => !action.completed).FirstOrDefault();
                var pick = new Pick(cell.cellId);

                var spell1 = DataDragon.Instance.GetSummonerById(cell.spell1Id);
                pick.spell1 = new SummonerSpell() { ID = cell.spell1Id , IconPath = spell1 != null ? spell1.Icon : "" };
                var spell2 = DataDragon.Instance.GetSummonerById(cell.spell2Id);
                pick.spell2 = new SummonerSpell() { ID = cell.spell2Id , IconPath = spell2 != null ? spell2.Icon : "" };

                var champion = DataDragon.Instance.GetChampionById(cell.championId);
                pick.champion = champion;

                var summoner = AppStateController.GetSummonerById(cell.summonerId);
                if (summoner != null)
                {
                    pick.displayName = summoner.displayName;
                }

                if (currentAction != null && currentAction.type == "pick" && currentAction.actorCellId == cell.cellId && !currentAction.completed)
                {
                    pick.isActive = true;
                    newTeam.isActive = true;
                }

                picks.Add(pick);
            });
            newTeam.picks = picks;

            var IsBanDetermined = false;
            List<Ban> bans = new List<Ban>();
            kwargs.actions.Where(action => action.type == "ban" && IsInThisTeam(kwargs, action.actorCellId)).ToList().ForEach(action => {
                var ban = new Ban();

                if (!action.completed && !IsBanDetermined)
                {
                    IsBanDetermined = true;
                    ban.isActive = true;
                    newTeam.isActive = true;
                    ban.champion = new Champion();
                    bans.Add(ban);
                    return;
                }

                var champion = DataDragon.Instance.GetChampionById(action.championId);
                ban.champion = champion;

                bans.Add(ban);
                return;
            });
            newTeam.bans = bans;

            return newTeam;
        }

        public static long ConvertTimer(Timer timer)
        {
            var startOfPhase = timer.internalNowInEpochMs;
            var expectedEndOfPhase = startOfPhase + timer.adjustedTimeLeftInPhase;

            var countDown = expectedEndOfPhase - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var countDownSec = (int)Math.Floor((double)(countDown / 1000));

            if (countDownSec < 0)
                return 0;

            return countDownSec;
        }

        public static string ConvertStateName(List<Data.LCU.Action> actions, string phase)
        {
            if (phase == "FINALIZATION")
                return "FINAL PHASE";

            if (actions.Count == 0)
                return "";

            var currentActionIndex = actions.FindIndex(action => !action.completed);
            var currentAction = currentActionIndex == -1 ? actions.Last() : actions[currentActionIndex];

            if (currentAction.type == "ban")
            {
                if (currentActionIndex <= 6)
                    return "BAN PHASE 1";
                return "BAN PHASE 2";
            }
            if (currentActionIndex <= 12)
                return "PICK PHASE 1";
            return "PICK PHASE 2";
        }

        public static StateConversionOutput ConvertState(CurrentState state)
        {
            var lcuSession = state.session;
            //Log.Info(JsonConvert.SerializeObject(lcuSession));
            var flattenedActions = new List<Data.LCU.Action>();
            lcuSession.actions.ForEach(actionGroup => { actionGroup.ForEach(groupedAction => flattenedActions.Add(groupedAction)); });

            var blueTeam = ConvertTeam(new ConversionInput(lcuSession.myTeam, flattenedActions));
            var redTeam = ConvertTeam(new ConversionInput(lcuSession.theirTeam, flattenedActions));

            var timer = ConvertTimer(lcuSession.timer);
            var stateName = ConvertStateName(flattenedActions, lcuSession.timer.phase);

            return new StateConversionOutput() { blueTeam = blueTeam, redTeam = redTeam, timer = timer, state = stateName };
        }

        private static bool IsInThisTeam(ConversionInput kwargs, int cellId)
        {
            return kwargs.team.Where(cell => cell.cellId == cellId).Count() != 0;
        }


        public class ConversionInput
        {
            public List<Cell> team;
            public List<Data.LCU.Action> actions;

            public ConversionInput(List<Cell> team, List<Data.LCU.Action> actions)
            {
                this.team = team;
                this.actions = actions;
            }
        }

        public class StateConversionOutput
        {
            public Team blueTeam;
            public Team redTeam;
            public long timer;
            public string state;
        }
    }
}
