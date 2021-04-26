using LeagueBroadcast.ChampSelect.Data.LCU;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueBroadcast.Common.Controllers
{
    class AppStateController : ITickable
    {
        public static EventHandler GameStart, GameLoad, GameStop, ChampSelectStart, ChampSelectStop;

        public static List<Summoner> summoners = new();

        public void DoTick()
        {

        }

        public static Summoner GetSummonerById(int id)
        {
            return summoners.Single(summoner => summoner.summonerId == id);
        }

        public async void CheckLeagueRunning()
        {
            var gameData = await IngameController.LoLDataProvider.GetGameData();
            if (gameData == null)
                return;

            GameStart?.Invoke(this, EventArgs.Empty);
        }

    }

    public class StateControllerSettings
    {
        public bool PickBan;
        public bool Delay;
        public bool Ingame;
    }
}

