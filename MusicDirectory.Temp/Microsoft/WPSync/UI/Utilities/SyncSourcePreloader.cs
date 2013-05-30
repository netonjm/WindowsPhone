namespace Microsoft.WPSync.UI.Utilities
{
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using System;
    using System.Linq;

    public class SyncSourcePreloader : ISyncSourcePreloader
    {
        private IMediaSyncSource musicSyncSource;
        private IMediaSyncSource photoSyncSource;

        private void InitSources()
        {
            if (DeviceSettings.GetDeviceDirectories().Count<string>() > 0)
            {
                switch ((GlobalSetting.GetApplicationSetting("MusicSyncSource") as string))
                {
                    case "ITunes":
                        this.musicSyncSource = DependencyContainer.ResolveITunesMusicSyncSource();
                        break;

                    case "WindowsLibraries":
                        this.musicSyncSource = DependencyContainer.ResolveWindowsLibraryMusicSyncSource();
                        break;
                }
            }
            this.photoSyncSource = DependencyContainer.ResolvePhotosSyncSource();
        }

        public void PreloadSources()
        {
            this.InitSources();
            if (((this.musicSyncSource != null) && !this.musicSyncSource.IsLoaded) && !this.musicSyncSource.HasClients)
            {
                this.musicSyncSource.ReloadModelData();
            }
            if (((this.photoSyncSource != null) && !this.photoSyncSource.IsLoaded) && !this.photoSyncSource.HasClients)
            {
                this.photoSyncSource.ReloadModelData();
            }
        }
    }
}

