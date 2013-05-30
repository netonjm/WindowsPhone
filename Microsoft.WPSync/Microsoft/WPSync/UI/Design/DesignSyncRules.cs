namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.Sync.Rules;
    using System;

    internal class DesignSyncRules : SyncRules
    {
        public DesignSyncRules()
        {
            base.IsMusicSyncEnabled = true;
            base.IsPhotoVideoSyncEnabled = true;
            base.IsPodcastSyncEnabled = true;
            base.IsTVMoviesSyncEnabled = true;
        }
    }
}

