namespace Microsoft.WPSync.UI.Utilities
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Models;
    using System;

    public interface ISqmHelper
    {
        void CreateDeviceInfoStream(ISyncPartnership partnership, int uniqueIndex);
        void CreateSyncContentStream(ISyncPartnership partnership, SyncStartType type, SyncStoppedReason reason, TimeSpan syncDuration);
        void EndSqmSession(uint uniquelyConnectedDeviceCount);
        void InitSqm();
        void OnDeviceConnected(IDevice device, int index);
        void OnSqmOptinChanged(bool value);
    }
}

