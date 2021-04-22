using LeagueBroadcastHub.Events;
using LeagueBroadcastHub.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LeagueBroadcastHub.Session;
using LeagueBroadcastHub.Events.Client;
using LeagueIngameServer;
using static LeagueBroadcastHub.State.Client.StateData;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace LeagueBroadcastHub.Server
{
    class ChampSelectConnector : ITickable
    {
        private ConcurrentDictionary<LeagueEvent, double> EventQueue;
        private ClientConnectorMode connectionMode;
        public ChampSelectConnector()
        {
            EventQueue = new ConcurrentDictionary<LeagueEvent, double>();
            if(ActiveSettings.current.DelayPickBan)
            {
                UseDelayPickBan();
                return;
            }
            UseDirectPickBan();
        }

        public void DoTick()
        {
            EventQueue.Keys.ToList().ForEach(key => {
                EventQueue[key] -= (double)(1 / (double)BroadcastHubController.tickRate);
            });

            var toRemove = EventQueue.Where(k => k.Value <= 0).ToList();
            toRemove.ForEach(async e => {
                EmbedIOServer.socketServer.SendEventToAllAsync(e.Key);
                EventQueue.TryRemove(e.Key, out var val);
                await Task.Delay(50);
            });
        }

        public void UseDelayPickBan()
        {
            //Ignore same mode requests
            if(connectionMode == ClientConnectorMode.DELAYED)
                return;
            if(State.Client.State.data.champSelectActive)
            {
                Logging.Warn("Tried enabling Champ Select Delay while active. Enable this while not in P&B!");
                ActiveSettings.current.DelayPickBan = false;
                return;
            }

            //Elegant code :) But it works, swap out direct events for queue
            State.Client.State.StateUpdate -= SendDirect;
            State.Client.State.StateUpdate += AddToQueue;
            State.Client.State.NewAction -= SendDirect;
            State.Client.State.NewAction += AddToQueue;
            State.Client.State.ChampSelectStarted -= SendStartDirect;
            State.Client.State.ChampSelectStarted += AddStartToQueue;
            State.Client.State.ChampSelectEnded -= SendEndDirect;
            State.Client.State.ChampSelectEnded += AddEndToQueue;

            BroadcastHubController.ToTick.Add(this);
            connectionMode = ClientConnectorMode.DELAYED;
            Logging.Info("Using delayed PickBan");
        }

        public void UseDirectPickBan()
        {
            //Ignore same mode requests
            if (connectionMode == ClientConnectorMode.DIRECT)
                return;
            if (EventQueue.Count != 0)
            {
                Logging.Warn("Disabled Champ Select Delay while P&B was still active! This might cause some errors");
                EventQueue.Keys.ToList().ForEach(e => {
                    EmbedIOServer.socketServer.SendEventToAllAsync(e);
                });
                EventQueue.Clear();
            }

            //Swap out queue for direct events
            State.Client.State.StateUpdate -= AddToQueue;
            State.Client.State.StateUpdate += SendDirect;
            State.Client.State.NewAction -= AddToQueue;
            State.Client.State.NewAction += SendDirect;
            State.Client.State.ChampSelectStarted -= AddStartToQueue;
            State.Client.State.ChampSelectStarted += SendStartDirect;
            State.Client.State.ChampSelectEnded -= AddEndToQueue;
            State.Client.State.ChampSelectEnded += SendEndDirect;

            BroadcastHubController.ToTick.Remove(this);
            connectionMode = ClientConnectorMode.DIRECT;
            Logging.Info("Using direct PickBan");
        }

        private void AddToQueue(object sender, State.Client.StateData e)
        {
            EventQueue.TryAdd(new NewStateEvent(new State.Client.StateData(e)), ActiveSettings.current.DelayPickBanValue);
        }

        private void AddToQueue(object sender, CurrentAction e)
        {
            EventQueue.TryAdd(new NewActionEvent(new CurrentAction(e)), ActiveSettings.current.DelayPickBanValue);
        }

        private void AddStartToQueue(object sender, EventArgs e)
        {
            EventQueue.TryAdd(new ChampSelectStartEvent(), ActiveSettings.current.DelayPickBanValue);
        }

        private void AddEndToQueue(object sender, bool finished)
        {
            //Make sure that champ select went all the way
            //otherwise send the end event directly if champ select has already started on the frontend
            //or just not send anything at all if it never got that far
            //This eliminates champ select remakes from ever even showing up
            if(finished)
            {
                EventQueue.TryAdd(new ChampSelectEndEvent(), ActiveSettings.current.DelayPickBanValue);
                return;
            }

            //If the EventQueue does not contain a Champ Select Start Event and Ended has just been fired
            //That means that Start has already been sent, so we have to send an End event. Don't otherwise
            if(!EventQueue.Keys.ToList().Contains(new ChampSelectStartEvent()))
            {
                EmbedIOServer.socketServer.SendEventToAllAsync(new NewStateEvent(State.Client.State.data));
                EmbedIOServer.socketServer.SendEventToAllAsync(new ChampSelectEndEvent());
            }

            EventQueue.Clear();
        }
        private void SendStartDirect(object sender, EventArgs e)
        {
            EmbedIOServer.socketServer.SendEventToAllAsync(new ChampSelectStartEvent());
        }

        private void SendEndDirect(object sender, bool finished)
        {
            EmbedIOServer.socketServer.SendEventToAllAsync(new ChampSelectEndEvent());
        }

        private void SendDirect(object sender, State.Client.StateData e)
        {
            EmbedIOServer.socketServer.SendEventToAllAsync(new NewStateEvent(e));
        }

        private void SendDirect(object sender, CurrentAction e)
        {
            EmbedIOServer.socketServer.SendEventToAllAsync(new NewActionEvent(e));
        }

        public enum ClientConnectorMode
        {
            NONE,
            DIRECT,
            DELAYED
        }

    }
}
