namespace Microsoft.WPSync.UI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Management;

    public interface IDeviceEnumerationListener : IDisposable
    {
        event EventArrivedEventHandler EventArrived;

        void Start();
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId="Stop")]
        void Stop();
    }
}

