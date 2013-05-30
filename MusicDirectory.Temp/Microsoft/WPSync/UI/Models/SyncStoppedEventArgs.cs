namespace Microsoft.WPSync.UI.Models
{
    using Microsoft.WPSync.Sync.Engine;
    using System;
    using System.Runtime.CompilerServices;

    public class SyncStoppedEventArgs : EventArgs
    {
        public SyncStoppedEventArgs(SyncStoppedReason reason)
        {
            this.Reason = reason;
        }

        public SyncStoppedReason Reason { get; private set; }
    }
}

