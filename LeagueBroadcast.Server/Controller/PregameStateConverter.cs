using LeagueBroadcast.Common.Data.CommunityDragon;
using LeagueBroadcast.Common.Data.LCU;
using LeagueBroadcast.Common.Data.Pregame;
using LeagueBroadcast.Common.Data.Pregame.State;

namespace LeagueBroadcast.Server.Controller
{
    internal class PregameStateConverter
    {
        public static PreGameTeamState ConvertTeam(PregameConversionInput kwargs)
        {
            PreGameTeamState newTeam = new();
            List<Pick> picks = new();
            kwargs.Team.ForEach(cell =>
            {
                Common.Data.LCU.Action? currentAction = kwargs.Actions.Where(action => !action.Completed).FirstOrDefault();
                Pick pick = new(cell.CellId);

                SummonerSpell? spell1 = SummonerSpell.All.SingleOrDefault(spell => spell.ID == cell.Spell1Id);
                pick.Spell1 = new() { Name = cell.Spell1Id + "", IconPath = spell1 != null ? spell1.IconPath : "" };
                SummonerSpell? spell2 = SummonerSpell.All.SingleOrDefault(spell => spell.ID == cell.Spell2Id);
                pick.Spell2 = new() { Name = cell.Spell2Id + "", IconPath = spell2 != null ? spell2.IconPath : "" };

                pick.Champion = Champion.All.SingleOrDefault(c => c.ID == cell.ChampionId);
                Player? summoner = PregameController.Instance.CachedSummoners.SingleOrDefault(s => s.SummonerId == cell.SummonerId);

                if (summoner != null)
                {
                    pick.DisplayName = summoner.DisplayName;
                }

                if (currentAction != null && currentAction.Type == "pick" && currentAction.ActorCellId == cell.CellId && !currentAction.Completed)
                {
                    pick.IsActive = true;
                    newTeam.IsActive = true;
                }

                picks.Add(pick);
            });
            newTeam.Picks = picks;

            bool IsBanDetermined = false;
            List<Ban> bans = new();
            kwargs.Actions.Where(action => action.Type == "ban" && IsInThisTeam(kwargs, action.ActorCellId)).ToList().ForEach(action =>
            {
                Ban ban = new();

                if (!action.Completed && !IsBanDetermined)
                {
                    IsBanDetermined = true;
                    ban.IsActive = true;
                    newTeam.IsActive = true;
                    ban = new();
                    bans.Add(ban);
                    return;
                }

                ban.Champion = Champion.All.Single(c => c.ID == action.ChampionID);
                bans.Add(ban);
                return;
            });
            newTeam.Bans = bans;

            return newTeam;
        }

        public static long ConvertTimer(SessionTimer timer)
        {
            long expectedEndOfPhase = timer.InternalNowInEpochMs + timer.AdjustedTimeLeftInPhase;
            long countDownMs = expectedEndOfPhase - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long countDownSec = (long)Math.Floor(countDownMs / 1000.0);
            return Math.Max(countDownSec, 0);
        }

        public static string ConvertStateName(List<Common.Data.LCU.Action> actions)
        {
            int currentActionIndex = actions.FindIndex(action => !action.Completed);

            return currentActionIndex == -1
                ? ""
                : actions[currentActionIndex].Type == "ban"
                ? currentActionIndex <= 6 ? "BAN PHASE 1" : "BAN PHASE 2"
                : currentActionIndex <= 12 ? "PICK PHASE 1" : "PICK PHASE 2";
        }

        public static PregameStateConversionOutput ConvertState(CurrentSession state)
        {
            Session lcuSession = state.Session;
            List<Common.Data.LCU.Action> flattenedActions = new();
            lcuSession.Actions.ForEach(actionGroup => { actionGroup.ForEach(groupedAction => flattenedActions.Add(groupedAction)); });

            PreGameTeamState blueTeam = ConvertTeam(new PregameConversionInput(lcuSession.MyTeam, flattenedActions));
            PreGameTeamState redTeam = ConvertTeam(new PregameConversionInput(lcuSession.TheirTeam, flattenedActions));

            long timer = ConvertTimer(lcuSession.Timer);
            string stateName = ConvertStateName(flattenedActions);

            return new PregameStateConversionOutput() { BlueTeam = blueTeam, RedTeam = redTeam, Timer = timer, State = stateName };
        }

        private static bool IsInThisTeam(PregameConversionInput kwargs, int cellId)
        {
            return kwargs.Team.Any(cell => cell.CellId == cellId);
        }
    }
}
