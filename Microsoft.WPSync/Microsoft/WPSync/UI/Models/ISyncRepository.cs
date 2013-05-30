namespace Microsoft.WPSync.UI.Models
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    public interface ISyncRepository
    {
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        List<string> DeviceList();
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId="1#")]
        bool EnsureSyncPartnershipForDevice(IDevice device, out ISyncPartnership partnership);
        IMusicLibraryProvider MusicLibraryForDevice(string deviceSerialNumber);
        IPictureLibraryProvider PictureLibraryForDevice(string deviceSerialNumber);
        void RemoveSyncPartnership(string deviceSerialNumber);
        ISyncEngine SyncEngineForDevice(string deviceSerialNumber);
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        List<ISyncEngine> SyncEngineList();
        ISyncPartnership SyncPartnershipForDevice(string deviceSerialNumber);
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        List<ISyncPartnership> SyncPartnershipList();
        ISyncRules SyncRulesForDevice(string deviceSerialNumber);
    }
}

