namespace Microsoft.WPSync.UI.Models
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.Sync.Source.Zmdb;
    using Microsoft.WPSync.UI;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class SyncFactory : ISyncFactory
    {
        public ISyncEngine CreateISyncEngine()
        {
            return DependencyContainer.ResolveISyncEngine();
        }

        public ISyncRulesManager CreateISyncRulesManager(string deviceId)
        {
            return DependencyContainer.ResolveISyncRulesManager(deviceId);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="Microsoft.WPSync.Logging.Logger.TraceInformation(System.String,System.String,System.String)")]
        public IMusicSyncSource CreateMusicSyncSource()
        {
            string applicationSetting = (string) GlobalSetting.GetApplicationSetting("MusicSyncSource");
            switch (applicationSetting)
            {
                case "ITunes":
                    return DependencyContainer.ResolveITunesMusicSyncSource();

                case "WindowsLibraries":
                    return DependencyContainer.ResolveWindowsLibraryMusicSyncSource();
            }
            Logger.TraceInformation("Managed:SyncEngine", "Factory: Bad music sync source type", applicationSetting);
            return DependencyContainer.ResolveWindowsLibraryMusicSyncSource();
        }

        public IPictureSyncSource CreatePictureSyncSource()
        {
            return DependencyContainer.ResolvePhotosSyncSource();
        }

        public IZmdbSyncSource CreateZmdbSyncSource(IDevice device)
        {
            return DependencyContainer.ResolveZmdbSyncSource(device);
        }
    }
}

