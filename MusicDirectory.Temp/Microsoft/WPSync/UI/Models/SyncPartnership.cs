namespace Microsoft.WPSync.UI.Models
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.Sync.Source.Zmdb;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Properties;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Media;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    public class SyncPartnership : PropChangeNotifier, ISyncPartnership, INotifyPropertyChanged, IDisposable, ISyncDelegate, IMediaSourceClient
    {
        private List<SyncOperation> attemptedSyncOperations;
        private EventHandler<ContentLoadingEventArgs> _contentLoadingProgress;
        private PartnershipState currentState;
        private bool disposed;
        private readonly ISyncFactory factory;
        private bool isCurrent;
        private bool isFirstSync;
        private bool isResettingRules;
        private bool isTargetLoaded;
        private object lockObject = new object();
        private IMusicSyncSource musicAndMovieProvider;
        private bool musicNeedsLoading;
        private bool musicSourceAttached;
        private IPictureSyncSource photoAndVideoProvider;
        private bool photosNeedLoading;
        private bool photosSourceAttached;
        private readonly BatchingTimer reloadSourcesTimer;
        private EventHandler<ContentLoadingEventArgs> sourceContentLoadingListener;
        private List<SourceResult> sourceErrors = new List<SourceResult>();
        private EventHandler<PartnershipStateChangeEventArgs> _stateChanged;
        private List<SyncOperation> successfulAcquiredOperations;
        private List<SyncOperation> successfulCumulativeSyncOperations;
        private List<SyncOperation> successfulSyncOperations;
        private List<SyncResult> syncErrors = new List<SyncResult>();
        private bool syncStarting;
        private Microsoft.WPSync.UI.SyncStartType syncStartType;
        private EventHandler<SyncStoppedEventArgs> _syncStopped;
        private IZmdbSyncSource zmdbProvider;

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

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SyncPartnership(IDevice device, ISyncFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(typeof(ISyncFactory).ToString());
            }
            this.factory = factory;
            this.sourceContentLoadingListener = new EventHandler<ContentLoadingEventArgs>(this.OnContentLoading);
            this.Device = device;
            this.reloadSourcesTimer = new BatchingTimer(new Action(this.LoadSourceContent), 0x3e8);
            this.InitPartnership();
            this.attemptedSyncOperations = new List<SyncOperation>();
            this.successfulSyncOperations = new List<SyncOperation>();
            this.successfulCumulativeSyncOperations = new List<SyncOperation>();
            this.successfulAcquiredOperations = new List<SyncOperation>();
        }

        private void ApplyRules()
        {
            this.isResettingRules = true;
            this.RuleManager.Apply();
            this.ResetIncludeVideoCounts();
            this.isResettingRules = false;
        }

        private void AttachMusicSource()
        {
            lock (this.lockObject)
            {
                if (!this.musicSourceAttached && this.IsCurrent)
                {
                    this.MusicAndMovieProvider.AddClient(this);
                    this.MusicAndMovieProvider.ContentLoading += this.sourceContentLoadingListener;
                    this.MusicAndMovieProvider.SyncSelectionsModel.SelectionChange += new EventHandler<SelectionChangeEventArgs>(this.OnSelectionChange);
                    this.MusicAndMovieProvider.ModelSourceChanged += new EventHandler<ModelSourceChangedEventArgs>(this.OnMusicAndMovieProviderModelSourceChanged);
                    this.MusicAndMovieProvider.StateChanged += new EventHandler<MediaSyncSourceStateChangeArgs>(this.OnSourceStateChanged);
                    IMusicSyncSource musicAndMovieProvider = this.MusicAndMovieProvider;
                    this.Engine.OperationCompleted += new EventHandler<OperationCompletedArgs>(musicAndMovieProvider.OnEngineOperationCompleted);
                    this.MusicAndMovieProvider.Rules = this.RuleManager;
                    this.musicSourceAttached = true;
                }
            }
        }

        private void AttachPhotosSource()
        {
            lock (this.lockObject)
            {
                if (!this.photosSourceAttached && this.IsCurrent)
                {
                    this.PhotoAndVideoProvider.AddClient(this);
                    this.PhotoAndVideoProvider.ContentLoading += this.sourceContentLoadingListener;
                    this.PhotoAndVideoProvider.SyncSelectionsModel.SelectionChange += new EventHandler<SelectionChangeEventArgs>(this.OnSelectionChange);
                    this.PhotoAndVideoProvider.ModelSourceChanged += new EventHandler<ModelSourceChangedEventArgs>(this.OnPhotoAndVideoProviderModelSourceChanged);
                    this.PhotoAndVideoProvider.StateChanged += new EventHandler<MediaSyncSourceStateChangeArgs>(this.OnSourceStateChanged);
                    this.PhotoAndVideoProvider.Rules = this.RuleManager;
                    this.photosSourceAttached = true;
                }
            }
        }

        private void ClearMusicAndMoviesSource()
        {
            this.Engine.RemoveSource(this.MusicAndMovieProvider);
            this.DetachMusicSource();
        }

        private void ClearPhotoAndVideoSource()
        {
            this.Engine.RemoveSource(this.PhotoAndVideoProvider);
            this.DetachPhotosSource();
        }

        private void ClearTargetSources()
        {
            IZmdbSyncSource zmdbProvider = this.ZmdbProvider;
            this.Engine.OperationCompleted -= new EventHandler<OperationCompletedArgs>(zmdbProvider.OnEngineOperationCompleted);
            this.ZmdbProvider.Dispose();
            this.ZmdbProvider = null;
        }

        private void ContinueAfterLoadingSources()
        {
            Trace.WriteLine("******************* ContinueAfterLoadingSources from partnership " + this.Device.Name);
            if (this.IsCurrent)
            {
                Trace.WriteLine("**** Applying rules " + this.Device.Name);
                this.ApplyRules();
                Trace.WriteLine("**** Verifying music " + this.Device.Name);
                this.VerifyMusicAndMovieSource();
                Trace.WriteLine("**** Verifying photos " + this.Device.Name);
                this.VerifyPhotoAndVideoSource();
            }
            Trace.WriteLine("^^^^^^^^^^^^^^^^^^^^  Exiting ContinueAfterLoadingSources from partnership " + this.Device.Name);
        }

        private void CreateSyncEngine()
        {
            using (new OperationLogger())
            {
                this.Engine = this.factory.CreateISyncEngine();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="Microsoft.WPSync.Logging.Logger.TraceError(System.String,System.String,System.String)"), SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="IOException"), SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="UnauthorizedAccessException")]
        private void CreateSyncRules()
        {
            using (new OperationLogger())
            {
                this.RuleManager = this.factory.CreateISyncRulesManager(this.Device.WinMoDeviceId);
                bool flag = false;
                try
                {
                    this.RuleManager.Load(false);
                }
                catch (XmlException)
                {
                    flag = true;
                }
                if (flag)
                {
                    Errors.ShowError(Microsoft.WPSync.UI.Properties.Resources.SyncRulesCorruptText, new object[0]);
                    try
                    {
                        this.RuleManager.Load(true);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Logger.TraceError("Managed:UI", "UnauthorizedAccessException trying to recreate rules", this.Device.WinMoDeviceId);
                    }
                    catch (IOException)
                    {
                        Logger.TraceError("Managed:UI", "IOException trying to recreate rules", this.Device.WinMoDeviceId);
                    }
                }
            }
        }

        private void DetachMusicSource()
        {
            lock (this.lockObject)
            {
                if (this.musicSourceAttached)
                {
                    this.MusicAndMovieProvider.RemoveClient(this);
                    this.MusicAndMovieProvider.ContentLoading -= this.sourceContentLoadingListener;
                    this.MusicAndMovieProvider.SyncSelectionsModel.SelectionChange -= new EventHandler<SelectionChangeEventArgs>(this.OnSelectionChange);
                    this.MusicAndMovieProvider.ModelSourceChanged -= new EventHandler<ModelSourceChangedEventArgs>(this.OnMusicAndMovieProviderModelSourceChanged);
                    this.MusicAndMovieProvider.StateChanged -= new EventHandler<MediaSyncSourceStateChangeArgs>(this.OnSourceStateChanged);
                    IMusicSyncSource musicAndMovieProvider = this.MusicAndMovieProvider;
                    this.Engine.OperationCompleted -= new EventHandler<OperationCompletedArgs>(musicAndMovieProvider.OnEngineOperationCompleted);
                    this.musicSourceAttached = false;
                }
            }
        }

        private void DetachPhotosSource()
        {
            lock (this.lockObject)
            {
                if (this.photosSourceAttached)
                {
                    this.PhotoAndVideoProvider.RemoveClient(this);
                    this.PhotoAndVideoProvider.ContentLoading -= this.sourceContentLoadingListener;
                    this.PhotoAndVideoProvider.SyncSelectionsModel.SelectionChange -= new EventHandler<SelectionChangeEventArgs>(this.OnSelectionChange);
                    this.PhotoAndVideoProvider.ModelSourceChanged -= new EventHandler<ModelSourceChangedEventArgs>(this.OnPhotoAndVideoProviderModelSourceChanged);
                    this.PhotoAndVideoProvider.StateChanged -= new EventHandler<MediaSyncSourceStateChangeArgs>(this.OnSourceStateChanged);
                    this.photosSourceAttached = false;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isManualDisposing)
        {
            this.IsCurrent = false;
            if (!this.disposed && isManualDisposing)
            {
                this.reloadSourcesTimer.Dispose();
                this.RuleManager.PropertyChanged -= new PropertyChangedEventHandler(this.OnRulesPropertyChanged);
                this.ClearMusicAndMoviesSource();
                this.ClearPhotoAndVideoSource();
                this.ClearTargetSources();
                this.Engine.RemoveSyncDelegate(this);
                this.Engine.StateChanged -= new EventHandler<EngineStateChangeArgs>(this.OnEngineStateChanged);
                this.Engine.Dispose();
            }
            this.disposed = true;
        }

        ~SyncPartnership()
        {
            this.Dispose(false);
        }

        private void FireSourceErrors()
        {
            if (this.PartnershipClient != null)
            {
                List<SourceResult> list;
                lock (this.sourceErrors)
                {
                    list = new List<SourceResult>(this.sourceErrors);
                    this.sourceErrors.Clear();
                }
                if (list.Count > 0)
                {
                    this.PartnershipClient.HandleSourceErrors(this, list);
                }
            }
        }

        private void FireStateChanged(PartnershipState newState, PartnershipState oldState)
        {
            if (this._stateChanged != null)
            {
                this._stateChanged(this, new PartnershipStateChangeEventArgs(newState, oldState));
            }
        }

        private void FireSyncErrors()
        {
            if (this.PartnershipClient != null)
            {
                List<SyncResult> list;
                lock (this.syncErrors)
                {
                    list = new List<SyncResult>(this.syncErrors);
                    this.syncErrors.Clear();
                }
                if (list.Count > 0)
                {
                    this.PartnershipClient.HandleSyncErrors(this, list);
                }
            }
        }

        private void FireSyncStopped(SyncStoppedReason reason)
        {
            this.OnSyncStopped(reason);
            if (this._syncStopped != null)
            {
                this._syncStopped(this, new SyncStoppedEventArgs(reason));
            }
        }

        public SyncErrorResultAction HandleError(SyncResult syncResult)
        {
            if ((syncResult != null) && (syncResult.Operation != SyncOperationType.ConfigureSource))
            {
                if (syncResult.Ignorable == Ignorable.Yes)
                {
                    return SyncErrorResultAction.Ignore;
                }
                switch (syncResult.ResultCode)
                {
                    case 0x800700aa:
                        if (syncResult.Item != null)
                        {
                            return SyncErrorResultAction.Retry;
                        }
                        return SyncErrorResultAction.Abort;

                    case 0x83002113:
                        if (syncResult.Item != null)
                        {
                            return SyncErrorResultAction.Pause;
                        }
                        return SyncErrorResultAction.Abort;
                }
                this.RecordSyncError(syncResult);
            }
            return SyncErrorResultAction.Ignore;
        }

        public SourceErrorResultAction HandleSourceError(SourceResult sourceResult)
        {
            if (sourceResult == null)
            {
                return SourceErrorResultAction.Abort;
            }
            this.RecordSourceError(sourceResult);
            if (sourceResult.CaughtException != null)
            {
                this.PartnershipClient.HandleException(sourceResult.CaughtException);
            }
            return SourceErrorResultAction.Ignore;
        }

        private void InitPartnership()
        {
            this.CreateSyncRules();
            this.CreateSyncEngine();
            this.Engine.AddSyncDelegate(this);
            this.Engine.StateChanged += new EventHandler<EngineStateChangeArgs>(this.OnEngineStateChanged);
            this.Engine.OperationCompleted += new EventHandler<OperationCompletedArgs>(this.OnEngineOperationCompleted);
            this.ResetSources();
            if ((this.MusicAndMovieProvider.CurrentState == MediaSyncSourceState.Idle) && (this.PhotoAndVideoProvider.CurrentState == MediaSyncSourceState.Idle))
            {
                this.CurrentState = PartnershipState.Idle;
            }
            this.musicNeedsLoading = this.MusicAndMovieProvider.CurrentState == MediaSyncSourceState.Created;
            this.photosNeedLoading = this.PhotoAndVideoProvider.CurrentState == MediaSyncSourceState.Created;
            this.RuleManager.AssociatedSyncEngine = this.Engine;
            this.RuleManager.PropertyChanged += new PropertyChangedEventHandler(this.OnRulesPropertyChanged);
        }

        public bool IsEngineStateChangeAllowed(EngineState newState, EngineState oldState)
        {
            switch (newState)
            {
                case EngineState.PreparingSync:
                    return (this.CurrentState == PartnershipState.Idle);

                case EngineState.Syncing:
                    return ((this.CurrentState == PartnershipState.Idle) || (this.CurrentState == PartnershipState.PreparingSync));
            }
            return true;
        }

        public bool IsSourceStateChangeAllowed(MediaSyncSourceState newState, MediaSyncSourceState oldState)
        {
            if (this.syncStarting)
            {
                return false;
            }
            switch (newState)
            {
                case MediaSyncSourceState.Loading:
                    return (((this.CurrentState == PartnershipState.Uninitialized) || (this.CurrentState == PartnershipState.Idle)) || (this.CurrentState == PartnershipState.LoadingSources));

                case MediaSyncSourceState.Verifying:
                    return (((this.CurrentState == PartnershipState.Idle) || (this.CurrentState == PartnershipState.ApplyingRules)) || (this.CurrentState == PartnershipState.VerifyingSources));

                case MediaSyncSourceState.RespondingToChanges:
                    if ((this.CurrentState != PartnershipState.Idle) || !this.MusicAndMovieProvider.IsVerified)
                    {
                        return false;
                    }
                    return this.PhotoAndVideoProvider.IsVerified;
            }
            return true;
        }

        private void LoadMusicAndMovieSource()
        {
            if ((this.MusicAndMovieProvider != null) && this.musicNeedsLoading)
            {
                this.MusicAndMovieProvider.ReloadModelData();
            }
            this.musicNeedsLoading = false;
        }

        private void LoadPhotoAndVideoSource()
        {
            if ((this.PhotoAndVideoProvider != null) && this.photosNeedLoading)
            {
                if (!this.MusicAndMovieProvider.IsLoaded)
                {
                    while (this.MusicAndMovieProvider.CurrentState == MediaSyncSourceState.Loading)
                    {
                        Thread.Sleep(100);
                    }
                }
                this.PhotoAndVideoProvider.ReloadModelData();
            }
            this.photosNeedLoading = false;
        }

        public void LoadSourceContent()
        {
            Trace.WriteLine("******************* Loading sources from partnership " + this.Device.Name);
            this.LoadMusicAndMovieSource();
            this.LoadPhotoAndVideoSource();
            Trace.WriteLine("^^^^^^^^^^^^^^^^^^^^  Exiting Loading sources from partnership " + this.Device.Name);
        }

        private void LoadTargetContent()
        {
            Task.Factory.StartNew(delegate {
                this.ReloadZmdbSource();
            });
        }

        public bool LockForSyncing()
        {
            lock (this.lockObject)
            {
                if (this.currentState != PartnershipState.Idle)
                {
                    return false;
                }
                this.syncStarting = true;
                return true;
            }
        }

        private void OnContentLoading(object sender, ContentLoadingEventArgs args)
        {
            if (this._contentLoadingProgress != null)
            {
                this._contentLoadingProgress(this, args);
            }
        }

        private void OnEngineOperationCompleted(object sender, OperationCompletedArgs e)
        {
            bool flag = (e.Operation.OperationType == SyncOperationType.TransferFrom) && (e.Operation.Item.SyncDirection == SyncDirection.Reverse);
            if (!flag)
            {
                this.attemptedSyncOperations.Add(e.Operation);
            }
            if ((e.SyncResult == null) || (e.SyncResult.ResultCode == 0))
            {
                if (flag)
                {
                    this.successfulAcquiredOperations.Add(e.Operation);
                }
                else
                {
                    this.successfulSyncOperations.Add(e.Operation);
                    this.successfulCumulativeSyncOperations.Add(e.Operation);
                }
            }
        }

        private void OnEngineStateChanged(object sender, EngineStateChangeArgs e)
        {
            switch (e.NewState)
            {
                case EngineState.Idle:
                    this.CurrentState = PartnershipState.Idle;
                    switch (e.OldState)
                    {
                        case EngineState.Cancelling:
                            this.FireSyncStopped(SyncStoppedReason.Cancelled);
                            return;

                        case EngineState.Aborting:
                            this.FireSyncStopped(SyncStoppedReason.Aborted);
                            return;

                        case EngineState.Paused:
                            return;

                        case EngineState.Completing:
                            this.FireSyncStopped(SyncStoppedReason.Completed);
                            return;
                    }
                    return;

                case EngineState.PreparingSync:
                    this.CurrentState = PartnershipState.PreparingSync;
                    this.attemptedSyncOperations.Clear();
                    this.successfulSyncOperations.Clear();
                    this.successfulAcquiredOperations.Clear();
                    this.isFirstSync = !this.ZmdbProvider.SyncedItemCacheExists();
                    this.UnlockForSyncing();
                    return;

                case EngineState.Syncing:
                    this.CurrentState = PartnershipState.Syncing;
                    this.UnlockForSyncing();
                    return;

                case EngineState.Cancelling:
                    if ((e.SyncResult == null) || (e.SyncResult.ResultCode != 0x83002113))
                    {
                        this.CurrentState = PartnershipState.CancelingSync;
                        break;
                    }
                    this.CurrentState = PartnershipState.CancelingSync;
                    if (this.PartnershipClient != null)
                    {
                        this.PartnershipClient.OnDeviceLocked(this);
                    }
                    break;

                case EngineState.Aborting:
                case EngineState.Completing:
                case EngineState.ShuttingDown:
                    return;

                case EngineState.Paused:
                    this.CurrentState = PartnershipState.Idle;
                    return;

                default:
                    return;
            }
            this.UnlockForSyncing();
        }

        private void OnIsCurrentChanged()
        {
            if (this.IsCurrent)
            {
                this.AttachPhotosSource();
                this.AttachMusicSource();
                if (this.MusicAndMovieProvider.IsLoaded && this.PhotoAndVideoProvider.IsLoaded)
                {
                    Task.Factory.StartNew(new Action(this.ContinueAfterLoadingSources));
                }
            }
            else
            {
                this.RuleManager.CancelApply();
                this.DetachPhotosSource();
                this.DetachMusicSource();
            }
        }

        private void OnMusicAndMovieProviderModelSourceChanged(object sender, ModelSourceChangedEventArgs e)
        {
            this.musicNeedsLoading = true;
            this.reloadSourcesTimer.PerformAction();
        }

        private void OnPhotoAndVideoProviderModelSourceChanged(object sender, ModelSourceChangedEventArgs e)
        {
            this.photosNeedLoading = true;
            this.reloadSourcesTimer.PerformAction();
        }

        private void OnRulesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string str;
            foreach (MediaSyncSource source in this.Engine.AllSources)
            {
                source.OnGlobalRuleChanged(e.PropertyName);
            }
            if (((str = e.PropertyName) != null) && (str == "SyncIncludeVideos"))
            {
                this.ResetIncludeVideoCounts();
            }
        }

        private void OnSelectionChange(object sender, SelectionChangeEventArgs args)
        {
            if (!this.isResettingRules)
            {
                this.RuleManager.Save();
            }
        }

        private void OnSourceStateChanged(object sender, MediaSyncSourceStateChangeArgs e)
        {
            lock (this.lockObject)
            {
                if (((this.CurrentState != PartnershipState.PreparingSync) && (this.CurrentState != PartnershipState.Syncing)) && (this.CurrentState != PartnershipState.CancelingSync))
                {
                    switch (e.NewState)
                    {
                        case MediaSyncSourceState.Idle:
                            switch (e.OldState)
                            {
                                case MediaSyncSourceState.Loading:
                                    goto Label_00AA;

                                case MediaSyncSourceState.Verifying:
                                    goto Label_0098;

                                case MediaSyncSourceState.ApplyingRules:
                                    goto Label_00F7;

                                case MediaSyncSourceState.RespondingToChanges:
                                    goto Label_00DB;
                            }
                            goto Label_00F7;

                        case MediaSyncSourceState.Loading:
                            this.CurrentState = PartnershipState.LoadingSources;
                            goto Label_00F7;

                        case MediaSyncSourceState.Verifying:
                            this.CurrentState = PartnershipState.VerifyingSources;
                            goto Label_00F7;

                        case MediaSyncSourceState.ApplyingRules:
                            this.CurrentState = PartnershipState.ApplyingRules;
                            goto Label_00F7;

                        case MediaSyncSourceState.RespondingToChanges:
                            this.CurrentState = PartnershipState.RespondingToChanges;
                            goto Label_00F7;
                    }
                }
                goto Label_00F7;
            Label_0098:
                if (sender == this.PhotoAndVideoProvider)
                {
                    this.CurrentState = PartnershipState.Idle;
                }
                goto Label_00F7;
            Label_00AA:
                if ((sender == this.PhotoAndVideoProvider) || !this.photosNeedLoading)
                {
                    this.CurrentState = PartnershipState.Idle;
                    Task.Factory.StartNew(new Action(this.ContinueAfterLoadingSources));
                }
                goto Label_00F7;
            Label_00DB:
                this.CurrentState = PartnershipState.Idle;
            Label_00F7:;
            }
        }

        private void OnStateChanged(PartnershipState oldState)
        {
            Trace.WriteLine(string.Concat(new object[] { "---- Partnership switched from ", oldState, " to ", this.CurrentState, " ", this.Device.Name }));
            this.FireStateChanged(this.CurrentState, oldState);
            switch (this.CurrentState)
            {
                case PartnershipState.LoadingSources:
                    this.ResetSourceErrors();
                    break;

                case PartnershipState.PreparingSync:
                    this.ResetSyncErrors();
                    break;
            }
            switch (oldState)
            {
                case PartnershipState.VerifyingSources:
                case PartnershipState.RespondingToChanges:
                    this.FireSourceErrors();
                    return;
            }
        }

        private void OnSyncStopped(SyncStoppedReason reason)
        {
            using (new OperationLogger())
            {
                if (this.Device != null)
                {
                    this.Device.RefreshProperties();
                }
                if ((((bool) GlobalSetting.GetApplicationSetting("PlaySoundOnSyncComplete")) && (reason != SyncStoppedReason.Cancelled)) && (reason != SyncStoppedReason.Paused))
                {
                    SystemSounds.Asterisk.Play();
                }
                this.FireSyncErrors();
            }
        }

        public void OnSystemMusicSyncSourceChanged()
        {
            this.ResetMusicAndMovieSource();
            if (this.IsCurrent)
            {
                this.musicNeedsLoading = true;
                this.LoadSourceContent();
            }
        }

        private void RecordSourceError(SourceResult result)
        {
            lock (this.sourceErrors)
            {
                this.sourceErrors.Add(result);
            }
        }

        private void RecordSyncError(SyncResult result)
        {
            lock (this.syncErrors)
            {
                this.syncErrors.Add(result);
            }
        }

        private void ReloadZmdbSource()
        {
            try
            {
                if (this.ZmdbProvider != null)
                {
                    this.ZmdbProvider.ReloadModelData();
                }
            }
            catch (ZmdbProviderException)
            {
                Errors.ShowError(Microsoft.WPSync.UI.Properties.Resources.ZmdbErrorText, new object[0]);
            }
        }

        public void ResetAndReloadTargets()
        {
            if (!this.isTargetLoaded)
            {
                this.ResetTargetSource();
                this.LoadTargetContent();
                this.isTargetLoaded = true;
            }
        }

        private void ResetIncludeVideoCounts()
        {
            Task.Factory.StartNew(delegate {
                this.PhotoAndVideoProvider.ResetPhotoVideoCounts(this.RuleManager.SyncIncludeVideos);
            });
        }

        private void ResetMusicAndMovieSource()
        {
            if (this.MusicAndMovieProvider != null)
            {
                this.ClearMusicAndMoviesSource();
            }
            this.MusicAndMovieProvider = this.factory.CreateMusicSyncSource();
            this.Engine.AddSource(this.MusicAndMovieProvider);
            this.AttachMusicSource();
        }

        private void ResetPhotoAndVideoSource()
        {
            if (this.PhotoAndVideoProvider != null)
            {
                this.ClearPhotoAndVideoSource();
            }
            this.PhotoAndVideoProvider = this.factory.CreatePictureSyncSource();
            this.Engine.AddSource(this.PhotoAndVideoProvider);
            this.AttachPhotosSource();
        }

        private void ResetSourceErrors()
        {
            lock (this.sourceErrors)
            {
                this.sourceErrors.Clear();
            }
        }

        private void ResetSources()
        {
            this.ResetMusicAndMovieSource();
            this.ResetPhotoAndVideoSource();
        }

        private void ResetSyncErrors()
        {
            lock (this.syncErrors)
            {
                this.syncErrors.Clear();
            }
        }

        private void ResetTargetSource()
        {
            this.Engine.ClearTargets();
            this.ZmdbProvider = this.factory.CreateZmdbSyncSource(this.Device);
            this.ZmdbProvider.PicturesDirectoryFormat = Microsoft.WPSync.UI.Properties.Resources.PicturesTransferDirectoryFormat;
            this.Engine.AddTarget(this.ZmdbProvider);
            IZmdbSyncSource zmdbProvider = this.ZmdbProvider;
            this.Engine.OperationCompleted += new EventHandler<OperationCompletedArgs>(zmdbProvider.OnEngineOperationCompleted);
        }

        public void UnlockForSyncing()
        {
            this.syncStarting = false;
        }

        private void VerifyMusicAndMovieSource()
        {
            if (this.MusicAndMovieProvider != null)
            {
                this.MusicAndMovieProvider.VerifyModelData();
            }
        }

        private void VerifyPhotoAndVideoSource()
        {
            if (this.PhotoAndVideoProvider != null)
            {
                this.PhotoAndVideoProvider.VerifyModelData();
            }
        }

        public ReadOnlyCollection<SyncOperation> AttemptedSyncOperations
        {
            get
            {
                return new ReadOnlyCollection<SyncOperation>(this.attemptedSyncOperations);
            }
        }

        public PartnershipState CurrentState
        {
            get
            {
                return this.currentState;
            }
            set
            {
                if (this.currentState != value)
                {
                    PartnershipState currentState = this.currentState;
                    this.currentState = value;
                    this.OnStateChanged(currentState);
                    this.OnPropertyChanged("CurrentState");
                }
            }
        }

        public IDevice Device { get; private set; }

        public ISyncEngine Engine { get; private set; }

        public bool IsCurrent
        {
            get
            {
                return this.isCurrent;
            }
            set
            {
                if (this.isCurrent != value)
                {
                    this.isCurrent = value;
                    this.OnIsCurrentChanged();
                    this.OnPropertyChanged("IsCurrent");
                }
            }
        }

        public bool IsFirstSync
        {
            get
            {
                return this.isFirstSync;
            }
        }

        public IMusicSyncSource MusicAndMovieProvider
        {
            get
            {
                return this.musicAndMovieProvider;
            }
            set
            {
                this.musicAndMovieProvider = value;
                this.OnPropertyChanged("MusicAndMovieProvider");
            }
        }

        public IPartnershipClient PartnershipClient { get; set; }

        public IPictureSyncSource PhotoAndVideoProvider
        {
            get
            {
                return this.photoAndVideoProvider;
            }
            set
            {
                this.photoAndVideoProvider = value;
                this.OnPropertyChanged("PhotoAndVideoProvider");
            }
        }

        public ISyncRulesManager RuleManager { get; private set; }

        public ReadOnlyCollection<SyncOperation> SuccessfulAcquiredOperations
        {
            get
            {
                return new ReadOnlyCollection<SyncOperation>(this.successfulAcquiredOperations);
            }
        }

        public ReadOnlyCollection<SyncOperation> SuccessfulCumulativeSyncOperations
        {
            get
            {
                return new ReadOnlyCollection<SyncOperation>(this.successfulCumulativeSyncOperations);
            }
        }

        public ReadOnlyCollection<SyncOperation> SuccessfulSyncOperations
        {
            get
            {
                return new ReadOnlyCollection<SyncOperation>(this.successfulSyncOperations);
            }
        }

        public Microsoft.WPSync.UI.SyncStartType SyncStartType
        {
            get
            {
                return this.syncStartType;
            }
            set
            {
                this.syncStartType = value;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Zmdb")]
        public IZmdbSyncSource ZmdbProvider
        {
            get
            {
                return this.zmdbProvider;
            }
            set
            {
                this.zmdbProvider = value;
                this.OnPropertyChanged("ZmdbProvider");
            }
        }
    }
}

