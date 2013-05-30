namespace Microsoft.WPSync.UI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class DeviceEnumerationListener : IDeviceEnumerationListener, IDisposable
    {
        private EventArrivedEventHandler _eventArrived;
        private static readonly Guid PortableDeviceClassGuid = new Guid("eec5ad98-8080-425f-922a-dabf3de3f69a");
        private ManagementEventWatcher watcher;

        public event EventArrivedEventHandler EventArrived
        {
            add
            {
                EventArrivedEventHandler handler2;
                EventArrivedEventHandler eventArrived = this._eventArrived;
                do
                {
                    handler2 = eventArrived;
                    EventArrivedEventHandler handler3 = (EventArrivedEventHandler) Delegate.Combine(handler2, value);
                    eventArrived = Interlocked.CompareExchange<EventArrivedEventHandler>(ref this._eventArrived, handler3, handler2);
                }
                while (eventArrived != handler2);
            }
            remove
            {
                EventArrivedEventHandler handler2;
                EventArrivedEventHandler eventArrived = this._eventArrived;
                do
                {
                    handler2 = eventArrived;
                    EventArrivedEventHandler handler3 = (EventArrivedEventHandler) Delegate.Remove(handler2, value);
                    eventArrived = Interlocked.CompareExchange<EventArrivedEventHandler>(ref this._eventArrived, handler3, handler2);
                }
                while (eventArrived != handler2);
            }
        }

        public DeviceEnumerationListener()
        {
            WqlEventQuery query = new WqlEventQuery("__InstanceOperationEvent", "TargetInstance ISA 'Win32_PnPEntity'") {
                WithinInterval = new TimeSpan(0, 0, 1)
            };
            this.watcher = new ManagementEventWatcher(query);
            this.watcher.EventArrived += new EventArrivedEventHandler(this.watcher_EventArrived);
            this.watcher.Start();
        }

        public void Dispose()
        {
            this.watcher.Dispose();
        }

        public void Start()
        {
            this.watcher.Start();
        }

        public void Stop()
        {
            try
            {
                this.watcher.Stop();
            }
            catch (COMException)
            {
            }
        }

        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies")]
        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject obj2 = e.NewEvent.Properties["TargetInstance"].Value as ManagementBaseObject;
            string g = (string) obj2.Properties["ClassGuid"].Value;
            if (g != null)
            {
                Guid guid = new Guid(g);
                if ((guid == PortableDeviceClassGuid) && (this._eventArrived != null))
                {
                    this._eventArrived(this, e);
                }
            }
        }
    }
}

