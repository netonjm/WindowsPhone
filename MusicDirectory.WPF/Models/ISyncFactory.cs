namespace Microsoft.WPSync.UI.Models
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.Sync.Source.Zmdb;
    using System;

    public interface ISyncFactory
    {
        ISyncEngine CreateISyncEngine();
        ISyncRulesManager CreateISyncRulesManager(string deviceId);
        IMusicSyncSource CreateMusicSyncSource();
        IPictureSyncSource CreatePictureSyncSource();
        IZmdbSyncSource CreateZmdbSyncSource(IDevice device);
    }
}

