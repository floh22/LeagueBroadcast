using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Common.Data.Events;
using LeagueBroadcast.Common.Data.Pregame.Events;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Common.Tickable;
using LeagueBroadcast.Server.Controller;
using LeagueBroadcast.Utils.Log;
using System.Collections.Concurrent;

namespace LeagueBroadcast.Server.Http.FrontendConnector
{
    internal class PregameConnector : ITickable
    {
        private readonly ConcurrentDictionary<BroadcastEvent, double> _eventQueue;
        private ClientConnectorMode _connectionMode;
        private ComponentConfig _componentConfig;

        private float _delayInSeconds;

        public PregameConnector()
        {
            "Starting PickBan frontend connector".Info();
            _eventQueue = new();
            _componentConfig = ConfigController.Get<ComponentConfig>();
            
            if(_componentConfig.PickBan.Delay.UseDelay)
            {
                UseDelayPickBan();
            } else
            {
                UseDirectPickBan();
            }

            $"Using {_connectionMode} connection mode".Info();

            _delayInSeconds = _componentConfig.PickBan.Delay.DelayAmount;

            _componentConfig.PickBan.PropertyChanged += OnConfigPropertyChanged;
        }

        public void DoTick()
        {
            _eventQueue.Keys.ToList().ForEach(key => {
                _eventQueue[key] -= TickController.TickRateInMS;
            });

            List<KeyValuePair<BroadcastEvent, double>> toRemove = _eventQueue.Where(k => k.Value <= 0).ToList();
            toRemove.ForEach(async e => {
                await FrontendWebServer.PreGameServer.BroadcastEventAsync(e.Key);
                _ = _eventQueue.TryRemove(e.Key, out var val);
            });
        }

        private void OnConfigPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is null)
                return;
            if (e.PropertyName.Equals("UseDelay", StringComparison.Ordinal))
            {
                ChangeConnectionType();
            }

            if(e.PropertyName.Equals("DelayAmount", StringComparison.Ordinal))
            {
                _delayInSeconds = _componentConfig.PickBan.Delay.DelayAmount;
            }
        }

        private void ChangeConnectionType()
        {

            if (_componentConfig.PickBan.Delay.UseDelay)
            {
                UseDelayPickBan();
                return;
            }
            UseDirectPickBan();
        }

        public void UseDelayPickBan()
        {
            //Ignore same mode requests
            if (_connectionMode == ClientConnectorMode.DELAYED)
            {
                return;
            }

            if (PregameController.Instance.State.ChampionSelectActive)
            {
                "[ChampSelect] Tried enabling Champ Select Delay while active. Enable this while not in P&B!".Info();
                ConfigController.Get<ComponentConfig>().PickBan.Delay.UseDelay = false;
                return;
            }

            //Elegant code :) But it works, swap out direct events for queue
            PregameControllerEventHandler.StateUpdate -= SendDirect;
            PregameControllerEventHandler.StateUpdate += AddToQueue;
            PregameControllerEventHandler.NewAction -= SendDirect;
            PregameControllerEventHandler.NewAction += AddToQueue;
            PregameControllerEventHandler.ChampSelectStarted -= SendStartDirect;
            PregameControllerEventHandler.ChampSelectStarted += AddStartToQueue;
            PregameControllerEventHandler.ChampSelectEnded -= SendEndDirect;
            PregameControllerEventHandler.ChampSelectEnded += AddEndToQueue;

            _ = TickController.AddTickable(this);
            _connectionMode = ClientConnectorMode.DELAYED;
            "[ChampSelect] Using delayed PickBan".Info();
        }

        public void UseDirectPickBan()
        {
            //Ignore same mode requests
            if (_connectionMode == ClientConnectorMode.DIRECT)
                return;
            if (!_eventQueue.IsEmpty)
            {
                "Disabled Champ Select Delay while P&B was still active! This might cause some errors".Warn("[ChampSelect]");
                _eventQueue.Keys.ToList().ForEach(e => {
                    FrontendWebServer.PreGameServer.BroadcastEventAsync(e);
                });
                _eventQueue.Clear();
            }

            //Swap out queue for direct events
            PregameControllerEventHandler.StateUpdate -= AddToQueue;
            PregameControllerEventHandler.StateUpdate += SendDirect;
            PregameControllerEventHandler.NewAction -= AddToQueue;
            PregameControllerEventHandler.NewAction += SendDirect;
            PregameControllerEventHandler.ChampSelectStarted -= AddStartToQueue;
            PregameControllerEventHandler.ChampSelectStarted += SendStartDirect;
            PregameControllerEventHandler.ChampSelectEnded -= AddEndToQueue;
            PregameControllerEventHandler.ChampSelectEnded += SendEndDirect;

            _ = TickController.RemoveTickable(this);
            _connectionMode = ClientConnectorMode.DIRECT;
            "Using direct PickBan".Info("[ChampSelect]");
        }

        private void SendStartDirect(object? sender, ChampSelectStartedEventArgs e)
        {
            FrontendWebServer.PreGameServer.BroadcastEventAsync(new ChampionSelectStart());
        }

        private void SendEndDirect(object? sender, ChampSelectEndedEventArgs e)
        {
            FrontendWebServer.PreGameServer.BroadcastEventAsync(new ChampionSelectEnd(e.IsFinished));
        }

        private void SendDirect(object? sender, StateUpdateEventArgs e)
        {
            FrontendWebServer.PreGameServer.BroadcastEventAsync(new NewState(e.State));
        }

        private void SendDirect(object? sender, NewActionEventArgs e)
        {
            FrontendWebServer.PreGameServer.BroadcastEventAsync(new NewAction(e.NewAction));
        }

        private void AddToQueue(object? sender, StateUpdateEventArgs e)
        {
            _eventQueue.TryAdd(new NewState(e.State), _delayInSeconds);
        }

        private void AddToQueue(object? sender, NewActionEventArgs e)
        {
            _eventQueue.TryAdd(new NewAction(e.NewAction), _delayInSeconds);
        }

        private void AddStartToQueue(object? sender, ChampSelectStartedEventArgs e)
        {
            _eventQueue.TryAdd(new ChampionSelectStart(), _delayInSeconds);
        }

        private void AddEndToQueue(object? sender, ChampSelectEndedEventArgs e)
        {
            //Make sure that champ select went all the way
            //otherwise send the end event directly if champ select has already started on the frontend
            //or just not send anything at all if it never got that far
            //This eliminates champ select remakes from ever even showing up
            if (e.IsFinished)
            {
                _eventQueue.TryAdd(new ChampionSelectEnd(e.IsFinished), _delayInSeconds);
                return;
            }

            //If the EventQueue does not contain a Champ Select Start Event and Ended has just been fired
            //That means that Start has already been sent, so we have to send an End event. Don't otherwise
            if (!_eventQueue.Keys.ToList().Contains(new ChampionSelectStart()))
            {
                FrontendWebServer.PreGameServer.BroadcastEventAsync(new NewState(PregameController.Instance.State));
                FrontendWebServer.PreGameServer.BroadcastEventAsync(new ChampionSelectEnd(e.IsFinished));
            }

            _eventQueue.Clear();
        }

        public enum ClientConnectorMode
        {
            NONE,
            DIRECT,
            DELAYED
        }
    }
}
