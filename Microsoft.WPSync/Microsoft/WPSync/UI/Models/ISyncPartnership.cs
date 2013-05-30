namespace Microsoft.WPSync.UI.Models
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public interface ISyncPartnership : INotifyPropertyChanged, IDisposable
    {
        event EventHandler<ContentLoadingEventArgs> ContentLoadingProgress;

        event EventHandler<PartnershipStateChangeEventArgs> StateChanged;

        event EventHandler<SyncStoppedEventArgs> SyncStopped;

        void LoadSourceContent();
        bool LockForSyncing();
        void OnSystemMusicSyncSourceChanged();
        void ResetAndReloadTargets();
        void UnlockForSyncing();

        ReadOnlyCollection<SyncOperation> AttemptedSyncOperations { get; }

        PartnershipState CurrentState { get; }

        IDevice Device { get; }

        ISyncEngine Engine { get; }

        bool IsCurrent { get; set; }

        bool IsFirstSync { get; }

        IMusicSyncSource MusicAndMovieProvider { get; }

        IPartnershipClient PartnershipClient { get; set; }

        IPictureSyncSource PhotoAndVideoProvider { get; }

        ISyncRulesManager RuleManager { get; }

        ReadOnlyCollection<SyncOperation> SuccessfulAcquiredOperations { get; }

        ReadOnlyCollection<SyncOperation> SuccessfulCumulativeSyncOperations { get; }

        ReadOnlyCollection<SyncOperation> SuccessfulSyncOperations { get; }

        Microsoft.WPSync.UI.SyncStartType SyncStartType { get; set; }
    }
}

