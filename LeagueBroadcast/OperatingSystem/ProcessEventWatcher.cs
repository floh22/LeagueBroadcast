using System;
using System.Management;
using System.Threading.Tasks;

namespace LeagueBroadcast.OperatingSystem
{
    //Taken from https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff/OperatingSystem/ProcessEventWatcher.cs
    public sealed class ProcessEventWatcher : IDisposable
    {
        public event EventHandler<ProcessEventArguments>? ProcessStarted;
        public event EventHandler<ProcessEventArguments>? ProcessStopped;

        private ManagementEventWatcher ProcessStartedEventWatcher { get; }
        private ManagementEventWatcher ProcessStoppedEventWatcher { get; }

        public ProcessEventWatcher()
        {
            ProcessStartedEventWatcher = CreateProcessStartedEventWatcher();
            ProcessStoppedEventWatcher = CreateProcessStoppedEventWatcher();
        }

        private ManagementEventWatcher CreateProcessStartedEventWatcher()
        {
            var startQuery = new EventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA \"Win32_Process\"");
            var watcher = new ManagementEventWatcher(startQuery);
            watcher.EventArrived += ProcessStartedEventWatcher_OnEventArrived;
            watcher.Start();
            return watcher;
        }

        private ManagementEventWatcher CreateProcessStoppedEventWatcher()
        {
            var stopQuery = new EventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA \"Win32_Process\"");
            var watcher = new ManagementEventWatcher(stopQuery);
            watcher.EventArrived += ProcessStoppedEventWatcher_OnEventArrived;
            watcher.Start();
            return watcher;
        }

        private void ProcessStartedEventWatcher_OnEventArrived(object sender, EventArrivedEventArgs e)
        {
            if (ProcessStarted == null)
            {
                return;
            }

            Task.Run(() => InvokeProcessEvent(e, ProcessStarted));
        }

        private void ProcessStoppedEventWatcher_OnEventArrived(object sender, EventArrivedEventArgs e)
        {
            if (ProcessStopped == null)
            {
                return;
            }

            Task.Run(() => InvokeProcessEvent(e, ProcessStopped));
        }

        private void InvokeProcessEvent(EventArrivedEventArgs eventArgs, EventHandler<ProcessEventArguments> eventHandler)
        {
            var processId = GetProcessIdFromManagementEvent(eventArgs);
            if (processId == null)
            {
                return;
            }

            eventHandler.Invoke(this, new ProcessEventArguments(processId.Value));
        }

        private static int? GetProcessIdFromManagementEvent(EventArrivedEventArgs e)
        {
            if (!(e.NewEvent.GetPropertyValue("TargetInstance") is ManagementBaseObject targetInstance))
            {
                return null;
            }

            return Convert.ToInt32(targetInstance.GetPropertyValue("ProcessId"));
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ProcessEventWatcher()
        {
            Dispose(false);
        }

        private bool _isDisposed;

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            if (disposing)
            {
                ProcessStartedEventWatcher.Dispose();
                ProcessStoppedEventWatcher.Dispose();
            }
        }

        #endregion
    }
}
