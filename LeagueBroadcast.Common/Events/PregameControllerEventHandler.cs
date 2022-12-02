using LeagueBroadcast.Common.Data.Pregame.State;

namespace LeagueBroadcast.Common.Events
{
    public static class PregameControllerEventHandler
    {
        public static event EventHandler<StateUpdateEventArgs>? StateUpdate;
        public static event EventHandler<NewActionEventArgs>? NewAction;
        public static event EventHandler<ChampSelectStartedEventArgs>? ChampSelectStarted;
        public static event EventHandler<ChampSelectEndedEventArgs>? ChampSelectEnded;


        public static void FireStateUpdate(StateUpdateEventArgs args, object? sender = null)
        {
            StateUpdate?.Invoke(sender, args);
        }

        public static void FireNewAction(NewActionEventArgs args, object? sender = null)
        {
            NewAction?.Invoke(sender, args);
        }

        public static void FireChampSelectStarted(ChampSelectStartedEventArgs args, object? sender = null)
        {
            ChampSelectStarted?.Invoke(sender, args);
        }

        public static void FireChampSelectEnded(ChampSelectEndedEventArgs args, object? sender = null)
        {
            ChampSelectEnded?.Invoke(sender, args);
        }
    }



    public class StateUpdateEventArgs : EventArgs
    {
        public PregameState State;

        public StateUpdateEventArgs(PregameState state)
        {
            State = state;
        }
    }

    public class NewActionEventArgs : EventArgs
    {
        public CurrentAction NewAction;


        public NewActionEventArgs(CurrentAction newAction)
        {
            NewAction = newAction;
        }
    }

    public class ChampSelectStartedEventArgs : EventArgs
    {

    }

    public class ChampSelectEndedEventArgs : EventArgs
    {
        public bool IsFinished;

        public ChampSelectEndedEventArgs(bool isFinished)
        {
            IsFinished = isFinished;
        }
    }
}
