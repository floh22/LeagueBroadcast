using Timer = System.Timers.Timer;

namespace LeagueBroadcast.Common.Tickable
{
    public abstract class TickController
    {
        private static readonly Dictionary<uint, TickTimer> _timers = new();

        private static uint TickRate { get; set; } = (uint)500;

        public static uint TickRateInMS
        {
            get => TickRate;
            set => TickRate = value;
        }

        public static uint TicksPerSecond
        {
            get => 1 / TickRate;
            set => TickRate = (uint)((1 / (decimal)value) * 1000);
        }

        public static bool AddTickable(int position, ITickable toTick, uint milliseconds = 0)
        {
            if (toTick.IsTicking())
            {
                return false;
            }
            if (milliseconds == 0)
            {
                milliseconds = TickRateInMS;
            }
            if (!_timers.ContainsKey(milliseconds))
            {
                _timers.Add(milliseconds, new TickTimer(milliseconds).With(toTick));
                return true;
            }
            if (position == -1)
            {
                _timers[milliseconds].Add(toTick);
            }
            else
            {
                _timers[milliseconds].Add(position, toTick);
            }
            return true;
        }

        public static bool AddTickable(ITickable toTick, uint milliseconds = 0)
        {
            return AddTickable(-1, toTick, milliseconds);
        }

        public static bool RemoveTickable(ITickable tickable)
        {
            TickTimer? timer = _timers.Values.ToList().SingleOrDefault(timer => timer.Remove(tickable));
            if (timer is null)
            {
                return false;
            }

            if(timer.IsEmpty())
            {
                _ = _timers.Remove(timer.TickRate);
                timer.Dispose();
            }

            return true;
        }

        public static bool IsTicking(ITickable tickable)
        {
            return _timers.Values.ToList().Any(timer => timer.IsTicking(tickable));
        }

        public static void Stop()
        {
            _timers.Values.ToList().ForEach(timer => timer.Stop());
            _timers.Clear();
        }
    }


    public class TickTimer : IDisposable
    {
        public uint TickRate { get; }
        private readonly Timer _timer;
        private readonly List<ITickable> toTick;
        private readonly object tickListLock = new();

        public TickTimer(uint milliseconds)
        {
            lock(tickListLock)
            {
                _timer = new Timer() { Interval = milliseconds, Enabled = true, AutoReset = true };
                _timer.Elapsed += DoTick;
                toTick = new List<ITickable>();
                TickRate = milliseconds;
            }
        }

        public void Add(ITickable tickable)
        {
            lock (tickListLock)
            {
                toTick.Add(tickable);
            }
        }
        
        public void Add(int position, ITickable tickable)
        {
            lock (tickListLock)
            {
                toTick.Insert(position, tickable);
            }
        }

        public bool Remove(ITickable tickable)
        {
            lock (tickListLock)
            {
                return toTick.Remove(tickable);
            }
        }

        public bool IsTicking(ITickable tickable)
        {
            return toTick.Contains(tickable);
        }

        public bool IsEmpty()
        {
            return toTick.Count == 0;
        }


        public TickTimer With(ITickable tickable)
        {
            lock (tickListLock)
            {
                Add(tickable);
            }
            return this;
        }

        public void Stop()
        {
            Dispose();
        }

        private void DoTick(object? sender, EventArgs e)
        {
            lock (tickListLock)
            {
                for(int i = 0; i < toTick.Count; i++)
                {
                    toTick[i].DoTick();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                _timer.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
