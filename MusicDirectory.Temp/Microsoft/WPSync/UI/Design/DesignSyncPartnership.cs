namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Models;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class DesignSyncPartnership : ISyncPartnership, INotifyPropertyChanged, IDisposable
    {
        private EventHandler<ContentLoadingEventArgs> _contentLoadingProgress;
        private PropertyChangedEventHandler _propertyChanged;
        private EventHandler<PartnershipStateChangeEventArgs> _stateChanged;
        private EventHandler<SyncStoppedEventArgs> _syncStopped;

        public event EventHandler<ContentLoadingEventArgs> ContentLoadingProgress
        {
            add
            {
                EventHandler<ContentLoadingEventArgs> handler2;
                EventHandler<ContentLoadingEventArgs> contentLoadingProgress = this._contentLoadingProgress;
                do
                {
                    handler2 = contentLoadingProgress;
                    EventHandler<ContentLoadingEventArgs> handler3 = (EventHandler<ContentLoadingEventArgs>) Delegate.Combine(handler2, value);
                    contentLoadingProgress = Interlocked.CompareExchange<EventHandler<ContentLoadingEventArgs>>(ref this._contentLoadingProgress, handler3, handler2);
                }
                while (contentLoadingProgress != handler2);
            }
            remove
            {
                EventHandler<ContentLoadingEventArgs> handler2;
                EventHandler<ContentLoadingEventArgs> contentLoadingProgress = this._contentLoadingProgress;
                do
                {
                    handler2 = contentLoadingProgress;
                    EventHandler<ContentLoadingEventArgs> handler3 = (EventHandler<ContentLoadingEventArgs>) Delegate.Remove(handler2, value);
                    contentLoadingProgress = Interlocked.CompareExchange<EventHandler<ContentLoadingEventArgs>>(ref this._contentLoadingProgress, handler3, handler2);
                }
                while (contentLoadingProgress != handler2);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEventHandler handler2;
                PropertyChangedEventHandler propertyChanged = this._propertyChanged;
                do
                {
                    handler2 = propertyChanged;
                    PropertyChangedEventHandler handler3 = (PropertyChangedEventHandler) Delegate.Combine(handler2, value);
                    propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this._propertyChanged, handler3, handler2);
                }
                while (propertyChanged != handler2);
            }
            remove
            {
                PropertyChangedEventHandler handler2;
                PropertyChangedEventHandler propertyChanged = this._propertyChanged;
                do
                {
                    handler2 = propertyChanged;
                    PropertyChangedEventHandler handler3 = (PropertyChangedEventHandler) Delegate.Remove(handler2, value);
                    propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this._propertyChanged, handler3, handler2);
                }
                while (propertyChanged != handler2);
            }
        }

        public event EventHandler<PartnershipStateChangeEventArgs> StateChanged
        {
            add
            {
                EventHandler<PartnershipStateChangeEventArgs> handler2;
                EventHandler<PartnershipStateChangeEventArgs> stateChanged = this._stateChanged;
                do
                {
                    handler2 = stateChanged;
                    EventHandler<PartnershipStateChangeEventArgs> handler3 = (EventHandler<PartnershipStateChangeEventArgs>) Delegate.Combine(handler2, value);
                    stateChanged = Interlocked.CompareExchange<EventHandler<PartnershipStateChangeEventArgs>>(ref this._stateChanged, handler3, handler2);
                }
                while (stateChanged != handler2);
            }
            remove
            {
                EventHandler<PartnershipStateChangeEventArgs> handler2;
                EventHandler<PartnershipStateChangeEventArgs> stateChanged = this._stateChanged;
                do
                {
                    handler2 = stateChanged;
                    EventHandler<PartnershipStateChangeEventArgs> handler3 = (EventHandler<PartnershipStateChangeEventArgs>) Delegate.Remove(handler2, value);
                    stateChanged = Interlocked.CompareExchange<EventHandler<PartnershipStateChangeEventArgs>>(ref this._stateChanged, handler3, handler2);
                }
                while (stateChanged != handler2);
            }
        }

        public event EventHandler<SyncStoppedEventArgs> SyncStopped
        {
            add
            {
                EventHandler<SyncStoppedEventArgs> handler2;
                EventHandler<SyncStoppedEventArgs> syncStopped = this._syncStopped;
                do
                {
                    handler2 = syncStopped;
                    EventHandler<SyncStoppedEventArgs> handler3 = (EventHandler<SyncStoppedEventArgs>) Delegate.Combine(handler2, value);
                    syncStopped = Interlocked.CompareExchange<EventHandler<SyncStoppedEventArgs>>(ref this._syncStopped, handler3, handler2);
                }
                while (syncStopped != handler2);
            }
            remove
            {
                EventHandler<SyncStoppedEventArgs> handler2;
                EventHandler<SyncStoppedEventArgs> syncStopped = this._syncStopped;
                do
                {
                    handler2 = syncStopped;
                    EventHandler<SyncStoppedEventArgs> handler3 = (EventHandler<SyncStoppedEventArgs>) Delegate.Remove(handler2, value);
                    syncStopped = Interlocked.CompareExchange<EventHandler<SyncStoppedEventArgs>>(ref this._syncStopped, handler3, handler2);
                }
                while (syncStopped != handler2);
            }
        }

        public DesignSyncPartnership()
        {
            this.TestAllEvents();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void LoadSourceContent()
        {
            throw new NotImplementedException();
        }

        public bool LockForSyncing()
        {
            throw new NotImplementedException();
        }

        public void OnSystemMusicSyncSourceChanged()
        {
            throw new NotImplementedException();
        }

        public void ResetAndReloadTargets()
        {
            throw new NotImplementedException();
        }

        private void TestAllEvents()
        {
            if (((this._syncStopped == null) && (this._contentLoadingProgress == null)) && (this._stateChanged == null))
            {
                PropertyChangedEventHandler propertyChanged = this._propertyChanged;
            }
        }

        public void UnlockForSyncing()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<SyncOperation> AttemptedSyncOperations
        {
            get
            {
                return null;
            }
        }

        public PartnershipState CurrentState
        {
            get
            {
                return PartnershipState.Idle;
            }
        }

        public IDevice Device { get; set; }

        public ISyncEngine Engine { get; set; }

        public bool IsCurrent
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public bool IsFirstSync
        {
            get
            {
                return false;
            }
        }

        public IMusicSyncSource MusicAndMovieProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IPartnershipClient PartnershipClient
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IPictureSyncSource PhotoAndVideoProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ISyncRulesManager RuleManager
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ReadOnlyCollection<SyncOperation> SuccessfulAcquiredOperations
        {
            get
            {
                return null;
            }
        }

        public ReadOnlyCollection<SyncOperation> SuccessfulCumulativeSyncOperations
        {
            get
            {
                return null;
            }
        }

        public ReadOnlyCollection<SyncOperation> SuccessfulSyncOperations
        {
            get
            {
                return null;
            }
        }

        public Microsoft.WPSync.UI.SyncStartType SyncStartType
        {
            get
            {
                return Microsoft.WPSync.UI.SyncStartType.ManualSync;
            }
            set
            {
            }
        }
    }
}

