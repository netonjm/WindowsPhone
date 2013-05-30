namespace Microsoft.WPSync.UI.Models
{
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public static class MediaSyncSourceFactory
    {
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="Microsoft.WPSync.Logging.Logger.TraceError(System.String,System.String,System.String)")]
        public static IMusicSyncSource CreateMusicSyncSource()
        {
            string applicationSetting = (string) GlobalSetting.GetApplicationSetting("MusicSyncSource");
            switch (applicationSetting)
            {
                case "ITunes":
                    return DependencyContainer.ResolveITunesMusicSyncSource();

                case "WindowsLibraries":
                    return DependencyContainer.ResolveWindowsLibraryMusicSyncSource();
            }
            Logger.TraceError("Managed:SyncEngine", "Factory: Bad music sync source type", applicationSetting);
            return DependencyContainer.ResolveWindowsLibraryMusicSyncSource();
        }

        public static IPictureSyncSource CreatePictureSyncSource()
        {
            return DependencyContainer.ResolvePhotosSyncSource();
        }
    }
}

